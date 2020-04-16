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

        public async Task<Pomas> Create(Pomas Poma)
        {
            if (!this.ExistPoma(Poma.Codigo))
            {
                _dataContext.Pomas.Add(Poma);
            }
            else
            {
                return _dataContext.Pomas.FirstOrDefault(x => x.Codigo == Poma.Codigo);
            }
            await _dataContext.SaveChangesAsync();
            return Poma;
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
                    Pomas PomaObj = new Pomas();
                    Pomas P = new Pomas();
                    P.Codigo = Poma.Codigo;
                    P.Numero = Poma.Numero;
                    P.Placa = Poma.Placa;
                    PomaObj = await this.Create(P);

                    Fincas Finca = new Fincas();
                    Fincas F = new Fincas();
                    F.Codigo = Poma.Finca.Codigo;
                    F.FincaName = Poma.Finca.FincaName;
                    Finca = await fincaController.Create(F);


                    Puertos Puerto = new Puertos();
                    Puertos PU = new Puertos();
                    PU.Codigo = Poma.Puerto.Codigo;
                    PU.PuertoName = Poma.Puerto.PuertoName;
                    Puerto = await puertoController.Create(PU);

                    TransportGuide TransportGuide = new TransportGuide();
                    TransportGuide.Estado = (int)EstadosPoma.NoChequeado;
                    TransportGuide.FechaRegistro = DateTime.UtcNow;
                    TransportGuide.Recibido = false;
                    TransportGuide.LlegadaCamion = Poma.LlegadaCamion;
                    TransportGuide.SalidaFinca = Poma.SalidaFinca;
                    TransportGuide.Estimado = Poma.Estimado;
                    TransportGuide.LlegadaTerminal = Poma.LlegadaTerminal;
                    TransportGuide.Poma = PomaObj;
                    TransportGuide.Finca = Finca;
                    TransportGuide.Puerto = Puerto;
                    _dataContext.TransportGuides.Add(TransportGuide);
                    await _dataContext.SaveChangesAsync();

                    foreach (var detail in Poma.DetailPoma)
                    {
                        Frutas Fruta = new Frutas();
                        Frutas FR = new Frutas();
                        FR.Codigo = detail.Frutas.Codigo;
                        FR.FrutaName = detail.Frutas.FrutaName;
                        Fruta = await frutasController.Create(FR);

                        Destinos Destino = new Destinos();
                        Destinos D = new Destinos();
                        D.Codigo = detail.Destino.Codigo;
                        D.DestinoName = detail.Destino.DestinoName;
                        Destino = await destinoController.Create(D);

                        Buques Buque = new Buques();
                        Buques B = new Buques();
                        B.Codigo = detail.Buque.Codigo;
                        B.BuqueName = detail.Buque.BuqueName;
                        Buque = await buqueController.Create(B);

                        Exportadores Exportador = new Exportadores();
                        Exportadores E = new Exportadores();
                        E.Codigo = detail.Exportador.Codigo;
                        E.ExportadorName = detail.Exportador.ExportadorName;
                        Exportador = await exportadorController.Create(E);

                        DetailTransportGuide DetailTG = new DetailTransportGuide();
                        DetailTG.TransportGuide = TransportGuide;
                        DetailTG.Fruta = Fruta;
                        DetailTG.Destino = Destino;
                        DetailTG.Buque = Buque;
                        DetailTG.Exportador = Exportador;
                        _dataContext.DetailTransportGuide.Add(DetailTG);
                        await _dataContext.SaveChangesAsync();

                        foreach (var palet in detail.Palets)
                        {
                            if (!_dataContext.Palets.Any(x => x.Codigo == palet.Codigo))
                            {
                                Palets Palet = new Palets();
                                Palet.Codigo = palet.Codigo;
                                Palet.CodigoPalet = palet.CodigoPalet;
                                Palet.LecturaPalet = new DateTime();
                                Palet.UsuarioLectura = "";
                                Palet.InspeccionPalet = new DateTime();
                                Palet.CaraPalet = palet.CaraPalet;
                                Palet.NumeroCajas = palet.NumeroCajas;
                                Palet.Carga = palet.Carga;
                                Palet.Perfilar = false;
                                Palet.DetailTransportGuide = DetailTG;
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

        public bool ExistPoma(int id)
        {
            return _dataContext.Pomas.Any(x => x.Codigo == id);
        }

        [HttpPost]
        [Route("ReceiveBoxes")]
        public async Task<IActionResult> ReceiveBoxes(TransportGuide TG)
        {
            try
            {
                var result = _dataContext.TransportGuides.Where(x => x.ID == TG.ID).FirstOrDefault();
                if(result == null)
                {
                    return NotFound();
                }
                result.Recibido = true;
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
