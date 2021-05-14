using CourseBack.Models;
using System;
using System.Collections.Generic;

namespace CourseBack.Repository
{
    public interface IRecognizedItemsRepository
    {
        public void AddItem(RecognizeItemRequest item);
        public IReadOnlyCollection<SavedItem> GetSavedItems(Guid userId);
        public void AddBatchItems(IEnumerable<SavedItem> items);
        public void DeleteAllItems();

        public IReadOnlyCollection<SavedItem> GetUserItems(Guid id);
    }
}
