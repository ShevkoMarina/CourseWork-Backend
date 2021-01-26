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


        // parse user id to guid

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
                var simularGoodsResult = await _savedItemsService.FindSimularGoods(uploadToBlobResult.Url, Guid.Parse(request.UserId));
                if (simularGoodsResult.Error == null)
                {
                    return Ok(simularGoodsResult.items);
                }
                // return BadRequest(simularGoodsResult.Error);

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

                //return Ok(simularGoodsResult);
            }
            return BadRequest("BLOB DO BRRRR");
        }

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
    }
}




























