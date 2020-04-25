using Common.DTO;
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
    [Authorize(Roles = "Administrador,Usuario,Calidad,Chequeo", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
            var Pomas = (from TransportGuides in _dataContext.TransportGuides
                         join DetailTransportGuide in _dataContext.DetailTransportGuide on TransportGuides.ID equals DetailTransportGuide.TransportGuide.ID
                         join Pallets in _dataContext.Palets on DetailTransportGuide.ID equals Pallets.DetailTransportGuide.ID
                         select new
                         {
                             IdPallet = Pallets.ID,
                             Finca = TransportGuides.Finca.FincaName,
                             TerminalDestino = TransportGuides.Puerto.PuertoName,
                             Poma = TransportGuides.Numero,
                             Fruta = DetailTransportGuide.Fruta.FrutaName,
                             Buque = TransportGuides.Buque.BuqueName,
                             Llegada = TransportGuides.LlegadaTerminal,
                             Salida = TransportGuides.SalidaFinca,
                             Estimado = TransportGuides.Estimado,
                             LlegadaTerminal = TransportGuides.LlegadaTerminal,
                             Cajas = TransportGuides.Recibido,
                             Exportador = TransportGuides.Exportador.ExportadorName,
                             Destino = TransportGuides.Destino.DestinoName,
                             Carga = Pallets.Carga,
                             CodigoDeBarras = Pallets.CodigoPalet,
                             CajasPalet = Pallets.NumeroCajas,
                             Pallets.Perfilar,
                             CaraPallet = Pallets.CaraPalet,
                             IdTransportGuide = TransportGuides.ID
                         }).ToList();
            return Pomas;
        }

        [HttpPost]
        [Route("GetByCodigo")]
        public IActionResult GetPaletByCodigo(Pallets Codigo)
        {
            try
            {
                var pallet = _dataContext.Palets.Where(x => x.CodigoPalet == Codigo.CodigoPalet).FirstOrDefault();
                pallet.LecturaPalet = DateTime.UtcNow;
                var Palets = (from TransportGuides in _dataContext.TransportGuides
                              join DetailTransportGuide in _dataContext.DetailTransportGuide on TransportGuides.ID equals DetailTransportGuide.TransportGuide.ID
                              join Pallets in _dataContext.Palets on DetailTransportGuide.ID equals Pallets.DetailTransportGuide.ID
                              where Pallets.CodigoPalet == Codigo.CodigoPalet
                             select new
                             {
                                 IdPallet  = Pallets.ID,
                                 Finca = TransportGuides.Finca.FincaName,
                                 TerminalDestino = TransportGuides.Puerto.PuertoName,
                                 Poma = TransportGuides.Numero,
                                 Fruta = DetailTransportGuide.Fruta.FrutaName,
                                 Buque = TransportGuides.Buque.BuqueName,
                                 Llegada = TransportGuides.LlegadaCamion,
                                 Salida = TransportGuides.SalidaFinca,
                                 Estimado = TransportGuides.Estimado,
                                 LlegadaTerminal = TransportGuides.LlegadaTerminal,
                                 Cajas = TransportGuides.Recibido,
                                 Exportador = TransportGuides.Exportador.ExportadorName,
                                 Destino = TransportGuides.Destino.DestinoName,
                                 Carga = Pallets.Carga,
                                 CodigoDeBarras = Pallets.CodigoPalet,
                                 CajasPalet = Pallets.NumeroCajas,
                                 Pallets.Perfilar,
                                 CaraPallet = Pallets.CaraPalet,
                                 IdTransportGuide = TransportGuides.ID
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
        public IActionResult Perfilar(GuardarCaraPallet pallet)
        {
            try
            {
                var Palet = _dataContext.Palets.Where(x => x.ID == pallet.Id).FirstOrDefault();
                if(Palet != null)
                {
                    Palet.CaraPalet = pallet.Cara;
                    Palet.UsuarioInspeccion = pallet.Usuario;
                    Palet.InspeccionPalet = DateTime.UtcNow;
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
