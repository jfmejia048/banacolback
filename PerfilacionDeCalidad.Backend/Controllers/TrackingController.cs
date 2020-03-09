using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Data.Entities;
using PerfilacionDeCalidad.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet)
                    .Include(x => x.Palet.Finca)
                    .Include(x => x.Palet.Puerto)
                    .Include(x => x.Palet.Buque)
                    .Include(x => x.Palet.Destino)
                    .Include(x => x.Palet.Exportador)
                    .Include(x => x.Palet.Caja)
                    .Include(x => x.Palet.Caja.Pomas)
                    .Include(x => x.Palet.Caja.Frutas).ToList();

                if (!string.IsNullOrEmpty(parameters.Finca))
                {
                    Trackings = Trackings.Where(x => x.Palet.Finca.FincaName.Contains(parameters.Finca)).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Puerto))
                {
                    Trackings = Trackings.Where(x => x.Palet.Puerto.PuertoName.Contains(parameters.Puerto)).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Buque))
                {
                    Trackings = Trackings.Where(x => x.Palet.Buque.BuqueName.Contains(parameters.Buque)).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Destino))
                {
                    Trackings = Trackings.Where(x => x.Palet.Destino.DestinoName.Contains(parameters.Destino)).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Exportador))
                {
                    Trackings = Trackings.Where(x => x.Palet.Exportador.ExportadorName.Contains(parameters.Exportador)).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Fruta))
                {
                    Trackings = Trackings.Where(x => x.Palet.Caja.Frutas.FrutaName.Contains(parameters.Fruta)).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Poma))
                {
                    Trackings = Trackings.Where(x => x.Palet.Caja.Pomas.Placa.Contains(parameters.Poma)).ToList();
                }

                return Ok(new { Data = Trackings.GroupBy(x => x.Palet).Select(x => new { x.FirstOrDefault().Palet, Tracking = x.ToList() }), Success = true });
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Where(x => x.Palet.Codigo == Palet.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
        [Route("GetByFinca")]
        public IActionResult GetByFinca(Fincas Finca)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Finca).Where(x => x.Palet.Finca.Codigo == Finca.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
        [Route("GetByPuerto")]
        public IActionResult GetByPuerto(Puertos Puerto)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Puerto).Where(x => x.Palet.Puerto.Codigo == Puerto.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Buque).Where(x => x.Palet.Buque.Codigo == Buque.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Destino).Where(x => x.Palet.Destino.Codigo == Destino.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Exportador).Where(x => x.Palet.Exportador.Codigo == Exportador.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
        [Route("GetByCaja")]
        public IActionResult GetByCaja(Cajas Caja)
        {
            try
            {
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Caja).Where(x => x.Palet.Caja.Codigo == Caja.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Caja).Include(x => x.Palet.Caja.Frutas).Where(x => x.Palet.Caja.Frutas.Codigo == Fruta.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Caja).Include(x => x.Palet.Caja.Pomas).Where(x => x.Palet.Caja.Pomas.Codigo == Poma.ID).Select(x => new
                {
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                    ID = x.Codigo,
                    CodigoPalet = x.Palet.Codigo,
                    x.LecturaPalet,
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
                var Palet = _dataContext.Palets.FirstOrDefault(x => x.Codigo == Tracking.Palet.ID);
                if (Palet != null)
                {
                    if (!this.ExistsTracking(Tracking.ID))
                    {
                        Tracking tracking = new Tracking();
                        tracking.Codigo = Tracking.ID;
                        tracking.LecturaPalet = Tracking.LecturaPalet;
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
