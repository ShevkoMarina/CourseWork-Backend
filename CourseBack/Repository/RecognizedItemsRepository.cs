using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseBack.Repository
{

    public class RecognizedItemsRepository : IRecognizedItemsRepository
    {
        private readonly CourseWorkDBContext _context;

        public RecognizedItemsRepository(CourseWorkDBContext context)
        {
            _context = context;
        }

        public void AddBatchItems(IEnumerable<SavedItem> items)
        {
            _context.Items.AddRange(items);
            _context.SaveChanges();
        }

        public IReadOnlyCollection<SavedItem> GetSavedItems(Guid userId)
        {
            return _context.Items.Where(i => i.UserId.ToString() == userId.ToString()).ToList();
        }
    }
}
