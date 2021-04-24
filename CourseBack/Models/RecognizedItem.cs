using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Models
{
    public class RecognizedItem
    {
        public string Url { get; set; }
        public string Name { get; set; }

        public RecognizedItem(string Url, string Name)
        {
            this.Url = Url;
            this.Name = Name;
        }
    }
}
