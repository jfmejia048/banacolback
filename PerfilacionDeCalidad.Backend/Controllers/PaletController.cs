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
    public class PaletController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PaletController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }


        [HttpPost]
        [Route("Get")]
        public IActionResult GetPomas()
        {
            try
            {

                var Pomas = (from Poma in _dataContext.Pomas
                             join Caja in _dataContext.Cajas on Poma.Codigo equals Caja.Pomas.Codigo
                             join Fruta in _dataContext.Frutas on Caja.Frutas.ID equals Fruta.ID
                             join Palet in _dataContext.Palets on Caja.Codigo equals Palet.Caja.Codigo
                             join Finca in _dataContext.Fincas on Palet.Finca.Codigo equals Finca.Codigo
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
                                 Cajas = Caja.Estado,
                                 Exportador = Exportador.ExportadorName,
                                 Destino = Destino.DestinoName,
                                 Carga = Palet.Carga,
                                 CodigoDeBarras = Palet.CodigoPalet,
                                 CajasPalet = Palet.NumeroCajas,
                                 Palet.Perfilar
                             }).ToList();
                return Ok(new { Data = Pomas, Success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }
    }
}
