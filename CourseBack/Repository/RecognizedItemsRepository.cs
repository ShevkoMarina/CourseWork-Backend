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

        // наверно стоит проверить есть ли уже она в базе
        public string AddItem(SavedItemRequest item)
        {
            try
            {
                _context.Items.Add(new SavedItem { 
                    ImageUrl = item.ImageUrl, 
                    Name = item.Name, 
                    Price = item.Price, 
                    UserId = item.UserId, 
                    WebUrl = item.WebUrl });

                _context.SaveChanges();

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // мб здесь ничего ловить не надо. надо бросать исключения специфизированные и обрабатывать в сервисе
        public IEnumerable<SavedItem> GetSavedItems()
        {
            try
            {
                return _context.Items.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public (string Error, string Url) UploadToBlob(UserPhotoRequest photo)
        {
            throw new NotImplementedException();
        }
    }

}
