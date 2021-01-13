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
        string conntectionString = "DefaultEndpointsProtocol=https;AccountName=neuralphotosblob;AccountKey=RefSuxn7AiKuRE4mkfeTWq1PY/P/" +
            "b8UgOuZBzugzWpfwoy2TFLPWsPFyf+JyOO0NucJvcJK4aLXbnenmkh5GxQ==;EndpointSuffix=core.windows.net";

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
                BlobServiceClient blobServiceClient = new BlobServiceClient(conntectionString);
                string containerName = Guid.NewGuid().ToString();
                BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
                containerClient.SetAccessPolicy(PublicAccessType.Blob);
                BlobClient blobClient = containerClient.GetBlobClient(userPhoto.Photo.FileName);

                await blobClient.UploadAsync(userPhoto.Photo.OpenReadStream(), new BlobHttpHeaders { ContentType = userPhoto.Photo.ContentType });
                return Ok(blobClient.Uri);
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

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] SavedItemRequest item)
        {
            var result = _savedItemsService.AddItem(item);
            return Ok();
        }
    }
}
