using CourseBack.Models;
using System.Collections.Generic;

namespace CourseBack.Repository
{
    public interface IRecognizedItemsRepository
    {
        public string UploadToBlob(UserPhotoRequest photo);
        public void AddItem(RecognizeItemRequest item);
        public IReadOnlyCollection<SavedItem> GetSavedItems();
    }
}
