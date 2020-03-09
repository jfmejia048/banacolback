using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PerfilacionDeCalidad.Backend.Data;
using PerfilacionDeCalidad.Backend.Helpers;
using PerfilacionDeCalidad.Backend.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PerfilacionDeCalidad.Backend.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Cors;
using System.Net.Mail;
using System.Net;
using Common.Models;

namespace PerfilacionDeCalidad.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;

        public AccountController(IUserHelper userHelper, IConfiguration configuration, DataContext dataContext)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _dataContext = dataContext;
        }

        //Login: api/Account/Login, response Token 
        [HttpPost]
        [Route("Login")]

        public async Task<IActionResult> Login(LoginViewModel Model)
        {
            try
            {
                if (string.IsNullOrEmpty(Model.Username))
                    return BadRequest(new { Data = "Username not found", Success = false });

                if (string.IsNullOrEmpty(Model.Password))
                    return BadRequest(new { Data = "Password not found", Success = false });

                var result = await _userHelper.LoginAsync(Model);

                if (result.Succeeded)
                {
                    var Token = await CreateToken(Model);
                    var response = await _dataContext.Users.Include(x => x.TypeUser).Include(x => x.TypeDocument).Where(x => x.UserName == Model.Username).FirstOrDefaultAsync();
                    var data = new
                    {
                        ID = response.Id,
                        Documento = response.Document,
                        Nombre = response.FirstName,
                        Apellido = response.LastName,
                        NombreCompleto = response.FullName,
                        Telefono = response.PhoneNumber,
                        Direccion = response.Address,
                        Email = response.UserName,
                        TipoUsuario = response.TypeUser.Type,
                        Token = Token
                    };
                    return Ok(new { Data = data, Success = true });
                }


                return NotFound(new { Data = "User not found", Success = false });
            }catch(Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }
        }

        //Creacion de token, se llama en el metodo Login
        public async Task<string> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Username);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(
                        user,
                        model.Password);

                    if (result.Succeeded)
                    {
                        var response = await _dataContext.Users.Include(x => x.TypeUser).Where(x => x.UserName == user.UserName).FirstOrDefaultAsync();
                        var claims = new[]
                        {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                            new Claim(ClaimTypes.Role, response.TypeUser.Type)
                        };
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
                        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken(
                            issuer:_configuration["Tokens:Issuer"],
                            audience:_configuration["Tokens:Audience"],
                            claims:claims,
                            notBefore:null,
                            expires: DateTime.UtcNow.AddYears(30),
                            signingCredentials: credentials);

                        return new JwtSecurityTokenHandler().WriteToken(token);
                    }
                }
            }

            return null;
        }

        //Create User: api/Account/Create
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create(AddUsersViewModel model)
        {
            if (_dataContext.Users.Any(x => x.Document == model.Document))
                return BadRequest(new { Data = "El documento ingresado ya se encuentra registrado", Success = false });

            if (_dataContext.Users.Any(x => x.UserName == model.Username))
                return BadRequest(new { Data = "El Email ingresado ya se encuentra registrado", Success = false });

            var Tipo = _dataContext.TypeUsers.First(x => x.ID == model.Type);
            var TipoDocumento = _dataContext.TypeDocuments.First(x => x.ID == model.TypeDocument);
            var user = new User
            {
                Address = model.Address,
                TypeDocument = TipoDocumento,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Username,
                TypeUser = Tipo,
                State = true,
            };
            var response = await _userHelper.AddUserAsync(user, model.Password);
            if (response.Succeeded)
            {
                var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);
                await _userHelper.AddUserToRoleAsync(userInDB, Tipo.Type);
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = user, Success = true });

                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = "Error al guardar en la base de datos", Success = false });
                }
            }
            return BadRequest(new { Data = "Error al crear al usuario", Success = false });
        }

        [HttpPost]
        [Route("RecoverPassword")]
        public async Task<IActionResult> Recovery(EmailRequest Email)
        {
            if (string.IsNullOrEmpty(Email.Email))
                return BadRequest(new { Data = "Email not found", Success = false });

            var longitud = 7;
            string caracteres = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < longitud--)
            {
                res.Append(caracteres[rnd.Next(caracteres.Length)]);
            }
            var user = _dataContext.Users.First(x => x.UserName == Email.Email);
            var response = await _userHelper.ChangePasswordRecovery(user, res.ToString());

            if (!response.Succeeded)
                return BadRequest(new { Data = "Hubo un error al guardar la contraseña", Success = false });

            try
            {
                await _dataContext.SaveChangesAsync();
            }catch(Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }

            SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("sebas.builes.a@gmail.com", "100399Gmail");
            client.EnableSsl = true;
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("sebas.builes.a@gmail.com");
            mailMessage.To.Add(Email.Email);
            mailMessage.Body = res.ToString();
            mailMessage.Subject = "Codigo para recuperación de contraseña";
            client.Send(mailMessage);

            return Ok(new { Data = "Codigo enviado correctamente", Success = true});
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword(EmailRequest Email)
        {
            if (string.IsNullOrEmpty(Email.Email))
                return BadRequest(new { Data = "Email not found", Success = false });

            if (string.IsNullOrEmpty(Email.OldPassword))
                return BadRequest(new { Data = "OldPassword not found", Success = false });

            if (string.IsNullOrEmpty(Email.NewPassword))
                return BadRequest(new { Data = "NewPassword not found", Success = false });

            var user = _dataContext.Users.First(x => x.UserName == Email.Email);
            var response = await _userHelper.ChangePassword(user, Email.OldPassword, Email.NewPassword);
            if (!response.Succeeded)
                return BadRequest(new { Data = "Hubo un error al guardar la contraseña", Success = false });

            try
            {
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Data = ex.ToString(), Success = false });
            }

            return Ok(new { Data = "La contraseña se actualizo con exito", Success = true });
        }

        [HttpPost]
        [Route("Edit")]
        [Authorize(Roles = "Administrador", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Edit(AddUsersViewModel model) 
        {
            if (_dataContext.Users.Any(x => x.Id != model.Id && x.Document == model.Document))
                return BadRequest(new { Data = "El documento ingresado ya se encuentra registrado", Success = false });

            if (_dataContext.Users.Any(x => x.Id != model.Id && x.UserName == model.Username))
                return BadRequest(new { Data = "El Email ingresado ya se encuentra registrado", Success = false });

            var Tipo = _dataContext.TypeUsers.First(x => x.ID == model.Type);
            var TipoDocumento = _dataContext.TypeDocuments.First(x => x.ID == model.TypeDocument);
            var user = _dataContext.Users.FirstOrDefault(x => x.Id == model.Id);
            if(user != null)
            {
                user.Address = model.Address;
                user.TypeDocument = TipoDocumento;
                user.Document = model.Document;
                user.Email = model.Username;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.UserName = model.Username;
                user.TypeUser = Tipo;
            }else
            {
                return BadRequest(new { Data = "El usuario por alguna razón no existe", Success = false });
            }

            var response = await _userHelper.UpdateUserAsync(user);
            if (response.Succeeded)
            {
                var userInDB = await _userHelper.GetUserByEmailAsync(model.Username);
                await _userHelper.AddUserToRoleAsync(userInDB, Tipo.Type);
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = user, Success = true });

                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = "Error al guardar en la base de datos", Success = false });
                }
            }
            return BadRequest(new { Data = "Error al crear al usuario", Success = false });
        }

        [HttpPost]
        [Route("Delete")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Delete(AddUsersViewModel model)
        {
            var user = _dataContext.Users.FirstOrDefault(x => x.Id == model.Id);
            if (user != null)
            {
                user.State = !user.State;
            }
            else
            {
                return BadRequest(new { Data = "El usuario por alguna razón no existe", Success = false });
            }

            var response = await _userHelper.UpdateUserAsync(user);
            if (response.Succeeded)
            {
                var userInDB = await _userHelper.GetUserByEmailAsync(user.Email);
                try
                {
                    await _dataContext.SaveChangesAsync();
                    return Ok(new { Data = user, Success = true });

                }
                catch (Exception ex)
                {
                    return BadRequest(new { Data = "Error al guardar en la base de datos", Success = false });
                }
            }
            return BadRequest(new { Data = "Error al crear al usuario", Success = false });
        }

        [HttpPost]
        [Route("GetUsers")]
        [Authorize(Roles = "Administrador", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUsers()
        {
            var Customers = _dataContext.Users.Include(x => x.TypeUser).Include(x => x.TypeDocument).Select(x => new
            {
                ID = x.Id,
                Documento = x.Document,
                IDTipoDocumento = x.TypeDocument.ID,
                TipoDocumento = x.TypeDocument.Nombre,
                Nombre = x.FirstName,
                Apellido = x.LastName,
                NombreCompleto = x.FullName,
                Telefono = x.PhoneNumber,
                Direccion = x.Address,
                Email = x.UserName,
                Estado = x.State,
                TipoUsuario = x.TypeUser.Type,
                IDTipo = x.TypeUser.ID
            }).ToList();
            return Ok(new { Data = Customers, Success = true });
        }

        [HttpPost]
        [Route("GetVias")]
        public IActionResult GetVias()
        {
            var Vias = _dataContext.TypeVias.ToList();
            if (Vias.Count > 0)
            {
                return Ok(new { Data = Vias, Success = true });
            }
            else
            {
                return BadRequest(new { Data = "No hay registro de vias", Success = false });
            }
        }

        [HttpPost]
        [Route("GetTipos")]
        public IActionResult GetTipos()
        {
            var Types = _dataContext.TypeUsers.ToList();
            if (Types.Count > 0)
            {
                return Ok(new { Data = Types, Success = true });
            }
            else
            {
                return BadRequest(new { Data = "No hay registro de tipos de usuario", Success = false });
            }
        }

        [HttpPost]
        [Route("GetTipoDocumento")]
        public IActionResult GetTipoDocumento()
        {
            var Types = _dataContext.TypeDocuments.ToList();
            if (Types.Count > 0)
            {
                return Ok(new { Data = Types, Success = true });
            }
            else
            {
                return BadRequest(new { Data = "No hay registro de tipos de documentos", Success = false });
            }
        }

    }
}
