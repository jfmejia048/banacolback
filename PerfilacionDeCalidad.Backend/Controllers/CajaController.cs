using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class CajaController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CajaController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("Get")]
        public IActionResult GetCajas()
        {
            try
            {
                var Cajas = _dataContext.Cajas.Include(x => x.Frutas).Include(x => x.Pomas).Select(x => new
                {
                    ID = x.Codigo,
                    Fruta = x.Frutas.FrutaName,
                    Placa = x.Pomas.Placa,
                    Cantidad = x.Cantidad,
                    Estado = x.Estado
                }).ToList();
                return Ok(new { Data = Cajas, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreateCaja([FromBody]List<Cajas> Cajas)
        {
            List<Cajas> ListCajas = new List<Cajas>();
            try {
                ListCajas = await this.Create(Cajas);
                return Ok(new
                {
                    Data = ListCajas.Select(x => new
                    {
                        ID = x.Codigo,
                        Fruta = x.Frutas.FrutaName,
                        Placa = x.Pomas.Placa,
                        Cantidad = x.Cantidad,
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

        public async Task<List<Cajas>> Create(List<Cajas> Cajas)
        {
            List<Cajas> ListCajas = new List<Cajas>();
            foreach (var Caja in Cajas)
            {
                if (!this.ExistCaja(Caja.ID))
                {
                    Cajas C = new Cajas();
                    C.Codigo = Caja.ID;
                    C.Frutas = await ValidFrutas(Caja.Frutas);
                    C.Pomas = await ValidPomas(Caja.Pomas);
                    C.Cantidad = Caja.Cantidad;
                    C.Estado = true;
                    ListCajas.Add(C);
                }
            }
            _dataContext.Cajas.AddRange(ListCajas);
            await _dataContext.SaveChangesAsync();
            return ListCajas;
        }
            
        private async Task<Frutas> ValidFrutas(Frutas frutas)
        {
            FrutasController frutasController = new FrutasController(_dataContext);
            if (frutasController.ExistFruta(frutas.ID))
            {
                return frutas;
            }
            else
            {
                List<Frutas> Frutas = new List<Frutas>();
                Frutas.Add(frutas);
                Frutas = await frutasController.Create(Frutas);
                return Frutas.First();
            }
        }

        private async Task<Pomas> ValidPomas(Pomas Pomas)
        {
            PomaController pomaController = new PomaController(_dataContext);
            if (pomaController.ExistPoma(Pomas.Codigo))
            {
                return Pomas;
            }
            else
            {
                List<Pomas> pomas = new List<Pomas>();
                pomas.Add(Pomas);
                pomas = await pomaController.Create(pomas);
                return pomas.First();
            }
        }

        [HttpPost]
        [Route("Edit")]
        public async Task<IActionResult> EditCaja(Cajas Cajas) 
        {
            try
            {
                var cajas = _dataContext.Cajas.FirstOrDefault(x => x.Codigo == Cajas.ID);
                if(cajas != null)
                {
                    cajas.Cantidad = Cajas.Cantidad;
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            ID = cajas.Codigo,
                            Fruta = cajas.Frutas.FrutaName,
                            Placa = cajas.Pomas.Placa,
                            Cantidad = cajas.Cantidad,
                            Estado = cajas.Estado
                        },
                        Success = true
                    });
                }else
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
        public async Task<IActionResult> DeleteCaja(Cajas Cajas)
        {
            if (_dataContext.Cajas.Any(x => x.Codigo == Cajas.ID))
            {
                var Caja = _dataContext.Cajas.First(x => x.Codigo == Cajas.ID);
                Caja.Estado = !Caja.Estado;
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new
                    {
                        Data = new
                        {
                            ID = Caja.Codigo,
                            Fruta = Caja.Frutas.FrutaName,
                            Placa = Caja.Pomas.Placa,
                            Cantidad = Caja.Cantidad,
                            Estado = Caja.Estado
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
                return BadRequest(new { Data = "la caja con codigo " + Cajas.ID + " no se encuentra en la base de datos.", Success = false });
            }
        }

        public bool ExistCaja(int id)
        {
            return _dataContext.Cajas.Any(x => x.Codigo == id);
        }

    }
}
