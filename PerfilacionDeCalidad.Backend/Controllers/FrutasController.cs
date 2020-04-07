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
                    x.FrutaName,
                    x.Estado,
                    x.Poma
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
        public async Task<IActionResult> CreateFruta([FromBody]List<Frutas> Frutas)
        {
            List<Frutas> ListFruta = new List<Frutas>();

            try
            {
                ListFruta = await Create(Frutas);
                return Ok(new { Data = ListFruta.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.FrutaName,
                    x.Estado,
                    x.Poma
                }).ToList(), Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Frutas>> Create(List<Frutas> Frutas)
        {
            List<Frutas> ListFruta = new List<Frutas>();
            foreach (var Fruta in Frutas)
            {
                if (!this.ExistFruta(Fruta.Codigo))
                {
                    Frutas F = new Frutas();
                    F.Poma = Fruta.Poma;
                    F.Codigo = Fruta.Codigo;
                    F.FrutaName = Fruta.FrutaName;
                    F.Estado = true;
                    ListFruta.Add(F);
                }
            }
            _dataContext.Frutas.AddRange(ListFruta);
            await _dataContext.SaveChangesAsync();
            return ListFruta;
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
                        fruta.FrutaName,
                        fruta.Estado
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

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteFinca(Frutas Fruta)
        {
            if (_dataContext.Frutas.Any(x => x.Codigo == Fruta.Codigo))
            {
                var fruta = _dataContext.Frutas.First(x => x.Codigo == Fruta.Codigo);
                fruta.Estado = !fruta.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        fruta.ID,
                        fruta.Codigo,
                        fruta.FrutaName,
                        fruta.Estado
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
