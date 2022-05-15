using System.Collections.Generic;

namespace CourseBack.Models
{
    public class FindSimilarRequest
    {
        public SearchEngine Engine { get; set; }
        public string UserId { get; set; }
        public List<FindSimilarImage> items { get; set; }
    }
}
