using CourseBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        // наверно стоит проверить есть ли уже она в базе
        public void AddItem(RecognizeItemRequest item)
        {
            _context.Items.Add(new SavedItem { 
                ImageUrl = item.ImageUrl, 
                Name = item.Name, 
                Price = item.Price, 
                UserId = item.UserId, 
                WebUrl = item.WebUrl });

            _context.SaveChanges();
        }

        public void DeleteAllItems()
        {
            _context.Items.RemoveRange(_context.Items);
            _context.SaveChanges();
        }

        public IReadOnlyCollection<SavedItem> GetSavedItems(Guid userId)
        {
            return _context.Items.Where(i => i.UserId.ToString() == userId.ToString()).ToList();
        }

        public IReadOnlyCollection<SavedItem> GetUserItems(Guid id)
        {
            return _context.Items.Where(item => item.UserId == id).ToList();
        }
    }
}
