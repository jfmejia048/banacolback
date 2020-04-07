using Common.DTO;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Data.Entities;
using PerfilacionDeCalidad.Backend.Enum;
using PerfilacionDeCalidad.Backend.Helpers;
using PerfilacionDeCalidad.Backend.Logic;
using PerfilacionDeCalidad.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Microsoft.AspNetCore.Authorization.Authorize(Roles = "Administrador,Usuario", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TrackingController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public TrackingController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetTracking()
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Select(x => new
                {
                    x.Codigo,
                    x.Palet,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetFilters")]
        public IActionResult GetFilters(Parameters parameters)
        {
            try
            {
                PomasLogic pomasLogic = new PomasLogic(this._dataContext);

                var List = pomasLogic.GetData(parameters.TipoExportar);

                List = pomasLogic.GetFilter(List, parameters);

                return Ok(new { Data = List, Success = true });
            }catch(Exception ex)
            {
                return BadRequest(new { Data = ex.Message, Success = false });
            }
        }

        [HttpPost]
        [Route("GetByPalet")]
        public IActionResult GetByPalet(Palets Palet)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Where(x => x.Palet.Codigo == Palet.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }


        [HttpPost]
        [Route("SaveTracking")]
        public IActionResult SaveTracking(PerfilarPaletListDTO palets)
        {
            try
            {
                using(TransactionScope tran = new TransactionScope())
                {
                    palets.Perfilar.ForEach(x => {
                        var palet = _dataContext.Palets.Where(w => w.ID == x.idPallet).FirstOrDefault();
                        palet.Perfilar = true;
                        var tracking = new Tracking
                        {
                            Codigo = x.codigoPalet,
                            Palet = palet,
                            RegisterDate = DateTime.UtcNow,
                            Localizacion = "",
                            Punto = "",
                            Evento = "Selección Pallets"
                        };
                        _dataContext.Tracking.Add(tracking);
                        _dataContext.SaveChanges();
                    });

                    palets.NoPerfilar.ForEach(x => {
                        var palet = _dataContext.Palets.Where(w => w.ID == x.idPallet).FirstOrDefault();
                        var tracking = new Tracking
                        {
                            Codigo = x.codigoPalet,
                            Palet = palet,
                            RegisterDate = DateTime.UtcNow,
                            Localizacion = "",
                            Punto = "",
                            Evento = "Selección Pallets"
                        };
                        _dataContext.Tracking.Add(tracking);
                        _dataContext.SaveChanges();
                    });

                    var poma = _dataContext.Pomas.Where(w => w.ID == palets.IdPoma).FirstOrDefault();
                    if(palets.Action == 0 || palets.Action == 1)
                    {
                        poma.Estado = (int)EstadosPoma.Chequeado;
                    }else if(palets.Action == 2)
                    {
                        poma.Estado = (int)EstadosPoma.Perfilado;
                    }
                    _dataContext.SaveChanges();

                    tran.Complete();
                }
                return Ok(new { Data = "Proceso ejecutado correctamente", Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetByPuerto")]
        public IActionResult GetByPuerto(Puertos Puerto)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Puerto).Where(x => x.Palet.Puerto.Codigo == Puerto.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetByBuque")]
        public IActionResult GetByBuque(Buques Buque)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Buque).Where(x => x.Palet.Buque.Codigo == Buque.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetByDestino")]
        public IActionResult GetByDestino(Destinos Destino)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Destino).Where(x => x.Palet.Destino.Codigo == Destino.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetByExportador")]
        public IActionResult GetByExportador(Exportadores Exportador)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Exportador).Where(x => x.Palet.Exportador.Codigo == Exportador.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetByFruta")]
        public IActionResult GetByFruta(Frutas Fruta)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Fruta).Where(x => x.Palet.Fruta.Codigo == Fruta.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GetByPoma")]
        public IActionResult GetByPoma(Pomas Poma)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Fruta.Poma).Where(x => x.Palet.Fruta.Poma.Codigo == Poma.Codigo).Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Trackings, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateTracking([FromBody]List<Tracking> Trackings)
        {
            List<Tracking> ListTracking = new List<Tracking>();
            try
            {
                ListTracking = await this.Create(Trackings);
                var Traking = ListTracking.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.RegisterDate,
                    x.Punto,
                    x.Localizacion,
                    x.Evento
                }).ToList();
                return Ok(new { Data = Traking, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Tracking>> Create(List<Tracking> Trackings)
        {
            List<Tracking> ListTracking = new List<Tracking>();
            foreach (var Tracking in Trackings)
            {
                var Palet = _dataContext.Palets.FirstOrDefault(x => x.Codigo == Tracking.Palet.Codigo);
                if (Palet != null)
                {
                    if (!this.ExistsTracking(Tracking.Codigo))
                    {
                        Tracking tracking = new Tracking();
                        tracking.ID = Tracking.ID;
                        tracking.Codigo = Tracking.Codigo;
                        tracking.RegisterDate = Tracking.RegisterDate;
                        tracking.Palet = Palet;
                        tracking.Punto = Tracking.Punto;
                        tracking.Localizacion = Tracking.Localizacion;
                        tracking.Evento = Tracking.Evento;
                        ListTracking.Add(tracking);
                    }
                }
            }
            _dataContext.Tracking.AddRange(ListTracking);
            await _dataContext.SaveChangesAsync();
            return ListTracking;
        }

        public bool ExistsTracking(int id)
        {
            return _dataContext.Tracking.Any(x => x.Codigo == id);
        }
    }
}
