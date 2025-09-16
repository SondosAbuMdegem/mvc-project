using Project0.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Project0
{
    public class CartService
    {
        private LibraryDbContext _context;

        public CartService(LibraryDbContext context)
        {
            _context = context;
        }

        public List<CartItem> GetCartItems()
        {
            return _context.CartItems.Include(c => c.Book).ToList();
        }
    }
}