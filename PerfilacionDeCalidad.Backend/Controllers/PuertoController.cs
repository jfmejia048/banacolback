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
    public class PuertoController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PuertoController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetPuertos()
        {
            try
            {
                var Puertos = _dataContext.Puertos.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.PuertoName
                }).ToList();
                return Ok(new { Data = Puertos, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreatePuerto(Puertos Puerto)
        {
            List<Puertos> ListPuerto = new List<Puertos>();

            try
            {
                Puertos P = await Create(Puerto);
                return Ok(new { Data = P, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Puertos> Create(Puertos Puerto)
        {
            if (!this.ExistPuerto(Puerto.Codigo))
            {
                _dataContext.Puertos.Add(Puerto);
            }
            else
            {
                return _dataContext.Puertos.FirstOrDefault(x => x.Codigo == Puerto.Codigo);
            }
            
            await _dataContext.SaveChangesAsync();
            return Puerto;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditPuerto(Puertos Puertos)
        {
            if (_dataContext.Puertos.Any(x => x.Codigo == Puertos.Codigo))
            {
                var Puerto = _dataContext.Puertos.First(x => x.Codigo == Puertos.Codigo);
                Puerto.PuertoName = Puertos.PuertoName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            Puerto.ID,
                            Puerto.Codigo,
                            Puerto.PuertoName
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
                return BadRequest(new { Data = "El puerto con codigo " + Puertos.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistPuerto(int id)
        {
            return _dataContext.Puertos.Any(x => x.Codigo == id);
        }
    }
}
