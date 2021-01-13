using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Services
{
    public class SavedItemsService : ISavedItemsService
    {
        public string AddItem(SavedItemRequest item)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SavedItemRequest> GetSavedItems()
        {
            throw new NotImplementedException();
        }

        public (string Error, string Url) UploadToBlob(UserPhoto photo)
        {
            throw new NotImplementedException();
        }
    }
}
