using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Data.Entities;
using PerfilacionDeCalidad.Backend.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            var Pomas = (from Poma in _dataContext.Pomas
                         join Caja in _dataContext.Cajas on Poma.Codigo equals Caja.Pomas.Codigo
                         join Fruta in _dataContext.Frutas on Caja.Frutas.ID equals Fruta.ID
                         join Palet in _dataContext.Palets on Caja.Codigo equals Palet.Caja.Codigo
                         join Finca in _dataContext.Fincas on Palet.Finca.Codigo equals Finca.Codigo
                         join Puerto in _dataContext.Puertos on Palet.Puerto.Codigo equals Puerto.Codigo
                         join Buque in _dataContext.Buques on Palet.Buque.Codigo equals Buque.Codigo
                         join Exportador in _dataContext.Exportadores on Palet.Exportador.Codigo equals Exportador.Codigo
                         join Destino in _dataContext.Destinos on Palet.Destino.Codigo equals Destino.Codigo
                         select new
                         {
                             CodigoPalet = Palet.Codigo,
                             Finca = Finca.FincaName,
                             TerminalDestino = Puerto.PuertoName,
                             Poma = Poma.Numero,
                             Fruta = Fruta.FrutaName,
                             Buque = Buque.BuqueName,
                             Llegada = Palet.LlegadaTerminal,
                             Salida = Palet.SalidaFinca,
                             Estimado = Palet.Estimado,
                             LlegadaTerminal = Palet.LlegadaTerminal,
                             Cajas = Caja.Estado,
                             Exportador = Exportador.ExportadorName,
                             Destino = Destino.DestinoName,
                             Carga = Palet.Carga,
                             CodigoDeBarras = Palet.CodigoPalet,
                             CajasPalet = Palet.NumeroCajas,
                             Palet.Perfilar
                         }).ToList();

            var List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma })
                .Select(x => new {
                    x.FirstOrDefault().Finca,
                    x.FirstOrDefault().TerminalDestino,
                    x.FirstOrDefault().Poma,
                    Frutas = x.GroupBy(g2 => g2.Fruta).Select(s2 => new {
                        s2.FirstOrDefault().Fruta,
                        s2.FirstOrDefault().Buque,
                        s2.FirstOrDefault().Llegada,
                        s2.FirstOrDefault().Salida,
                        s2.FirstOrDefault().Estimado,
                        s2.FirstOrDefault().LlegadaTerminal,
                        s2.FirstOrDefault().Cajas,
                        s2.FirstOrDefault().Exportador,
                        s2.FirstOrDefault().Destino,
                        Palet = s2.Select(s3 => new
                        {
                            s3.CodigoPalet,
                            s3.Carga,
                            s3.CodigoDeBarras,
                            s3.CajasPalet,
                            s3.Perfilar
                        })
                    }).ToList()
                });
            return List;
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
                if (!this.ExistPoma(Poma.ID))
                {
                    Pomas P = new Pomas();
                    P.Codigo = Poma.ID;
                    P.Numero = Poma.Numero;
                    P.Placa = Poma.Placa;
                    P.Estado = true;
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
            CajaController cajaController = new CajaController(_dataContext, _NotificationHubContext);
            PaletController paletController = new PaletController(_dataContext);
            List<Pomas> ListPoma = new List<Pomas>();
            foreach (var Poma in Pomas)
            {
                try
                {
                    List<Pomas> pomas = new List<Pomas>();
                    if (!this.ExistPoma(Poma.ID))
                    {
                        Pomas P = new Pomas();
                        P.ID = Poma.ID;
                        P.Codigo = Poma.ID;
                        P.Numero = Poma.Numero;
                        P.Placa = Poma.Placa;
                        P.Estado = true;
                        pomas.Add(P);
                        pomas = await this.Create(pomas);
                    }
                    else
                    {
                        pomas.Add(_dataContext.Pomas.First(x => x.Codigo == Poma.ID));
                    }

                    List<Frutas> frutas = new List<Frutas>();
                    if (!frutasController.ExistFruta(Poma.Frutas.ID))
                    {
                        frutas.Add(Poma.Frutas);
                        frutas = await frutasController.Create(frutas);
                    }
                    else
                    {
                        frutas.Add(_dataContext.Frutas.First(x => x.Codigo == Poma.Frutas.ID));
                    }

                    List<Cajas> cajas = new List<Cajas>();
                    if (!cajaController.ExistCaja(Poma.Cajas.ID))
                    {
                        Poma.Cajas.Frutas = frutas.First();
                        Poma.Cajas.Pomas = pomas.First();
                        cajas.Add(Poma.Cajas);
                        cajas = await cajaController.Create(cajas);
                    }
                    else
                    {
                        cajas.Add(_dataContext.Cajas.First(x => x.Codigo == Poma.Cajas.ID));
                    }

                    List<Fincas> finca = new List<Fincas>();
                    if (!fincaController.ExistFinca(Poma.Finca.ID))
                    {
                        finca.Add(Poma.Finca);
                        finca = await fincaController.Create(finca);
                    }
                    else
                    {
                        finca.Add(_dataContext.Fincas.First(x => x.Codigo == Poma.Finca.ID));
                    }

                    List<Puertos> puertos = new List<Puertos>();
                    if (!puertoController.ExistPuerto(Poma.Puerto.ID))
                    {
                        puertos.Add(Poma.Puerto);
                        puertos = await puertoController.Create(puertos);
                    }
                    else
                    {
                        puertos.Add(_dataContext.Puertos.First(x => x.Codigo == Poma.Puerto.ID));
                    }

                    List<Buques> buques = new List<Buques>();
                    if (!buqueController.ExistBuque(Poma.Buque.ID))
                    {
                        buques.Add(Poma.Buque);
                        buques = await buqueController.Create(buques);
                    }
                    else
                    {
                        buques.Add(_dataContext.Buques.First(x => x.Codigo == Poma.Buque.ID));
                    }

                    List<Destinos> destinos = new List<Destinos>();
                    if (!destinoController.ExistDestino(Poma.Destino.ID))
                    {
                        destinos.Add(Poma.Destino);
                        destinos = await destinoController.Create(destinos);
                    }
                    else
                    {
                        destinos.Add(_dataContext.Destinos.First(x => x.Codigo == Poma.Destino.ID));
                    }

                    List<Exportadores> exportadores = new List<Exportadores>();
                    if (!exportadorController.ExistExportador(Poma.Exportador.ID))
                    {
                        exportadores.Add(Poma.Exportador);
                        exportadores = await exportadorController.Create(exportadores);
                    }
                    else
                    {
                        exportadores.Add(_dataContext.Exportadores.First(x => x.Codigo == Poma.Exportador.ID));
                    }

                    foreach (var palet in Poma.Palets)
                    {
                        if (!_dataContext.Palets.Any(x => x.Codigo == palet.ID))
                        {
                            Palets Palet = new Palets();
                            Palet.Codigo = palet.ID;
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
                            Palet.Perfilar = palet.Perfilar;
                            Palet.Finca = _dataContext.Fincas.First(x => x.Codigo == Poma.Finca.ID);
                            Palet.Puerto = _dataContext.Puertos.First(x => x.Codigo == Poma.Puerto.ID);
                            Palet.Buque = _dataContext.Buques.First(x => x.Codigo == Poma.Buque.ID);
                            Palet.Destino = _dataContext.Destinos.First(x => x.Codigo == Poma.Destino.ID);
                            Palet.Exportador = _dataContext.Exportadores.First(x => x.Codigo == Poma.Exportador.ID);
                            Palet.Caja = _dataContext.Cajas.First(x => x.Codigo == Poma.Cajas.ID);
                            _dataContext.Palets.Add(Palet);
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
                poma.Estado = !poma.Estado;
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
    }
}
