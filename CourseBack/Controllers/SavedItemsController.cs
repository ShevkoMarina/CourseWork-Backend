using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using CourseBack.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using Azure.Storage.Blobs.Models;
using Newtonsoft.Json;
using CourseBack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CourseBack.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SavedItemsController : Controller
    {

        private readonly ISavedItemsService _savedItemsService;

        public SavedItemsController(ISavedItemsService savedItemsService)
        {
            _savedItemsService = savedItemsService;
        }

        [Route("[action]")]
        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> FindSimilarByUrl([FromBody] FindSimilarRequest request)
        {
            List<SavedItem> similarItems = new List<SavedItem>();
            foreach (var item in request.items)
            {
                var similarGoodsResult = await _savedItemsService.FindSimularGoods(item.ImageUri, Guid.Parse(request.UserId), item.Category, request.Engine);

                if (similarGoodsResult.items != null)
                {
                    similarItems.AddRange(similarGoodsResult.items);
                }
            }
            return Ok(similarItems);
        }

        [Route("[action]")]
        [Produces("application/json")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UploadToBlob([FromForm] UserPhotoRequest request)
        {
            var uploadToBlobResult = await _savedItemsService.UploadToBlob(request);
            if (uploadToBlobResult.Error == null)
            {

                return Ok(uploadToBlobResult.Url);
            }

            var problemDetals = new ProblemDetails
            {
                Status = 400,
                Title = uploadToBlobResult.Error
            };

            return new ObjectResult(problemDetals)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = 400
            };
        }

        [Route("[action]")]
        [Produces("application/json")]
        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult SaveItems([FromBody] List<SavedItem> request)
        {
            var result = _savedItemsService.SaveItems(request);
            if (result == null)
            {
                return Ok();
            }
            return BadRequest();
        }


        [Route("[action]")]
        [Produces("application/json")]
        [HttpGet()]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult MakePrediction([FromQuery] string url)
        {
            List<RecognizedItem> result = _savedItemsService.MakePrediction(url);

            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("[action]")]
        [Produces("application/json")]
        [HttpGet()]
        public IActionResult GetUserCategories([FromQuery] Guid guid)
        {
            List<CategoryItem> categories = _savedItemsService.GetUserCategories(guid);
            return Ok(categories);
        }

        [Route("[action]")]
        [Produces("application/json")]
        [HttpGet()]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult GetUserItemsByCategory([FromQuery] Guid userId, String category)
        {
            var result = _savedItemsService.GetUserItemsByCategory(userId, category);
            return Ok(result);
        }
    }
}
























