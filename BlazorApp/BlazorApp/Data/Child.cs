using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Data
{
    public class Child
    {
        [Required]
        public string Code { get; set; }

        [StringLength(5, ErrorMessage = "长度超出")]
        public string Name { get; set; }
    }
}
