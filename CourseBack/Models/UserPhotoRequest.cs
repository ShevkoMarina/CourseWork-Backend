using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CourseBack.Models
{
    public class UserPhotoRequest
    {
        [FromForm(Name = "file")]
        public IFormFile Photo { get; set; }

        [FromForm(Name = "id")]

        public string UserId { get; set; }
    }
}
