using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Models
{
    public class YoloRecognitionResult
    {
        public float[] BBox { get; }

        public string Label { get; }

        public float Confidence { get; }

        public YoloRecognitionResult(float[] bbox, string label, float confidence)
        {
            BBox = bbox;
            Label = label;
            Confidence = confidence;
        }
    }
}
