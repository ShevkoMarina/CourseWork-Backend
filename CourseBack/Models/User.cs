using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseBack.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("ID")]
        public Guid Id { get; set; }

        [Required]
        [Column("user_login")]
        [MaxLength(20)]
        public string Login { get; set; }

        [Required]
        [Column("user_password")]
        [MaxLength(20)]
        public string Password { get; set; }
    }
}
