using System.Collections.Generic;

namespace CourseBack.Models
{
    public class FindSimilarRequest
    {
        public string UserId { get; set; }
        public List<FindSimilarImage> items { get; set; }
    }
}
