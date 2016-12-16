using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Presence.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usuário")]
        public string Username { get; set; }

        [Display(Name = "Administrador?")]
        public bool Admin { get; set; }

        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Data de Criação")]
        public DateTime CreationDate { get; set; }
    }
}
