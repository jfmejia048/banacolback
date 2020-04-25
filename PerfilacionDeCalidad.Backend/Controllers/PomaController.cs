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
                List<int> palets = x.Frutas.Select(s => s.IdPallet).ToList();
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
            List<Vehiculos> ListPoma = new List<Vehiculos>();
            try {
                ListPoma = await CreateMasivo(Pomas);
                return Ok(new { Data = ListPoma, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Vehiculos> Create(Vehiculos Vehiculo)
        {
            if (!this.ExistPoma(Vehiculo.Placa))
            {
                _dataContext.Pomas.Add(Vehiculo);
            }
            else
            {
                return _dataContext.Pomas.FirstOrDefault(x => x.Placa.ToLower() == Vehiculo.Placa.ToLower());
            }
            await _dataContext.SaveChangesAsync();
            return Vehiculo;
        }

        public async Task<List<Vehiculos>> CreateMasivo(List<CreatePomas> Pomas)
        {
            ExportadorController exportadorController = new ExportadorController(_dataContext);
            PuertoController puertoController = new PuertoController(_dataContext);
            BuqueController buqueController = new BuqueController(_dataContext);
            DestinoController destinoController = new DestinoController(_dataContext);
            FrutasController frutasController = new FrutasController(_dataContext);
            FincaController fincaController = new FincaController(_dataContext);
            PaletController paletController = new PaletController(_dataContext);
            List<Vehiculos> ListPoma = new List<Vehiculos>();
            foreach (var Poma in Pomas)
            {
                try
                {
                    Vehiculos Vehiculo = new Vehiculos();
                    Vehiculos V = new Vehiculos();
                    V.Placa = Poma.Placa;
                    Vehiculo = await this.Create(V);

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

                    Destinos Destino = new Destinos();
                    Destinos D = new Destinos();
                    D.Codigo = Poma.Destino.Codigo;
                    D.DestinoName = Poma.Destino.DestinoName;
                    Destino = await destinoController.Create(D);

                    Buques Buque = new Buques();
                    Buques B = new Buques();
                    B.Codigo = Poma.Buque.Codigo;
                    B.BuqueName = Poma.Buque.BuqueName;
                    Buque = await buqueController.Create(B);

                    Exportadores Exportador = new Exportadores();
                    Exportadores E = new Exportadores();
                    E.Codigo = Poma.Exportador.Codigo;
                    E.ExportadorName = Poma.Exportador.ExportadorName;
                    Exportador = await exportadorController.Create(E);

                    TransportGuide TransportGuide = new TransportGuide();
                    TransportGuide.Vehiculo = Vehiculo;
                    TransportGuide.Numero = Poma.Numero;
                    TransportGuide.Estado = (int)EstadosPoma.NoChequeado;
                    TransportGuide.FechaRegistro = DateTime.UtcNow;
                    TransportGuide.Recibido = false;
                    TransportGuide.LlegadaCamion = Poma.LlegadaCamion;
                    TransportGuide.SalidaFinca = Poma.SalidaFinca;
                    TransportGuide.Estimado = Poma.Estimado;
                    TransportGuide.LlegadaTerminal = Poma.LlegadaTerminal;
                    TransportGuide.Finca = Finca;
                    TransportGuide.Puerto = Puerto;
                    TransportGuide.Buque = Buque;
                    TransportGuide.Destino = Destino;
                    TransportGuide.Exportador = Exportador;

                    _dataContext.TransportGuides.Add(TransportGuide);
                    await _dataContext.SaveChangesAsync();

                    foreach (var detail in Poma.DetailPoma)
                    {
                        Frutas Fruta = new Frutas();
                        Frutas FR = new Frutas();
                        FR.Codigo = detail.Frutas.Codigo;
                        FR.FrutaName = detail.Frutas.FrutaName;
                        Fruta = await frutasController.Create(FR);

                        DetailTransportGuide DetailTG = new DetailTransportGuide();
                        DetailTG.TransportGuide = TransportGuide;
                        DetailTG.Fruta = Fruta;
                        _dataContext.DetailTransportGuide.Add(DetailTG);
                        await _dataContext.SaveChangesAsync();

                        foreach (var palet in detail.Palets)
                        {
                            Pallets Palet = new Pallets();
                            Palet.CodigoPalet = palet.CodigoPalet;
                            Palet.LecturaPalet = new DateTime();
                            Palet.UsuarioInspeccion = "";
                            Palet.InspeccionPalet = new DateTime();
                            Palet.CaraPalet = palet.CaraPalet;
                            Palet.NumeroCajas = palet.NumeroCajas;
                            Palet.Carga = palet.Carga;
                            Palet.Tipo = palet.Tipo;
                            Palet.Perfilar = false;
                            Palet.DetailTransportGuide = DetailTG;
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

        public bool ExistPoma(string placa)
        {
            return _dataContext.Pomas.Any(x => x.Placa.ToLower() == placa.ToLower());
        }

        [HttpPost]
        [Route("ReceiveBoxes")]
        public async Task<IActionResult> ReceiveBoxes(TransportGuide TG)
        {
            try
            {
                var result = _dataContext.TransportGuides.Where(x => x.Numero == TG.Numero).FirstOrDefault();
                if(result == null)
                {
                    return NotFound();
                }
                result.Recibido = true;
                result.LlegadaTerminal = DateTime.UtcNow;
                await _dataContext.SaveChangesAsync();
                try
                {
                    await _NotificationHubContext.Clients.All.BroadcastMessage(Get());
                }
                catch (Exception ex)
                {
                    return Ok(new { Data = "Editado correctamente", Success = true });
                }
                return Ok(new { Data = "Editado correctamente", Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }
    }
}
