using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Logic;
using PerfilacionDeCalidad.Backend.Models;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Administrador,Usuario", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExportController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IHostingEnvironment _host;

        public ExportController(DataContext dataContext, IHostingEnvironment host)
        {
            _dataContext = dataContext;
            _host = host;
        }

        [HttpPost]
        [Route("ExportFile")]
        public IActionResult ExportFile(Parameters parameters)
        {
            try
            {
                ExportLogic exportLogic = new ExportLogic(this._dataContext, this._host);
                var file = exportLogic.ExportExcel(parameters);

                return File(file, "application/vnd.ms-excel", "ExportPallets.xlsx");
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }
    }
}