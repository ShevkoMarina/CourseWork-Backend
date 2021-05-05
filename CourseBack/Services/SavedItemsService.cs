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
using Newtonsoft.Json;
using System.Net;
using System.Drawing;

namespace CourseBack.Services
{
    public class SavedItemsService : ISavedItemsService
    {
        const string containerName = "usersphotos";
        const string croppedContainer = "croppedimages";
        // Configuration
        const string conntectionString = "DefaultEndpointsProtocol=https;AccountName=neuralphotosblob;AccountKey=RefSuxn7AiKuRE4mkfeTWq1PY/P/" +
           "b8UgOuZBzugzWpfwoy2TFLPWsPFyf+JyOO0NucJvcJK4aLXbnenmkh5GxQ==;EndpointSuffix=core.windows.net";
        const string currentImagePath = "currentImage.jpeg";

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

        public string SaveItems(List<SavedItem> items)
        {
            try
            {
                _recognizedItemsRepository.AddBatchItems(items);
                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            } 
        }

        public async Task<(string Error, IEnumerable<SavedItem> items)> FindSimularGoods(string imageUrl, Guid userId)
        {
            try
            {
                Parser parser = new Parser(imageUrl, userId);
                var items = await parser.GetData();
                if (items == null)
                {
                    return ("Parsing error", null);
                }
               // _recognizedItemsRepository.AddBatchItems(items.Take(6));
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

        public List<RecognizedItem> MakePrediction(string imageUrl)
        {
            try
            {
                (Bitmap originalBitmap, String filepath) = DownloadImageByUrl(imageUrl);
                if (originalBitmap != null)
                {
                    var results = YoloNetwork.MakeYoloPrediction(imageUrl);
                    var items = ProcessResults(results, originalBitmap);

                    return items;
                }
                var item = new List<RecognizedItem>();
                item.Add(new RecognizedItem(filepath, ""));
                return item;
            }
            catch (Exception ex)
            {
                var item = new List<RecognizedItem>();
                item.Add(new RecognizedItem(ex.Message, ""));
                return item;
            }
        }

        private (Bitmap, String) DownloadImageByUrl(string imageUrl)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (Stream stream = client.OpenRead(imageUrl))
                    {
                        Bitmap bitmap = new Bitmap(stream);

                        if (bitmap != null)
                        {
                             bitmap.Save(currentImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                             return (bitmap, currentImagePath);
                        }
                    }
                }
                return (null, null);
            }
            catch (Exception ex)
            {
                return (null, "error saving");
            }

        }

        private List<RecognizedItem> ProcessResults(IReadOnlyList<YoloV4Result> results, Bitmap originalBitmap)
        {
            List<RecognizedItem> items = new List<RecognizedItem>();

            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].Confidence > 0.2)
                {     
                    // Save cropped bitmap in temp
                    var croppedBmpUrl = SaveCroppedImage(results[i].BBox, originalBitmap, Guid.NewGuid().ToString() + ".jpg").GetAwaiter().GetResult();

                    // Add in output list
                    if (croppedBmpUrl != null)
                    {
                        items.Add(new RecognizedItem(croppedBmpUrl, results[i].Label));
                    }
                }
            }

            return items;
        }

        private static async Task<String> SaveCroppedImage(float[] BBox, Bitmap originalBitmap, String fileName)
        {
            // In bitmap saved an image. Now crop it with bounding box
            var x1 = BBox[0];
            var y1 = BBox[1];
            var x2 = BBox[2];
            var y2 = BBox[3];

            Bitmap bmpCrop = originalBitmap.Clone(new Rectangle((int)x1, (int)y1, (int)(x2 - x1), (int)(y2 - y1)), originalBitmap.PixelFormat);

            if (bmpCrop != null)
            {
                //bmpCrop.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                await SaveCroppedInBlob(bmpCrop, fileName);
                return "https://neuralphotosblob.blob.core.windows.net/croppedimages/" + fileName;
            }
            return null;
        }

        private static async Task SaveCroppedInBlob(Bitmap croppedImage, String fileName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(croppedContainer);
            await containerClient.CreateIfNotExistsAsync();

            containerClient.SetAccessPolicy(PublicAccessType.Blob);
            using (MemoryStream ms = new MemoryStream())
            {
                croppedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(ms, new BlobHttpHeaders { ContentType = "image/jpeg" });
            }
        }


        /*
        public async Task<List<RecognizedItem>> MakePrediction(string imageUrl)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-Key", "f603b1feddce40ac8758932ea45abe8b");
            string baseUrl = "https://iseecustomvision.cognitiveservices.azure.com/customvision/v3.0/Prediction/" +
                "943f4fa0-faca-496e-99bd-69fb5dd1e3dc/detect/iterations/Iteration5/url";

            String jsonString = JsonConvert.SerializeObject(new  RecognizeModel(imageUrl));
            var content = new StringContent(jsonString.ToString(), System.Text.Encoding.UTF8, "application/json");
            var result = client.PostAsync(baseUrl, content).Result;

            RecognitionResult recognitionResult = null;
            if (result.IsSuccessStatusCode)
            {
                var resultString = await result.Content.ReadAsStringAsync();
                recognitionResult = JsonConvert.DeserializeObject<RecognitionResult>(resultString);
                List<RecognizedItem> items = ProcessResult(recognitionResult, imageUrl);

                return items;
                
            }
            return null;
        }

        private List<RecognizedItem> ProcessResult(RecognitionResult result, String imageUrl)
        {
            List<RecognizedItem> items = new List<RecognizedItem>();
            RecognitionResult newResult = new RecognitionResult();
            for (int i = 0; i < result.predictions.Count(); i++)
            {
                if (result.predictions[i].probability > 0.5)
                {
                    newResult.predictions.Add(result.predictions[i]);
                    var btm = DownloadImageByUrl(imageUrl);
                    var croppedUrl = SaveCroppedImage(result.predictions[i].boundingBox, btm, i + ".jpg").GetAwaiter();
                    items.Add(new RecognizedItem(croppedUrl.GetResult(), result.predictions[i].tagName));
                }
            }

            return items;
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

        private static async Task<String> SaveCroppedImage(BoundingBox box, Bitmap bitmap, String fileName)
        {
            // In bitmap saved an image. Now crop it with bounding box
            Bitmap bmpCrop = bitmap.Clone(new Rectangle((int)(box.left * bitmap.Width), (int)(box.top * bitmap.Height),
                (int)(box.width * bitmap.Width), (int)(box.height * bitmap.Height)), bitmap.PixelFormat);

            if (bmpCrop != null)
            {
                //bmpCrop.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                await SaveCroppedInBlob(bmpCrop, fileName);
                return "https://neuralphotosblob.blob.core.windows.net/croppedimages/" + fileName;
            }
            return null;
        }

        private static async Task SaveCroppedInBlob(Bitmap croppedImage, String fileName)
        {
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(croppedContainer);
            await containerClient.CreateIfNotExistsAsync();

            containerClient.SetAccessPolicy(PublicAccessType.Blob);
            using (MemoryStream ms = new MemoryStream())
            {
                croppedImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                ms.Seek(0, SeekOrigin.Begin);
                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                await blobClient.UploadAsync(ms, new BlobHttpHeaders { ContentType = "image/jpeg" });
            }
        }
        */
    }
}
