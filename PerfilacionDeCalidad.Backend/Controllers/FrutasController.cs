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
    public class FrutasController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public FrutasController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetFrutas()
        {
            try
            {
                var Frutas = _dataContext.Frutas.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.FrutaName
                }).ToList();
                return Ok(new { Data = Frutas, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateFruta(Frutas Fruta)
        {
            try
            {
                Frutas F = await Create(Fruta);
                return Ok(new { Data = F, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Frutas> Create(Frutas Fruta)
        {
            if (!this.ExistFruta(Fruta.Codigo))
            {
                _dataContext.Frutas.Add(Fruta);
            }
            else
            {
                return _dataContext.Frutas.FirstOrDefault(x => x.Codigo == Fruta.Codigo);
            }
            await _dataContext.SaveChangesAsync();
            return Fruta;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditFruta(Frutas Fruta)
        {
            if (_dataContext.Frutas.Any(x => x.Codigo == Fruta.Codigo))
            {
                var fruta = _dataContext.Frutas.First(x => x.Codigo == Fruta.Codigo);
                fruta.FrutaName = Fruta.FrutaName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        fruta.ID,
                        fruta.Codigo,
                        fruta.FrutaName
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "La finca con codigo " + Fruta.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistFruta(int id)
        {
            return _dataContext.Frutas.Any(x => x.Codigo == id);
        }
    }
}
