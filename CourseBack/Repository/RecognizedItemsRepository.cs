using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Repository
{
    public class RecognizedItemsRepository : IRecognizedItemsRepository
    {
        public string AddItem(SavedItemRequest item)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<SavedItemRequest> GetSavedItems()
        {
            throw new NotImplementedException();
        }

        public (string Error, string Url) UploadToBlob(UserPhoto photo)
        {
            throw new NotImplementedException();
        }
    }
}
