using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowForge.Core.DTO
{
    public class ResetPasswordDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords Don't Match")]
        public string ConfirmPassword { get; set; }
        public string token { get; set; }
    }
}
