using System.Data.Entity;

namespace Project0.Models
{
    public class LibraryDbContext : DbContext
    {
        public LibraryDbContext() : base("LibraryDbContext")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Waitlist> Waitlist { get; set; }
        public DbSet<BorrowedBook> BorrowedBooks { get; set; }
        public DbSet<BookRentals> BookRentals { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // تعيين أسماء الجداول
            modelBuilder.Entity<Book>().ToTable("Book");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Waitlist>().ToTable("Waitlist");
            modelBuilder.Entity<BorrowedBook>().ToTable("BorrowedBook");
            modelBuilder.Entity<BookRentals>().ToTable("BookRentals");
            modelBuilder.Entity<CartItem>().ToTable("CartItems");

            // العلاقة بين CartItem و Book
            modelBuilder.Entity<CartItem>()
                .HasRequired(c => c.Book)
                .WithMany()
                .HasForeignKey(c => c.BookId);

            // العلاقة بين CartItem و User
            modelBuilder.Entity<CartItem>()
                .HasRequired(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.Username)
                .WillCascadeOnDelete(false);

            // العلاقة بين Waitlist و Book
            modelBuilder.Entity<Waitlist>()
                .HasRequired(w => w.Book)
                .WithMany()
                .HasForeignKey(w => w.BookId);

            // العلاقة بين Waitlist و User
            modelBuilder.Entity<Waitlist>()
                .HasRequired(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.Username);

            // العلاقة بين Order و User
            modelBuilder.Entity<Order>()
                .HasRequired(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.Username)
                .WillCascadeOnDelete(false);

            // العلاقة بين OrderItem و Order
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems) // لكل طلب عدة عناصر طلب
                .HasForeignKey(oi => oi.OrderId);

            // العلاقة بين OrderItem و Book
            modelBuilder.Entity<OrderItem>()
                .HasRequired(oi => oi.Book)
                .WithMany() // لكل كتاب عدة عناصر طلب
                .HasForeignKey(oi => oi.BookId);






            modelBuilder.Entity<Rating>()
                .HasRequired(r => r.Book)  // يجب أن يكون لكل Rating كتاب واحد
                .WithMany(b => b.Rating) // قد يحتوي الكتاب على عدة تقييمات
                .HasForeignKey(r => r.BookId); // ربطها مع BookId في Rating




            base.OnModelCreating(modelBuilder);
        }
    }
}