using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Storage.Blobs;
using CourseBack.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        [HttpPost]
        public async Task<IActionResult> UploadToBlob([FromForm] UserPhoto userPhoto)
        {
            try
            {
                var result = await _savedItemsService.UploadToBlob(userPhoto);
                return Ok(result.Url);
            }
            catch (Exception e)
            {
                return BadRequest("BLOB DO BRRRR");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSavedItems()
        {
            return Ok();
        }

        // разобраться с асинхроннкой
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] SavedItemRequest item)
        {
            var result = _savedItemsService.AddItem(item);
            return Ok();
        }

        // не хочет принимать интегер как гуид
        // возвращать не все а штук 6
        [Route("[action]")]
        [HttpPost]
        public async Task<IActionResult> RecognizeItem(RecognizeImageRequest request)
        {
            var result = await _savedItemsService.FindSimularGoods(request.ImageUri, request.UserId);
            return Ok(result);
        }
    }
}




