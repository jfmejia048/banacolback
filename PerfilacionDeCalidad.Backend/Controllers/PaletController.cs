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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PaletController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PaletController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        [HttpPost]
        [Route("Get")]
        public IActionResult GetPalet()
        {
            try
            {
                var Pomas = Get();
                return Ok(new { Data = Pomas, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        public object Get()
        {
            var Pomas = (from Poma in _dataContext.Pomas
                         join Finca in _dataContext.Fincas on Poma.Codigo equals Finca.Pomas.Codigo
                         join Fruta in _dataContext.Frutas on Poma.Codigo equals Fruta.Poma.Codigo
                         join Palet in _dataContext.Palets on Fruta.Codigo equals Palet.Fruta.Codigo
                         join Puerto in _dataContext.Puertos on Palet.Puerto.Codigo equals Puerto.Codigo
                         join Buque in _dataContext.Buques on Palet.Buque.Codigo equals Buque.Codigo
                         join Exportador in _dataContext.Exportadores on Palet.Exportador.Codigo equals Exportador.Codigo
                         join Destino in _dataContext.Destinos on Palet.Destino.Codigo equals Destino.Codigo
                         select new
                         {
                             Finca = Finca.FincaName,
                             TerminalDestino = Puerto.PuertoName,
                             Poma = Poma.Numero,
                             Fruta = Fruta.FrutaName,
                             Buque = Buque.BuqueName,
                             Llegada = Palet.LlegadaTerminal,
                             Salida = Palet.SalidaFinca,
                             Estimado = Palet.Estimado,
                             LlegadaTerminal = Palet.LlegadaTerminal,
                             Cajas = Poma.Recibido,
                             Exportador = Exportador.ExportadorName,
                             Destino = Destino.DestinoName,
                             Carga = Palet.Carga,
                             CodigoDeBarras = Palet.CodigoPalet,
                             CajasPalet = Palet.NumeroCajas,
                             Palet.Perfilar
                         }).ToList();
            return Pomas;
        }

        [HttpPost]
        [Route("GetByCodigo")]
        public IActionResult GetPaletByCodigo(Palets Codigo)
        {
            try
            {
                var Palets = (from Poma in _dataContext.Pomas
                              join Finca in _dataContext.Fincas on Poma.Codigo equals Finca.Pomas.Codigo
                              join Fruta in _dataContext.Frutas on Poma.Codigo equals Fruta.Poma.Codigo
                              join Palet in _dataContext.Palets on Fruta.Codigo equals Palet.Fruta.Codigo
                              join Puerto in _dataContext.Puertos on Palet.Puerto.Codigo equals Puerto.Codigo
                              join Buque in _dataContext.Buques on Palet.Buque.Codigo equals Buque.Codigo
                              join Exportador in _dataContext.Exportadores on Palet.Exportador.Codigo equals Exportador.Codigo
                              join Destino in _dataContext.Destinos on Palet.Destino.Codigo equals Destino.Codigo
                              where Palet.CodigoPalet == Codigo.CodigoPalet
                             select new
                             {
                                 IdPallet  = Palet.ID,
                                 Finca = Finca.FincaName,
                                 TerminalDestino = Puerto.PuertoName,
                                 Poma = Poma.Numero,
                                 Fruta = Fruta.FrutaName,
                                 Buque = Buque.BuqueName,
                                 Llegada = Palet.LlegadaTerminal,
                                 Salida = Palet.SalidaFinca,
                                 Estimado = Palet.Estimado,
                                 LlegadaTerminal = Palet.LlegadaTerminal,
                                 Cajas = Poma.Recibido,
                                 Exportador = Exportador.ExportadorName,
                                 Destino = Destino.DestinoName,
                                 Carga = Palet.Carga,
                                 CodigoDeBarras = Palet.CodigoPalet,
                                 CajasPalet = Palet.NumeroCajas,
                                 Palet.Perfilar,
                                 CaraPallet = Palet.CaraPalet
                             }).FirstOrDefault();
                return Ok(new { Data = Palets, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        [HttpPost]
        [Route("GuardarCara")]
        public IActionResult Perfilar(Palets pallet)
        {
            try
            {
                var Palet = _dataContext.Palets.Where(x => x.ID == pallet.ID).FirstOrDefault();
                if(Palet != null)
                {
                    Palet.CaraPalet = pallet.CaraPalet;
                    _dataContext.SaveChanges();
                    return Ok(new { Data = Palet, success = true });
                }
                else
                {
                    return BadRequest(new { Data = "Palet not found", success = false });
                }
            }
            catch(Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), success = false });
            }
        }
    }
}
