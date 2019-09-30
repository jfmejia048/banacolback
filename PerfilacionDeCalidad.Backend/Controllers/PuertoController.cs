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
                    ID = x.Codigo,
                    PuertoName = x.PuertoName,
                    Estado = x.Estado
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
        public async Task<IActionResult> CreatePuerto([FromBody]List<Puertos> Puertos)
        {
            List<Puertos> ListPuerto = new List<Puertos>();

            try
            {
                ListPuerto = await Create(Puertos);
                return Ok(new
                {
                    Data = ListPuerto.Select(x => new
                    {
                        ID = x.Codigo,
                        PuertoName = x.PuertoName,
                        Estado = x.Estado
                    }).ToList(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Puertos>> Create(List<Puertos> Puertos)
        {
            List<Puertos> ListPuerto = new List<Puertos>();
            foreach (var Puerto in Puertos)
            {
                if (!this.ExistPuerto(Puerto.ID))
                {
                    Puertos puerto = new Puertos();
                    puerto.Codigo = Puerto.ID;
                    puerto.PuertoName = Puerto.PuertoName;
                    puerto.Estado = true;
                    ListPuerto.Add(puerto);
                }
            }
            _dataContext.Puertos.AddRange(ListPuerto);
            await _dataContext.SaveChangesAsync();
            return ListPuerto;
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditPuerto(Puertos Puertos)
        {
            if (_dataContext.Puertos.Any(x => x.Codigo == Puertos.ID))
            {
                var Puerto = _dataContext.Puertos.First(x => x.Codigo == Puertos.ID);
                Puerto.PuertoName = Puertos.PuertoName;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            ID = Puerto.Codigo,
                            PuertoName = Puerto.PuertoName,
                            Estado = Puerto.Estado
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
                return BadRequest(new { Data = "El puerto con codigo " + Puertos.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeletePuerto(Puertos Puertos)
        {
            if (_dataContext.Puertos.Any(x => x.Codigo == Puertos.ID))
            {
                var Puerto = _dataContext.Puertos.First(x => x.Codigo == Puertos.ID);
                Puerto.Estado = !Puerto.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            ID = Puerto.Codigo,
                            PuertoName = Puerto.PuertoName,
                            Estado = Puerto.Estado
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
                return BadRequest(new { Data = "El Puerto con codigo " + Puertos.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }


        public bool ExistPuerto(int id)
        {
            return _dataContext.Puertos.Any(x => x.Codigo == id);
        }
    }
}
