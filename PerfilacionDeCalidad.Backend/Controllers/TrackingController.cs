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
                        palet.Perfilar = false;
                        var tracking = new Tracking
                        {
                            Palet = palet,
                            RegisterDate = DateTime.UtcNow,
                            Localizacion = "",
                            Punto = "",
                            Evento = "Selección Pallets"
                        };
                        _dataContext.Tracking.Add(tracking);
                        _dataContext.SaveChanges();
                    });

                    var tg = _dataContext.TransportGuides.Where(w => w.ID == palets.IdTransportGuide).FirstOrDefault();
                    if(palets.Action == 0 || palets.Action == 1)
                    {
                        tg.Estado = (int)EstadosPoma.Chequeado;
                    }else if(palets.Action == 2)
                    {
                        tg.Estado = (int)EstadosPoma.Perfilado;
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
                    CodigoPalet = x.Palet.CodigoPalet,
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
                var Palet = _dataContext.Palets.FirstOrDefault(x => x.CodigoPalet == Tracking.Palet.CodigoPalet);
                if (Palet != null)
                {
                    Tracking tracking = new Tracking();
                    tracking.ID = Tracking.ID;
                    tracking.RegisterDate = Tracking.RegisterDate;
                    tracking.Palet = Palet;
                    tracking.Punto = Tracking.Punto;
                    tracking.Localizacion = Tracking.Localizacion;
                    tracking.Evento = Tracking.Evento;
                    ListTracking.Add(tracking);
                }
            }
            _dataContext.Tracking.AddRange(ListTracking);
            await _dataContext.SaveChangesAsync();
            return ListTracking;
        }
    }
}
