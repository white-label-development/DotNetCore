using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SportsStore.Models
{
    public class LoginModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        // ReSharper disable once Mvc.TemplateNotResolved
        [UIHint("password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; } = "/";
    }
}
