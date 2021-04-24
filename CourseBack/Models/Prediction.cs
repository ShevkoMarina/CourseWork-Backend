using System;

namespace CourseBack.Models
{
    public class Prediction
    {
        public double probability;
        public String tagId;
        public String tagName;
        public BoundingBox boundingBox;
    }
}
