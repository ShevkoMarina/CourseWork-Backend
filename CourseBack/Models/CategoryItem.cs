using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Models
{
    public class CategoryItem
    {
        public string imageUrl { get; set; }
        public string category { get; set; }

        public CategoryItem(String imageUrl, String category)
        {
            this.category = category;
            this.imageUrl = imageUrl;
        }
    }
}
