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
                    ID = x.Codigo,
                    FincaName = x.FincaName,
                    Estado = x.Estado
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
        public async Task<IActionResult> CreateFinca(List<Fincas> Fincas)
        {
            List<Fincas> ListFinca = new List<Fincas>();
            try
            {
                ListFinca = await Create(Fincas);
                _dataContext.Fincas.AddRange(ListFinca);
                await _dataContext.SaveChangesAsync();
                return Ok(new { Data = ListFinca.Select(x => new
                {
                    ID = x.Codigo,
                    FincaName = x.FincaName,
                    Estado = x.Estado
                }).ToList(), Success = true });
            }
            catch(Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Fincas>> Create(List<Fincas> Fincas)
        {
            List<Fincas> ListFinca = new List<Fincas>();
            foreach (var Finca in Fincas)
            {
                if (!this.ExistFinca(Finca.ID))
                {
                    Fincas finca = new Fincas();
                    finca.Codigo = Finca.ID;
                    finca.FincaName = Finca.FincaName;
                    finca.Estado = true;
                    ListFinca.Add(finca);
                }
            }
            _dataContext.Fincas.AddRange(ListFinca);
            await _dataContext.SaveChangesAsync();
            return ListFinca;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditFinca(Fincas Finca)
        {
            if (_dataContext.Fincas.Any(x => x.Codigo == Finca.ID))
            {
                var finca = _dataContext.Fincas.First(x => x.Codigo == Finca.ID);
                finca.FincaName = Finca.FincaName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        ID = finca.Codigo,
                        FincaName = finca.FincaName,
                        Estado = finca.Estado
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "La finca con codigo " + Finca.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteFinca(Fincas Finca)
        {
            if (_dataContext.Fincas.Any(x => x.Codigo == Finca.ID))
            {
                var finca = _dataContext.Fincas.First(x => x.Codigo == Finca.ID);
                finca.Estado = !finca.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        ID = finca.Codigo,
                        FincaName = finca.FincaName,
                        Estado = finca.Estado
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "La finca con codigo " + Finca.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistFinca(int id)
        {
            return _dataContext.Fincas.Any(x => x.Codigo == id);
        }
    }
}
