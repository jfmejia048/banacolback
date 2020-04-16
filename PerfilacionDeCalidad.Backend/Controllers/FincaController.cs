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
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FincaController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public FincaController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetFincas()
        {
            try
            {
                var Fincas = _dataContext.Fincas.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.FincaName
                }).ToList();
                return Ok(new { Data = Fincas, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateFinca(Fincas Finca)
        {
            List<Fincas> ListFinca = new List<Fincas>();
            try
            {
                Fincas F = await Create(Finca);
                _dataContext.Fincas.AddRange(ListFinca);
                return Ok(new { Data = F, Success = true });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Fincas> Create(Fincas Finca)
        {
            if (!this.ExistFinca(Finca.Codigo))
            {
                _dataContext.Fincas.Add(Finca);
            }
            else
            {
                return _dataContext.Fincas.FirstOrDefault(x => x.Codigo == Finca.Codigo);
            }
            await _dataContext.SaveChangesAsync();
            return Finca;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditFinca(Fincas Finca)
        {
            if (_dataContext.Fincas.Any(x => x.Codigo == Finca.Codigo))
            {
                var finca = _dataContext.Fincas.First(x => x.Codigo == Finca.Codigo);
                finca.FincaName = Finca.FincaName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        finca.ID,
                        finca.Codigo,
                        finca.FincaName
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "La finca con codigo " + Finca.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistFinca(int id)
        {
            return _dataContext.Fincas.Any(x => x.Codigo == id);
        }
    }
}
