using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExportadorController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public ExportadorController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetExportador()
        {
            try
            {
                var Exportador = _dataContext.Exportadores.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.ExportadorName,
                    x.Estado
                }).ToList();
                return Ok(new { Data = Exportador, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateExportador([FromBody]List<Exportadores> Exportadores)
        {
            List<Exportadores> ListExportador = new List<Exportadores>();

            try
            {
                ListExportador = await Create(Exportadores);
                return Ok(new
                {
                    Data = ListExportador.Select(x => new
                    {
                        x.ID,
                        x.Codigo,
                        x.ExportadorName,
                        x.Estado
                    }).ToList(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Exportadores>> Create(List<Exportadores> Exportadores)
        {
            List<Exportadores> ListExportadores = new List<Exportadores>();
            foreach (var Exportador in Exportadores)
            {
                if (!this.ExistExportador(Exportador.Codigo))
                {
                    Exportadores exportador = new Exportadores();
                    exportador.Codigo = Exportador.Codigo;
                    exportador.ExportadorName = Exportador.ExportadorName;
                    exportador.Estado = true;
                    ListExportadores.Add(exportador);
                }
            }
            _dataContext.Exportadores.AddRange(ListExportadores);
            await _dataContext.SaveChangesAsync();
            return ListExportadores;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditExportador(Exportadores Exportadores)
        {
            if (_dataContext.Exportadores.Any(x => x.Codigo == Exportadores.Codigo))
            {
                var exportadores = _dataContext.Exportadores.First(x => x.Codigo == Exportadores.Codigo);
                exportadores.ExportadorName = Exportadores.ExportadorName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            exportadores.ID,
                            exportadores.Codigo,
                            exportadores.ExportadorName,
                            exportadores.Estado
                        },
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "El exportador con codigo " + Exportadores.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteExportador(Exportadores Exportadores)
        {
            if (_dataContext.Exportadores.Any(x => x.Codigo == Exportadores.Codigo))
            {
                var exportadores = _dataContext.Exportadores.First(x => x.Codigo == Exportadores.Codigo);
                exportadores.Estado = !exportadores.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            exportadores.ID,
                            exportadores.Codigo,
                            exportadores.ExportadorName,
                            exportadores.Estado
                        },
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "El exportador con codigo " + Exportadores.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistExportador(int id)
        {
            return _dataContext.Exportadores.Any(x => x.Codigo == id);
        }
    }
}
