using CourseBack.Models;
using System.Collections.Generic;

namespace CourseBack.Repository
{
    public interface IRecognizedItemsRepository
    {
        public void AddItem(RecognizeItemRequest item);
        public IReadOnlyCollection<SavedItem> GetSavedItems();

        public void AddBatchItems(IEnumerable<SavedItem> items);
    }
}
