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
    public class DestinoController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public DestinoController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetDestino()
        {
            try
            {
                var Destino = _dataContext.Destinos.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.DestinoName
                }).ToList();
                return Ok(new { Data = Destino, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreatePuerto(Destinos Destino)
        {
            try
            {
                Destinos D = await Create(Destino);
                return Ok(new { Data = D, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Destinos> Create(Destinos Destino)
        {
            if (!this.ExistDestino(Destino.Codigo))
            {
                _dataContext.Destinos.Add(Destino);
            }
            else
            {
                return _dataContext.Destinos.FirstOrDefault(x => x.Codigo == Destino.Codigo);
            }

            await _dataContext.SaveChangesAsync();
            return Destino;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditDestino(Destinos Destinos)
        {
            try
            {
                var destinos = _dataContext.Destinos.FirstOrDefault(x => x.Codigo == Destinos.Codigo);
                if (destinos != null)
                {
                    destinos.DestinoName = Destinos.DestinoName;
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            destinos.ID,
                            destinos.Codigo,
                            destinos.DestinoName
                        },
                        Success = true
                    });
                }
                else
                {
                    return BadRequest(new { Data = "El registro no existe", Success = false });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public bool ExistDestino(int id)
        {
            return _dataContext.Destinos.Any(x => x.Codigo == id);
        }
    }
}
