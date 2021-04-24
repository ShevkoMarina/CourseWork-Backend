using System;
using System.Collections.Generic;

namespace CourseBack.Models
{
    public class RecognitionResult
    {
        public String id;
        public String project;
        public String iteration;
        public DateTime created;
        public List<Prediction> predictions = new List<Prediction>();
    }
}
