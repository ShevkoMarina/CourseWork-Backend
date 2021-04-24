using CourseBack.Models;
using System;
using System.Net.Http;
using System.Collections.Generic;
using CourseBack.Repository;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Linq;
using System.IO;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;
using System.Drawing;

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

        public async Task<List<String>> MakePrediction(string imageUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "f603b1feddce40ac8758932ea45abe8b");
            string baseUrl = "https://iseecustomvision.cognitiveservices.azure.com/customvision/v3.0/Prediction/" +
                         "943f4fa0-faca-496e-99bd-69fb5dd1e3dc/detect/iterations/Iteration4/url";

            String jsonString = JsonConvert.SerializeObject(new  RecognizeModel(imageUrl));
            var content = new StringContent(jsonString.ToString(), System.Text.Encoding.UTF8, "application/json");
            var result = client.PostAsync(baseUrl, content).Result;

            RecognitionResult recognitionResult = null;
            if (result.IsSuccessStatusCode)
            {
                var resultString = await result.Content.ReadAsStringAsync();
                recognitionResult = JsonConvert.DeserializeObject<RecognitionResult>(resultString);
                (RecognitionResult newResult, List<String> recognozedItems) = ProcessResult(recognitionResult, imageUrl);
                return recognozedItems;
                
            }
            return null;
        }

        private (RecognitionResult, List<String>) ProcessResult(RecognitionResult result, String imageUrl)
        {
            List<String> recognizedItemsNames = new List<String>();
            RecognitionResult newResult = new RecognitionResult();
            for (int i = 0; i < result.predictions.Count(); i++)
            {
                if (result.predictions[i].probability > 0.5)
                {
                    newResult.predictions.Add(result.predictions[i]);
                    var btm = DownloadImageByUrl(imageUrl);
                    SaveCroppedImage(result.predictions[i].boundingBox, btm, i + ".jpg");
                    recognizedItemsNames.Add(result.predictions[i].tagName);
                }
            }

            return (newResult, recognizedItemsNames);
        }

        private Bitmap DownloadImageByUrl(string imageUrl)
        {
            try
            {
                var client = new WebClient();
                Stream stream = client.OpenRead(imageUrl);
                Bitmap bitmap = new Bitmap(stream);

                if (bitmap != null)
                {
                    bitmap.Save("currentImage.jpeg", System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                stream.Flush();
                stream.Close();
                client.Dispose();

                return bitmap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static void SaveCroppedImage(BoundingBox box, Bitmap bitmap, String fileName)
        {
            // In bitmap saved an image. Now crop it with bounding box
            Bitmap bmpCrop = bitmap.Clone(new Rectangle((int)(box.left * bitmap.Width), (int)(box.top * bitmap.Height),
                (int)(box.width * bitmap.Width), (int)(box.height * bitmap.Height)), bitmap.PixelFormat);

            if (bmpCrop != null)
            {
                bmpCrop.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
    }
}
