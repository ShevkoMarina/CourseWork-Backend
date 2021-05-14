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
        public (string Error, IEnumerable<SavedItem> items) GetSavedItems(Guid userId);

        public Task<(string Error, IEnumerable<SavedItem> items)> FindSimularGoods(string imageUrl, Guid userId, string category);

        public string DeleteAllItems();

        public (IEnumerable<SavedItem> Items, string Error) GetUsersItems(Guid id);

        public List<RecognizedItem> MakePrediction(string imageFilePath);

        public string SaveItems(List<SavedItem> items);

        public List<CategoryItem> GetUserCategories(Guid userId);

        public IEnumerable<SavedItem> GetUserItemsByCategory(Guid userId, String category);
    }
}
