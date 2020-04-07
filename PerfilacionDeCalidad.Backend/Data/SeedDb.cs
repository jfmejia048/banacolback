using PerfilacionDeCalidad.Backend.Data.Entities;
using PerfilacionDeCalidad.Backend.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _dataContext;
        private readonly IUserHelper _userHelper;

        public SeedDb(
            DataContext context,
            IUserHelper userHelper)
        {
            _dataContext = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _dataContext.Database.EnsureCreatedAsync();
            await CheckRoles();
            var Admin = await CheckType("Administrador");
            var Customer = await CheckType("Usuario");
            await CreateVias();
            await CheckTipos();
            var manager = await CheckUserAsync("1010", _dataContext.TypeDocuments.First(x => x.Abreviatura == "CC"), "Sebastian", "Builes", "Admin@gmail.com", "3017019551", "CARRERA-43a-#-1sur-188", true, Admin);
            var customer = await CheckUserAsync("2020", _dataContext.TypeDocuments.First(x => x.Abreviatura == "CC"), "Sebastian", "Builes", "Customer@gmail.com", "3017019551", "CALLE-19-#-88-11", true, Customer);

        }

        public async Task CreateVias()
        {
            if (!_dataContext.TypeVias.Any())
            {
                List<TypeVias> Vias = new List<TypeVias>();
                Vias.Add(new TypeVias { ID = "AU", TypeVia = "AUTOPISTA" });
                Vias.Add(new TypeVias { ID = "AV", TypeVia = "AVENIDA" });
                Vias.Add(new TypeVias { ID = "AC", TypeVia = "AVENIDA CALLE" });
                Vias.Add(new TypeVias { ID = "AK", TypeVia = "AVENIDA CARRERA" });
                Vias.Add(new TypeVias { ID = "BL", TypeVia = "BULEVAR" });
                Vias.Add(new TypeVias { ID = "CL", TypeVia = "CALLE" });
                Vias.Add(new TypeVias { ID = "KR", TypeVia = "CARRERA" });
                Vias.Add(new TypeVias { ID = "CT", TypeVia = "CARRETERA" });
                Vias.Add(new TypeVias { ID = "CQ", TypeVia = "CIRCULAR" });
                Vias.Add(new TypeVias { ID = "CV", TypeVia = "CIRCUNVALAR" });
                Vias.Add(new TypeVias { ID = "CC", TypeVia = "CUENTAS CORRIDAS" });
                Vias.Add(new TypeVias { ID = "DG", TypeVia = "DIAGONAL" });
                Vias.Add(new TypeVias { ID = "PJ", TypeVia = "PASAJE" });
                Vias.Add(new TypeVias { ID = "PS", TypeVia = "PASEO" });
                Vias.Add(new TypeVias { ID = "PT", TypeVia = "PEATONAL" });
                Vias.Add(new TypeVias { ID = "TV", TypeVia = "TRANSVERSAL" });
                Vias.Add(new TypeVias { ID = "TC", TypeVia = "TRONCAL" });
                Vias.Add(new TypeVias { ID = "VT", TypeVia = "VARIANTE" });
                Vias.Add(new TypeVias { ID = "VI", TypeVia = "VÍA" });
                _dataContext.TypeVias.AddRange(Vias);
                await _dataContext.SaveChangesAsync();
            }
        }

        public async Task CheckTipos()
        {
            if (!_dataContext.TypeDocuments.Any())
            {
                List<TypeDocument> Tipos = new List<TypeDocument>();
                Tipos.Add(new TypeDocument { Abreviatura = "CC", Nombre = "Cedula de Ciudadania" });
                Tipos.Add(new TypeDocument { Abreviatura = "CE", Nombre = "Cedula de Extrangeria" });
                Tipos.Add(new TypeDocument { Abreviatura = "TI", Nombre = "Tarjeta de Identidad" });
                _dataContext.TypeDocuments.AddRange(Tipos);
                await _dataContext.SaveChangesAsync();
            }
        }

        private async Task CheckRoles()
        {
            await _userHelper.CheckRoleAsync("Administrador");
            await _userHelper.CheckRoleAsync("Usuario");
        }

        private async Task<User> CheckUserAsync(string document, TypeDocument TypeDocument, string firstName, string lastName, string email, string phone, string address, bool state, TypeUser role)
        {
            var user = await _userHelper.GetUserByEmailAsync(email);
            if (user == null)
            {
                user = new User
                {
                    FirstName = firstName,
                    TypeDocument = TypeDocument,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    TypeUser = role,
                    State = state
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, role.Type);
            }

            return user;
        }

        private async Task<TypeUser> CheckType(string Type)
        {
            var Types = _dataContext.TypeUsers.FirstOrDefault(x => x.Type == Type);
            if (Types == null)
            {
                var type = new TypeUser
                {
                    Type = Type
                };
                _dataContext.TypeUsers.Add(type);
                await _dataContext.SaveChangesAsync();
                return type;
            }
            return Types;
        }

    }
}
