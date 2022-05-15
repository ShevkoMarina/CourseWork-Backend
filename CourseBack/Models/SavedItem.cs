using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseBack.Models
{
    [Table("items")]
    public class SavedItem
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Required]
        [Column("image_url", TypeName = "text")]
        public string ImageUrl { get; set; }


        [Column("category")]
        [MaxLength(200)]
        public string Category { get; set; }

        [Required]
        [Column("name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [Column("price")]
        [MaxLength(10)]
        public string Price { get; set; }

        [Required]
        [Column("web_url", TypeName = "text")]
        public string WebUrl { get; set; }

        public List<User> Users { get; set; }
    }
}