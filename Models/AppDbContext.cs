
using App.Models.Product;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVC_1.Models_Contact;

namespace MVC_1.Models
{
public class AppDbContext:IdentityDbContext<AppUser>
{
   public DbSet<Contact> Contacts{set;get;}
   public DbSet<Category> Categories {set; get;}
   public DbSet<PostCategory> PostCategories{set;get;}
   public DbSet<Post> Posts{set;get;}

   //Tạo các bảng Sản phẩm
   public DbSet<CategoryProduct> CategoryProducts { get; set; }
   public DbSet<ProductModel> Products { get; set;}

    public DbSet<ProductCategoryProduct>  ProductCategoryProducts { get; set; }

    public DbSet<ProductPhoto> ProductPhotos { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder buider)
    {
        base.OnConfiguring(buider);
   

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Category>(entity=>entity.HasIndex(c=>c.Slug).IsUnique());

        modelBuilder.Entity<Post>(entity=>entity.HasIndex(p=>p.Slug).IsUnique());
        modelBuilder.Entity<PostCategory>(entity=>entity.HasKey(pc=>new {pc.CategoryID,pc.PostID}));

        modelBuilder.Entity<CategoryProduct>( entity => {
                entity.HasIndex(c => c.Slug)
                      .IsUnique();
            });

            modelBuilder.Entity<ProductCategoryProduct>( entity => {
                entity.HasKey( c => new {c.ProductID, c.CategoryID});
            });

            modelBuilder.Entity<ProductModel>( entity => {
                entity.HasIndex( p => p.Slug)
                      .IsUnique();
            }); 


        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            string tableName=entityType.GetTableName();
            if(tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));

            }
        }
        
    }
}
}