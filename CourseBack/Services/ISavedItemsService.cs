using System;
using System.Collections.Generic;
using System.Linq;
using CourseBack.Models;
using System.Threading.Tasks;

namespace CourseBack.Services
{
    public interface ISavedItemsService
    {
        public (string Error, string Url) UploadToBlob(UserPhoto photo);
        public string AddItem(SavedItemRequest item);
        public IEnumerable<SavedItemRequest> GetSavedItems();
    }
}
