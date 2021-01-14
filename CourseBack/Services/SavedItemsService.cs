using CourseBack.Models;
using System;
using System.Collections.Generic;
using CourseBack.Repository;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace CourseBack.Services
{
    public class SavedItemsService : ISavedItemsService
    {
        string conntectionString = "DefaultEndpointsProtocol=https;AccountName=neuralphotosblob;AccountKey=RefSuxn7AiKuRE4mkfeTWq1PY/P/" +
           "b8UgOuZBzugzWpfwoy2TFLPWsPFyf+JyOO0NucJvcJK4aLXbnenmkh5GxQ==;EndpointSuffix=core.windows.net";

        private IRecognizedItemsRepository _recognizedItemsRepository;
        public SavedItemsService(IRecognizedItemsRepository recognizedItemsRepository)
        {
            _recognizedItemsRepository = recognizedItemsRepository;
        }

        public string AddItem(SavedItemRequest item)
        {
           return  _recognizedItemsRepository.AddItem(item);
        }

        public IEnumerable<SavedItem> GetSavedItems()
        {
            return _recognizedItemsRepository.GetSavedItems();
        }

        // вынести а репозиторий
        public async Task<(string Error, string Url)> UploadToBlob(UserPhotoRequest userPhoto)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(conntectionString);
                string containerName = Guid.NewGuid().ToString();
                BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
                containerClient.SetAccessPolicy(PublicAccessType.Blob);
                BlobClient blobClient = containerClient.GetBlobClient(userPhoto.Photo.FileName);
                await blobClient.UploadAsync(userPhoto.Photo.OpenReadStream(), new BlobHttpHeaders { ContentType = userPhoto.Photo.ContentType });

                return (null, blobClient.Uri.ToString());
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
        }

        public async Task<IEnumerable<SavedItemRequest>> FindSimularGoods(string imageUrl, Guid userId)
        {
            try
            {
                Parser parser = new Parser(imageUrl, userId);
                return await parser.GetData();
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
