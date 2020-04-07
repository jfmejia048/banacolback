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
                    x.DestinoName,
                    x.Estado
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
        public async Task<IActionResult> CreatePuerto([FromBody]List<Destinos> Destino)
        {
            List<Destinos> ListDestino = new List<Destinos>();

            try
            {
                ListDestino = await Create(Destino);
                return Ok(new
                {
                    Data = ListDestino.Select(x => new
                    {
                        x.ID,
                        x.Codigo,
                        x.DestinoName,
                        x.Estado
                    }).ToList(),
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public async Task<List<Destinos>> Create(List<Destinos> Destinos)
        {
            List<Destinos> ListDestino = new List<Destinos>();
            foreach (var Destino in Destinos)
            {
                if (!this.ExistDestino(Destino.Codigo))
                {
                    Destinos destino = new Destinos();
                    destino.Codigo = Destino.Codigo;
                    destino.DestinoName = Destino.DestinoName;
                    destino.Estado = true;
                    ListDestino.Add(destino);
                }
            }
            _dataContext.Destinos.AddRange(ListDestino);
            await _dataContext.SaveChangesAsync();
            return ListDestino;
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
                            destinos.DestinoName,
                            destinos.Estado
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

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> DeleteDestino(Destinos Destinos)
        {
            if (_dataContext.Destinos.Any(x => x.Codigo == Destinos.Codigo))
            {
                var Destino = _dataContext.Destinos.First(x => x.Codigo == Destinos.Codigo);
                Destino.Estado = !Destino.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            Destino.ID,
                            Destino.Codigo,
                            Destino.DestinoName,
                            Destino.Estado
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
                return BadRequest(new { Data = "el destino con codigo " + Destinos.Codigo + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistDestino(int id)
        {
            return _dataContext.Destinos.Any(x => x.Codigo == id);
        }
    }
}
