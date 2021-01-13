using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseBack.Repository
{
    public interface IRecognizedItemsRepository
    {
        public (string Error, string Url) UploadToBlob(UserPhoto photo);
        public string AddItem(SavedItemRequest item);
        public IReadOnlyCollection<SavedItemRequest> GetSavedItems();
    }
}
