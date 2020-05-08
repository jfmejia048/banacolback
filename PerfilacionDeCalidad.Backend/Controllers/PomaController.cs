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
            string result = "";
            try {
                result = await CreateMasivo(Pomas);
                return Ok(new { Data = result, Success = true });
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

        public async Task<string> CreateMasivo(List<CreatePomas> Pomas)
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
                    TransportGuide.LlegadaCamion = Poma.LlegadaCamion.ToUniversalTime();
                    TransportGuide.SalidaFinca = Poma.SalidaFinca.ToUniversalTime();
                    TransportGuide.Estimado = Poma.Estimado.ToUniversalTime();
                    TransportGuide.LlegadaTerminal = null;
                    TransportGuide.Finca = Finca;
                    TransportGuide.Puerto = Puerto;
                    TransportGuide.Buque = Buque;
                    TransportGuide.Destino = Destino;
                    TransportGuide.Exportador = Exportador;

                    bool EditTG = false;
                    TransportGuide valTG = this.ExistTransportGuide(Poma.Numero);
                    if(valTG == null)
                    {
                        _dataContext.TransportGuides.Add(TransportGuide);
                        await _dataContext.SaveChangesAsync();
                    }
                    else
                    {
                        var currentDate = DateTime.UtcNow.ToLocalTime();
                        var StartDate = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 3, 0, 0);
                        var EndDate = StartDate.AddDays(1);
                        EndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 2, 59, 0);
                        if (currentDate < StartDate)
                        {
                            StartDate = StartDate.AddDays(-1);
                            EndDate = EndDate.AddDays(-1);
                        }
                        if((valTG.FechaRegistro >= StartDate && valTG.FechaRegistro <= EndDate) && valTG.Estado == 0)
                        {
                            EditTG = true;
                        }
                        else
                        {
                            return "La poma ya existe";
                        }
                    }

                    foreach (var detail in Poma.DetailPoma)
                    {
                        Frutas Fruta = new Frutas();
                        Frutas FR = new Frutas();
                        FR.Codigo = detail.Frutas.Codigo;
                        FR.FrutaName = detail.Frutas.FrutaName;
                        Fruta = await frutasController.Create(FR);

                        DetailTransportGuide valDTG = null;
                        DetailTransportGuide DetailTG = new DetailTransportGuide();
                        DetailTG.Fruta = Fruta;
                        if (EditTG)
                        {
                            valDTG = this.ExistDetailTransportGuide(valTG.ID, Fruta.ID);
                            if(valDTG == null)
                            {
                                DetailTG.TransportGuide = valTG;
                                _dataContext.DetailTransportGuide.Add(DetailTG);
                                await _dataContext.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            DetailTG.TransportGuide = TransportGuide;
                            _dataContext.DetailTransportGuide.Add(DetailTG);
                            await _dataContext.SaveChangesAsync();
                        }

                        foreach (var palet in detail.Palets)
                        {
                            Pallets Palet = new Pallets();
                            Palet.CodigoPalet = palet.CodigoPalet;
                            Palet.UsuarioLectura = "";
                            Palet.LecturaPalet = DateTime.UtcNow;
                            Palet.UsuarioInspeccion = "";
                            Palet.InspeccionPalet = DateTime.UtcNow;
                            Palet.CaraPalet = palet.CaraPalet;
                            Palet.NumeroCajas = palet.NumeroCajas;
                            Palet.Carga = palet.Carga;
                            Palet.Tipo = palet.Tipo;
                            Palet.Perfilar = false;
                            if (EditTG)
                            {
                                if(valDTG == null)
                                {
                                    Palet.DetailTransportGuide = DetailTG;
                                }
                                else
                                {
                                    Palet.DetailTransportGuide = valDTG;
                                }
                            }
                            else
                            {
                                Palet.DetailTransportGuide = DetailTG;
                            }
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
                return ex.Message;
            }
            return "Ejecutado correctamente";
        }

        public bool ExistPoma(string placa)
        {
            return _dataContext.Pomas.Any(x => x.Placa.ToLower() == placa.ToLower());
        }

        public TransportGuide ExistTransportGuide(int numero)
        {
            var TG =  _dataContext.TransportGuides.Where(x => x.Numero == numero).FirstOrDefault();
            return TG;
        }

        public DetailTransportGuide ExistDetailTransportGuide(int TG, int F)
        {
            var DTG = _dataContext.DetailTransportGuide.Where(x => x.TransportGuide.ID == TG && x.Fruta.ID == F).FirstOrDefault();
            return DTG;
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
