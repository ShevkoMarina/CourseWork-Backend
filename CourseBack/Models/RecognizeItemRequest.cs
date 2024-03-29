﻿using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseBack.Models
{
    public class RecognizeItemRequest
    {
        public Guid UserId { get; set; }
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public string WebUrl { get; set; }
    }
}
