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

        /*
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RecognizeItem([FromForm] UserPhotoRequest request)
        {
            if (!Guid.TryParse(request.UserId, out _))
            {
                return BadRequest("Incorrect user id");
            }

            var uploadToBlobResult = await _savedItemsService.UploadToBlob(request);
            if (uploadToBlobResult.Error == null)
            {
                var simularGoodsResult = await _savedItemsService.FindSimularGoods(uploadToBlobResult.Url, Guid.Parse(request.UserId), );
                if (simularGoodsResult.Error == null)
                {
                    return Ok(simularGoodsResult.items);
                }

                var problemDetals = new ProblemDetails
                {
                    Status = 400,
                    Title = simularGoodsResult.Error
                };

                return new ObjectResult(problemDetals)
                {
                    ContentTypes = { "application/problem+json" },
                    StatusCode = 400
                };

            }
            return BadRequest("BLOB DO BRRRR");
        }
        */

        [HttpGet]
        public async Task<IActionResult> GetSavedItems()
        {
            return Ok();
        }


        [Route("[action]")]
        [HttpPost]
        public IActionResult AddItem([FromBody] RecognizeItemRequest item)
        {
            var resultError = _savedItemsService.AddItem(item);
            if (resultError == null)
            {
                return Ok();
            }
            return BadRequest(resultError);
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> FindSimilarByUrl([FromBody] FindSimilarRequest request)
        {
            List<SavedItem> similarItems = new List<SavedItem>();
            foreach (var item in request.items)
            {
                var similarGoodsResult = await _savedItemsService.FindSimularGoods(item.ImageUri, Guid.Parse(request.UserId), item.Category);

                if (similarGoodsResult.items != null)
                {
                    similarItems.AddRange(similarGoodsResult.items);
                }
            }
            return Ok(similarItems);
        }

        /*
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> FindByUrl([FromBody] RecognizeImageRequest photo)
        {
            var simularGoodsResult = await _savedItemsService.FindSimularGoods(photo.ImageUri, Guid.Parse(photo.UserId));

            if (simularGoodsResult.Error == null)
            {
                return Ok(simularGoodsResult.items);
            }

            var problemDetals = new ProblemDetails
            {
                Status = 400,
                Title = simularGoodsResult.Error
            };

            return new ObjectResult(problemDetals)
            {
                ContentTypes = { "application/problem+json" },
                StatusCode = 400
            };
        }

        */
        [Route("[action]")]
        [Produces("application/json")]
        [HttpPost]
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
        public IActionResult SaveItems([FromBody] List<SavedItem> request)
        {
            var result = _savedItemsService.SaveItems(request);
            if (result == null)
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpDelete]
        public IActionResult DeleteAllItems()
        {
            var result = _savedItemsService.DeleteAllItems();

            if (String.IsNullOrEmpty(result))
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetUsersItems(Guid id)
        {
            var result = _savedItemsService.GetUsersItems(id);

            if (String.IsNullOrEmpty(result.Error))
            {
                return Ok(result.Items);
            }
            return BadRequest(result.Error);
        }

        [Route("[action]")]
        [Produces("application/json")]
        [HttpGet()]
        public IActionResult MakePrediction([FromQuery] string url)
        {
            List<RecognizedItem> result = _savedItemsService.MakePrediction(url);

            return Ok(result);
        }

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
        public IActionResult GetUserItemsByCategory([FromQuery] Guid userId, String category)
        {
            var result = _savedItemsService.GetUserItemsByCategory(userId, category);
            return Ok(result);
        }
    }
}
























