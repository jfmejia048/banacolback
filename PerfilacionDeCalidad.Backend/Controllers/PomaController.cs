using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Data.Entities;
using PerfilacionDeCalidad.Backend.Enum;
using PerfilacionDeCalidad.Backend.Helpers;
using PerfilacionDeCalidad.Backend.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Usuario", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PomaController : ControllerBase
    {
        private readonly DataContext _dataContext;
        IHubContext<NotificationHub, HubHelper> _NotificationHubContext;

        public PomaController(DataContext dataContext, IHubContext<NotificationHub, HubHelper> NotiHubContext)
        {
            _NotificationHubContext = NotiHubContext;
            _dataContext = dataContext;
        }        

        [HttpPost]
        [Route("Get")]
        public IActionResult GetPomas()
        {
            try
            {
                var List = Get();
                return Ok(new { Data = List, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public object Get()
        {
            List<object> retList = new List<object>();

            PomasLogic pomasLogic = new PomasLogic(this._dataContext);
            var List = pomasLogic.GetData();

            List.ToList().ForEach(x =>
            {
                List<int> palets = x.Frutas.FirstOrDefault().Pallets.Select(s => s.IdPallet).ToList();
                if(!_dataContext.Tracking.Where(a => palets.Contains(a.Palet.ID)).Any())
                {
                    retList.Add(x);
                }
            });

            return retList;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreatePoma([FromBody]List<CreatePomas> Pomas)
        {
            List<Pomas> ListPoma = new List<Pomas>();
            try {
                ListPoma = await CreateMasivo(Pomas);
                return Ok(new { Data = ListPoma, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Pomas>> Create(List<Pomas> Pomas)
        {
            List<Pomas> ListPoma = new List<Pomas>();
            foreach (var Poma in Pomas)
            {
                if (!this.ExistPoma(Poma.Codigo))
                {
                    Pomas P = new Pomas();
                    P.ID = Poma.ID;
                    P.Codigo = Poma.Codigo;
                    P.Numero = Poma.Numero;
                    P.Placa = Poma.Placa;
                    P.Estado = (int)EstadosPoma.NoChequeado;
                    P.FechaRegistro = Poma.FechaRegistro;
                    P.Recibido = false;
                    P.Delete = true;
                    ListPoma.Add(P);
                }
            }
            _dataContext.Pomas.AddRange(ListPoma);
            await _dataContext.SaveChangesAsync();
            return ListPoma;
        }

        public async Task<List<Pomas>> CreateMasivo(List<CreatePomas> Pomas)
        {
            ExportadorController exportadorController = new ExportadorController(_dataContext);
            PuertoController puertoController = new PuertoController(_dataContext);
            BuqueController buqueController = new BuqueController(_dataContext);
            DestinoController destinoController = new DestinoController(_dataContext);
            FrutasController frutasController = new FrutasController(_dataContext);
            FincaController fincaController = new FincaController(_dataContext);
            PaletController paletController = new PaletController(_dataContext);
            List<Pomas> ListPoma = new List<Pomas>();
            foreach (var Poma in Pomas)
            {
                try
                {
                    List<Pomas> pomas = new List<Pomas>();
                    if (!this.ExistPoma(Poma.Codigo))
                    {
                        Pomas P = new Pomas();
                        P.Codigo = Poma.Codigo;
                        P.Numero = Poma.Numero;
                        P.Placa = Poma.Placa;
                        P.Estado = (int)EstadosPoma.NoChequeado;
                        P.FechaRegistro = DateTime.UtcNow;
                        P.Recibido = false;
                        P.Delete = true;
                        pomas.Add(P);
                        pomas = await this.Create(pomas);
                    }
                    else
                    {
                        pomas.Add(_dataContext.Pomas.First(x => x.Codigo == Poma.Codigo));
                    }

                    List<Fincas> finca = new List<Fincas>();
                    if (!fincaController.ExistFinca(Poma.Finca.Codigo))
                    {
                        Poma.Finca.Pomas = pomas.First();
                        finca.Add(Poma.Finca);
                        finca = await fincaController.Create(finca);
                    }
                    else
                    {
                        finca.Add(_dataContext.Fincas.First(x => x.Codigo == Poma.Finca.Codigo));
                    }

                    List<Puertos> puertos = new List<Puertos>();
                    if (!puertoController.ExistPuerto(Poma.Puerto.Codigo))
                    {
                        puertos.Add(Poma.Puerto);
                        puertos = await puertoController.Create(puertos);
                    }
                    else
                    {
                        puertos.Add(_dataContext.Puertos.First(x => x.Codigo == Poma.Puerto.Codigo));
                    }

                    foreach (var detail in Poma.DetailPoma)
                    {
                        List<Frutas> frutas = new List<Frutas>();
                        if (!frutasController.ExistFruta(detail.Frutas.Codigo))
                        {
                            detail.Frutas.Poma = pomas.First();
                            frutas.Add(detail.Frutas);
                            frutas = await frutasController.Create(frutas);
                        }
                        else
                        {
                            frutas.Add(_dataContext.Frutas.First(x => x.Codigo == detail.Frutas.Codigo));
                        }

                        List<Destinos> destinos = new List<Destinos>();
                        if (!destinoController.ExistDestino(detail.Destino.Codigo))
                        {
                            destinos.Add(detail.Destino);
                            destinos = await destinoController.Create(destinos);
                        }
                        else
                        {
                            destinos.Add(_dataContext.Destinos.First(x => x.Codigo == detail.Destino.Codigo));
                        }

                        List<Buques> buques = new List<Buques>();
                        if (!buqueController.ExistBuque(detail.Buque.Codigo))
                        {
                            buques.Add(detail.Buque);
                            buques = await buqueController.Create(buques);
                        }
                        else
                        {
                            buques.Add(_dataContext.Buques.First(x => x.Codigo == detail.Buque.Codigo));
                        }

                        List<Exportadores> exportadores = new List<Exportadores>();
                        if (!exportadorController.ExistExportador(detail.Exportador.Codigo))
                        {
                            exportadores.Add(detail.Exportador);
                            exportadores = await exportadorController.Create(exportadores);
                        }
                        else
                        {
                            exportadores.Add(_dataContext.Exportadores.First(x => x.Codigo == detail.Exportador.Codigo));
                        }

                        foreach (var palet in detail.Palets)
                        {
                            if (!_dataContext.Palets.Any(x => x.Codigo == palet.Codigo))
                            {
                                Palets Palet = new Palets();
                                Palet.Codigo = palet.Codigo;
                                Palet.CodigoPalet = palet.CodigoPalet;
                                Palet.LlegadaCamion = palet.LlegadaCamion;
                                Palet.SalidaFinca = palet.SalidaFinca;
                                Palet.Estimado = palet.Estimado;
                                Palet.LlegadaTerminal = palet.LlegadaTerminal;
                                Palet.LecturaPalet = palet.LecturaPalet;
                                Palet.UsuarioLectura = palet.UsuarioLectura;
                                Palet.InspeccionPalet = palet.InspeccionPalet;
                                Palet.CaraPalet = palet.CaraPalet;
                                Palet.NumeroCajas = palet.NumeroCajas;
                                Palet.Carga = palet.Carga;
                                Palet.Perfilar = false;
                                Palet.Fruta = frutas.FirstOrDefault();
                                Palet.Puerto = puertos.FirstOrDefault();
                                Palet.Buque = buques.FirstOrDefault();
                                Palet.Destino = destinos.FirstOrDefault();
                                Palet.Exportador = exportadores.FirstOrDefault();
                                _dataContext.Palets.Add(Palet);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    continue;
                }
            }
            
            await _dataContext.SaveChangesAsync();
            try
            {
                await _NotificationHubContext.Clients.All.BroadcastMessage(Get());
            }
            catch (Exception ex)
            {
                return ListPoma;
            }
            return ListPoma;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditPoma(Pomas Poma)
        {
            if (_dataContext.Pomas.Any(x => x.Codigo == Poma.ID))
            {
                var poma = _dataContext.Pomas.First(x => x.Codigo == Poma.ID);
                poma.Numero = Poma.Numero;
                poma.Placa = Poma.Placa;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = poma, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "La Poma con codigo " + Poma.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeletePoma(Pomas Poma)
        {
            if (_dataContext.Pomas.Any(x => x.Codigo == Poma.ID))
            {
                var poma = _dataContext.Pomas.First(x => x.Codigo == Poma.ID);
                poma.Delete = !poma.Delete;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = poma, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "La finca con codigo " + Poma.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistPoma(int id)
        {
            return _dataContext.Pomas.Any(x => x.Codigo == id);
        }

        [HttpPost]
        [Route("ReceiveBoxes")]
        public async Task<IActionResult> ReceiveBoxes(Pomas Pomas)
        {
            try
            {
                var poma = _dataContext.Pomas.Where(x => x.Codigo == Pomas.Codigo).FirstOrDefault();
                if(poma == null)
                {
                    return NotFound();
                }
                poma.Recibido = true;
                await _dataContext.SaveChangesAsync();
                return Ok(new { Data = "Editado correctamente", Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }
    }
}
