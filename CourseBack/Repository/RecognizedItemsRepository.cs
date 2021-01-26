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

        public IReadOnlyCollection<SavedItem> GetSavedItems()
        {
             return _context.Items.ToList();
        }
    }
}
