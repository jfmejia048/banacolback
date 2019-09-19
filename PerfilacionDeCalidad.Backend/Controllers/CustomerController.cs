using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PerfilacionDeCalidad.Backend.Data;
using System.Linq;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin",AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CustomerController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public CustomerController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        [Route("GetCustomers")]
        public IActionResult GetCustomers()
        {
            var Customers = _dataContext.Users.Include(x => x.TypeUser).Where(x => x.TypeUser.Type == "Customer").Select(x => new
            {
                ID = x.Id,
                Documento = x.Document,
                Nombre = x.FirstName,
                Apellido = x.LastName,
                NombreCompleto = x.FullName,
                Telefono = x.PhoneNumber,
                Direccion = x.Address,
                Email = x.UserName,
                TipoUsuario = x.TypeUser.Type
            }).ToList();
            return Ok(new { Data = Customers, Success = true});
        }
    }
}
