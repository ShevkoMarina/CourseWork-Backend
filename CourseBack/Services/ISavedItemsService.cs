using System;
using System.Collections.Generic;
using System.Linq;
using CourseBack.Models;
using System.Threading.Tasks;

namespace CourseBack.Services
{
    // remove public modifire
    public interface ISavedItemsService
    {
        public Task<(string Error, string Url)> UploadToBlob(UserPhotoRequest photo);
        public string AddItem(RecognizeItemRequest item);
        public (string Error, IEnumerable<SavedItem> items) GetSavedItems();

        public Task<(string Error, IEnumerable<SavedItem> items)> FindSimularGoods(string imageUrl, Guid userId);

        public string DeleteAllItems();

        public (IEnumerable<SavedItem> Items, string Error) GetUsersItems(Guid id);

        public Task<List<String>> MakePrediction(string imageFilePath);
    }
}
