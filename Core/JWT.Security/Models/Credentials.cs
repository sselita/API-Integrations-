using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JWT.Security.Models
{
    public class Credentials
    {
        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}