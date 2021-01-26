using CourseBack.Models;
using System;
using System.Collections.Generic;
using CourseBack.Repository;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Linq;

namespace CourseBack.Services
{
    public class SavedItemsService : ISavedItemsService
    {
        const string containerName = "usersphotos";
        // Configuration
        const string conntectionString = "DefaultEndpointsProtocol=https;AccountName=neuralphotosblob;AccountKey=RefSuxn7AiKuRE4mkfeTWq1PY/P/" +
           "b8UgOuZBzugzWpfwoy2TFLPWsPFyf+JyOO0NucJvcJK4aLXbnenmkh5GxQ==;EndpointSuffix=core.windows.net";

        private static BlobServiceClient blobServiceClient = new BlobServiceClient(conntectionString);


        private IRecognizedItemsRepository _recognizedItemsRepository;

        public SavedItemsService(IRecognizedItemsRepository recognizedItemsRepository)
        {
            _recognizedItemsRepository = recognizedItemsRepository;
        }

        public string AddItem(RecognizeItemRequest item)
        {
            try
            {
                _recognizedItemsRepository.AddItem(item);
                return null;
            }
            catch (Exception)
            {
                return "Database connection error";
            }
        }

        public (string Error, IEnumerable<SavedItem> items) GetSavedItems()
        {
            try
            {
                var items = _recognizedItemsRepository.GetSavedItems();

                if (items.Count == 0)
                {
                    return ("No saved items", null);
                }

                return (null, items);
            }
            catch (Exception)
            {
                return ("Database connection error", null);
            }
        }

        // один контейнер для всех
        public async Task<(string Error, string Url)> UploadToBlob(UserPhotoRequest userPhoto)
        {
            try
            {
                /*
                string containerName = Guid.NewGuid().ToString();
                */

                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync();

                containerClient.SetAccessPolicy(PublicAccessType.Blob);
                BlobClient blobClient = containerClient.GetBlobClient(userPhoto.Photo.FileName);
                await blobClient.UploadAsync(userPhoto.Photo.OpenReadStream(), new BlobHttpHeaders { ContentType = "image/jpeg" });

                return (null, blobClient.Uri.ToString());
            }
            catch (Exception)
            {
                return ("Blob connection error", null);
            }
        }

        public async Task<(string Error, IEnumerable<SavedItem> items)> FindSimularGoods(string imageUrl, Guid userId)
        {
            try
            {
                Parser parser = new Parser(imageUrl, userId);
                var items = await parser.GetData();
                _recognizedItemsRepository.AddBatchItems(items.Take(6));
                return (null, items.Take(6));
            }
            catch(NullReferenceException ex)
            {
                return (ex.Message, null);
            }
            catch(Exception ex)
            {
                return (ex.Message, null);
            }
        }

        public string DeleteAllItems()
        {
            try
            {
                _recognizedItemsRepository.DeleteAllItems();
                return null;
            }
            catch(Exception ex)
            {
                return (ex.Message);
            }
        }

        public (IEnumerable<SavedItem> Items, string Error) GetUsersItems(Guid id)
        {
            try
            {
                var items = _recognizedItemsRepository.GetUserItems(id);
                return (items, null);
            }
            catch(Exception)
            {
                return (null, "Database connection error");
            }
        }
    }
}
