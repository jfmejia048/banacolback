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
    public class BuqueController : ControllerBase
    {

        private readonly DataContext _dataContext;

        public BuqueController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetBuques()
        {
            try
            {
                var Buques = _dataContext.Buques.Select(x => new
                {
                    x.ID,
                    x.Codigo,
                    x.BuqueName
                }).ToList();
                var Fincas = _dataContext.Buques.ToList();
                return Ok(new { Data = Buques, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateBuque(Buques Buque)
        {
            List<Buques> ListBuque = new List<Buques>();
            try
            {

                Buques B = await this.Create(Buque);
                return Ok(new { Data = B, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<Buques> Create(Buques Buque)
        {
            if (!this.ExistBuque(Buque.Codigo))
            {
                _dataContext.Buques.Add(Buque);
            }
            else
            {
                return _dataContext.Buques.FirstOrDefault(x => x.Codigo == Buque.Codigo);
            }
            await _dataContext.SaveChangesAsync();
            return Buque;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditBuque(Buques Buque)
        {
            if (_dataContext.Buques.Any(x => x.Codigo == Buque.Codigo))
            {
                var Buques = _dataContext.Buques.First(x => x.Codigo == Buque.Codigo);
                Buques.BuqueName = Buque.BuqueName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        Buque.ID,
                        Buques.Codigo,
                        Buques.BuqueName
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "El buque con codigo " + Buque.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }
        
        public bool ExistBuque(int id)
        {
            return _dataContext.Buques.Any(x => x.Codigo == id);
        }
    }
}
