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
                    ID = x.Codigo,
                    BuqueName = x.BuqueName,
                    Estado = x.Estado
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
        public async Task<IActionResult> CreateBuque(List<Buques> Buques)
        {
            List<Buques> ListBuque = new List<Buques>();
            try
            {

                ListBuque = await this.Create(Buques);
                var Buque = ListBuque.Select(x => new
                {
                    ID = x.Codigo,
                    BuqueName = x.BuqueName,
                    Estado = x.Estado
                }).ToList();
                return Ok(new { Data = Buque, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Buques>> Create(List<Buques> Buques)
        {
            List<Buques> ListBuque = new List<Buques>();
            foreach (var Buque in Buques)
            {
                if (!this.ExistBuque(Buque.ID))
                {
                    Buques buques = new Buques();
                    buques.Codigo = Buque.ID;
                    buques.BuqueName = Buque.BuqueName;
                    buques.Estado = true;
                    ListBuque.Add(buques);
                }
            }
            _dataContext.Buques.AddRange(ListBuque);
            await _dataContext.SaveChangesAsync();
            return ListBuque;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditBuque(Buques Buque)
        {
            if (_dataContext.Buques.Any(x => x.Codigo == Buque.ID))
            {
                var Buques = _dataContext.Buques.First(x => x.Codigo == Buque.ID);
                Buques.BuqueName = Buque.BuqueName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        ID = Buques.Codigo,
                        BuqueName = Buques.BuqueName,
                        Estado = Buques.Estado
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "El buque con codigo " + Buque.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteBuque(Buques Buque)
        { 
            if (_dataContext.Buques.Any(x => x.Codigo == Buque.ID))
            {
                var buque = _dataContext.Buques.First(x => x.Codigo == Buque.ID);
                buque.Estado = !buque.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = new
                    {
                        ID = buque.Codigo,
                        BuqueName = buque.BuqueName,
                        Estado = buque.Estado
                    }, Success = true });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = ex.ToString(), Success = false });
                }
            }
            else
            {
                return BadRequest(new { Data = "El buque con codigo " + Buque.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistBuque(int id)
        {
            return _dataContext.Buques.Any(x => x.Codigo == id);
        }
    }
}
