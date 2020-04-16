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
                    x.ExportadorName
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
        public async Task<IActionResult> CreateExportador(Exportadores Exportador)
        {
            List<Exportadores> ListExportador = new List<Exportadores>();

            try
            {
                Exportadores E = await Create(Exportador);
                return Ok(new { Data = E, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Exportadores> Create(Exportadores Exportador)
        {
            if (!this.ExistExportador(Exportador.Codigo))
            {
                _dataContext.Exportadores.Add(Exportador);
            }
            else
            {
                return _dataContext.Exportadores.FirstOrDefault(x => x.Codigo == Exportador.Codigo);
            }
            await _dataContext.SaveChangesAsync();
            return Exportador;
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
                            exportadores.ExportadorName
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
