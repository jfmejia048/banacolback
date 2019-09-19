using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PerfilacionDeCalidad.Backend.Models
{
    public class AddUsersViewModel
    {
        public string Id { get; set; }

        public int TypeDocument { get; set; }

        public string Document { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address { get; set; }

        public string PhoneNumber { get; set; }

        public int Type { get; set; }

        public bool? State { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }
    }
}
