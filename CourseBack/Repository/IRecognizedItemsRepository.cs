using CourseBack.Models;
using System;
using System.Collections.Generic;

namespace CourseBack.Repository
{
    public interface IRecognizedItemsRepository
    {
        public IReadOnlyCollection<SavedItem> GetSavedItems(Guid userId);
        public void AddBatchItems(IEnumerable<SavedItem> items);
    }
}
