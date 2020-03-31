using Common.DTO;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Data.Entities;
using PerfilacionDeCalidad.Backend.Enum;
using PerfilacionDeCalidad.Backend.Helpers;
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
                var Pomas = (from Poma in _dataContext.Pomas
                             join Finca in _dataContext.Fincas on Poma.Codigo equals Finca.Pomas.Codigo
                             join Fruta in _dataContext.Frutas on Finca.Frutas.ID equals Fruta.ID
                             join Palets in _dataContext.Palets on Finca.Codigo equals Palets.Finca.Codigo
                             join Puerto in _dataContext.Puertos on Palets.Puerto.Codigo equals Puerto.Codigo
                             join Buque in _dataContext.Buques on Palets.Buque.Codigo equals Buque.Codigo
                             join Exportador in _dataContext.Exportadores on Palets.Exportador.Codigo equals Exportador.Codigo
                             join Destino in _dataContext.Destinos on Palets.Destino.Codigo equals Destino.Codigo
                             select new
                             {
                                 CodigoPalet = Palets.Codigo,
                                 Finca = Finca.FincaName,
                                 TerminalDestino = Puerto.PuertoName,
                                 Poma = Poma.Numero,
                                 EstadoPoma = EnumHelper.GetEnumDescription((EstadosPoma)Poma.Estado),
                                 FechaCreacion = Poma.FechaRegistro.ToLocalTime(),
                                 Fruta = Fruta.FrutaName,
                                 Buque = Buque.BuqueName,
                                 Llegada = Palets.LlegadaTerminal,
                                 Salida = Palets.SalidaFinca,
                                 Estimado = Palets.Estimado,
                                 LlegadaTerminal = Palets.LlegadaTerminal,
                                 Cajas = Poma.Recibido,
                                 Exportador = Exportador.ExportadorName,
                                 Destino = Destino.DestinoName,
                                 Carga = Palets.Carga,
                                 CodigoDeBarras = Palets.CodigoPalet,
                                 idPallet = Palets.ID,
                                 CajasPalet = Palets.NumeroCajas,
                                 Palets.Perfilar
                             }).OrderBy(x => x.FechaCreacion).ToList();

                var List = Pomas.GroupBy(x => new { x.Finca, x.TerminalDestino, x.Poma, x.FechaCreacion })
                    .Select(x => new {
                        x.FirstOrDefault().Finca,
                        x.FirstOrDefault().TerminalDestino,
                        x.FirstOrDefault().Poma,
                        x.FirstOrDefault().FechaCreacion,
                        x.FirstOrDefault().EstadoPoma,
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
                                s3.idPallet,
                                s3.CodigoPalet,
                                s3.Carga,
                                s3.CodigoDeBarras,
                                s3.CajasPalet,
                                s3.Perfilar
                            })
                        }).ToList()
                    });

                if (!string.IsNullOrEmpty(parameters.Finca))
                {
                    List = List.Where(x => x.Finca.ToLower().Contains(parameters.Finca.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Puerto))
                {
                    List = List.Where(x => x.TerminalDestino.ToLower().Contains(parameters.Puerto.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Buque))
                {
                    List = List.Where(x => x.Frutas.Any(w => w.Buque.ToLower().Contains(parameters.Buque.ToLower()))).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Destino))
                {
                    List = List.Where(x => x.Frutas.Any(w => w.Destino.ToLower().Contains(parameters.Destino.ToLower()))).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Exportador))
                {
                    List = List.Where(x => x.Frutas.Any(w => w.Exportador.ToLower().Contains(parameters.Exportador.ToLower()))).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Fruta))
                {
                    List = List.Where(x => x.Frutas.Any(w => w.Fruta.ToLower().Contains(parameters.Fruta.ToLower()))).ToList();
                }

                if (!string.IsNullOrEmpty(parameters.Poma))
                {
                    List = List.Where(x => x.Poma.ToString().ToLower().Contains(parameters.Poma.ToLower())).ToList();
                }

                if (parameters.RangoFechas[0].ToLocalTime() != null && parameters.RangoFechas[1].ToLocalTime() != null)
                {
                    List = List.Where(x => x.FechaCreacion >= parameters.RangoFechas[0].ToLocalTime() && x.FechaCreacion <= parameters.RangoFechas[1].ToLocalTime()).ToList();
                }

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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Where(x => x.Palet.Codigo == Palet.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Puerto).Where(x => x.Palet.Puerto.Codigo == Puerto.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Buque).Where(x => x.Palet.Buque.Codigo == Buque.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Destino).Where(x => x.Palet.Destino.Codigo == Destino.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Exportador).Where(x => x.Palet.Exportador.Codigo == Exportador.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Finca.Frutas).Where(x => x.Palet.Finca.Frutas.Codigo == Fruta.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                var Trackings = _dataContext.Tracking.Include(x => x.Palet).Include(x => x.Palet.Finca.Pomas).Where(x => x.Palet.Finca.Pomas.Codigo == Poma.ID).Select(x => new
                {
                    ID = x.Codigo,
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
                    ID = x.Codigo,
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
                var Palet = _dataContext.Palets.FirstOrDefault(x => x.Codigo == Tracking.Palet.ID);
                if (Palet != null)
                {
                    if (!this.ExistsTracking(Tracking.ID))
                    {
                        Tracking tracking = new Tracking();
                        tracking.Codigo = Tracking.ID;
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
