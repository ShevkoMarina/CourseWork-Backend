using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Models
{
    public class UserPhoto
    {
        [FromForm(Name = "file")]
        public IFormFile Photo { get; set; }

        [FromForm(Name = "id")]

        public string UserId { get; set; }
    }
}
