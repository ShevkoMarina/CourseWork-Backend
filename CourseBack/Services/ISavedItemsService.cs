using System;
using System.Collections.Generic;
using System.Linq;
using CourseBack.Models;
using System.Threading.Tasks;

namespace CourseBack.Services
{
    public interface ISavedItemsService
    {
        public Task<(string Error, string Url)> UploadToBlob(UserPhotoRequest photo);
        public string AddItem(SavedItemRequest item);
        public IEnumerable<SavedItem> GetSavedItems();

        public Task<IEnumerable<SavedItemRequest>> FindSimularGoods(string imageUrl, Guid userId);
    }
}
