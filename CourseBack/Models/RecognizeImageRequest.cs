﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Models
{
    public class RecognizeImageRequest
    {
        public string UserId { get; set; }
        public string ImageUri { get; set; }

    }
}
