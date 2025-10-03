using BigShotCore.Data.Dtos;
using BigShotCore.Data.Models;
using BigShotCore.Extensions;
using BigShotCore.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BigShotApi.Infrastructure
{
    public static class DbSeeder
    {
        public static void SeedUsers(AppDbContext db)
        {

            // Seed default roles
           

            if (!db.Roles.Any())
            {
                db.Roles.AddRange(
                    new AppRole {Name = "Admin" },
                    new AppRole {Name = "Customer" }
                );


                db.SaveChanges();

                Console.WriteLine("Roles Added");
            }


            if (!db.Users.Any())
            {
                var adminRole = db.Roles.FirstOrDefault(t => t.Name == "Admin");
                var userRole = db.Roles.FirstOrDefault(t => t.Name == "Customer");

                var admin = new AppUser
                {
                    UserName = "Admin User",
                    Role = adminRole,
                    ApiKey = "ef1a74e8-5fdd-48d5-9439-4386d60bd522",
                    Email = "TestAdmin@gmail.com"
                };

                var customer = new AppUser
                {
                    UserName = "Test Customer",
                    Role = userRole,
                    ApiKey = "2fb5c9f8-c7dd-48bf-b4f3-d89c4fa557fa",
                    Email = "TestUser@gmail.com"
                };

                db.Users.AddRange(admin, customer);
                db.SaveChanges();

                Console.WriteLine("🔑 Admin API Key: " + admin.ApiKey);
                Console.WriteLine("🔑 Customer API Key: " + customer.ApiKey);
            }
        }

        public static void SeedProducts(AppDbContext db)
        {
            if (!db.Products.Any())
            {
                var products = new List<Product>
                {
                    new CreateProductDto(
                        Name : "Wireless Headphones",
                        ShortDescription : "Noise-cancelling over-ear headphones.",
                        LongDescriptionMarkdown : "### Features\n- Noise cancellation\n- 20 hours battery life\n- Bluetooth 5.0",
                        Price : 99.99,
                        InStock : 50,
                        ImageUrl : "https://imgs.search.brave.com/7cCFj_ldTKdU9p9ZLYSeFNiJi0NVZEdBzpxIG9QY9uE/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9wbHVz/LnVuc3BsYXNoLmNv/bS9wcmVtaXVtX3Bo/b3RvLTE2NzgwOTk5/NDA5NjctNzNmZTMw/NjgwOTQ5P2ZtPWpw/ZyZxPTYwJnc9MzAw/MCZpeGxpYj1yYi00/LjEuMCZpeGlkPU0z/d3hNakEzZkRCOE1I/eHpaV0Z5WTJoOE5Y/eDhhR1ZoWkhCb2Iy/NWxjM3hsYm53d2ZI/d3dmSHg4TUE9PQ",
                        Rating: 4.5
                    ).ToEntity(),
                    new CreateProductDto(
                        Name:"Gaming Mouse",
                        ShortDescription:"RGB wired gaming mouse with 7 buttons.",
                        LongDescriptionMarkdown:"### Specs\n- 7200 DPI\n- RGB lighting\n- Ergonomic design",
                        Price :49.99,
                        InStock :100,
                        ImageUrl :"https://imgs.search.brave.com/0NvL9HeZOw9Bag8JAhj6VL26MtF3K_x-SkCUx4viRn0/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9pbWcu/ZW5kZ2FtZWdlYXIu/Y29tL2ltYWdlcy9H/QU1PLTg2NC9jYWVl/NDVkMTI4ZThlNWMw/YWZmYzM1MmU5Y2Zj/NWJkZi5qcGc",
                        Rating : 4.2
                    ).ToEntity(),
                    new CreateProductDto(
                        Name:"Mechanical Keyboard",
                        ShortDescription: "Blue switch mechanical keyboard.",
                        LongDescriptionMarkdown: "### Highlights\n- 104 keys\n- Anti-ghosting\n- Customizable lighting",
                        Price: 79.99,
                        InStock: 75,
                        ImageUrl: "https://imgs.search.brave.com/CykOnB25yPlYl_pVNDpyQVpWzONxzwZLRjCB51d5IlE/rs:fit:500:0:1:0/g:ce/aHR0cHM6Ly9hdHRh/Y2tzaGFyay5jb20v/Y2RuL3Nob3AvZmls/ZXMvYjkyYTkyZGQ2/ZWY1YmYxZmFkOGY4/ZWNjNTQwNThjYmIu/anBnP3Y9MTc1NTY5/MDYyOCZ3aWR0aD0x/NjAw",
                        Rating: 4.7
                    ).ToEntity()
                };

                db.Products.AddRange(products);
                db.SaveChanges();

                Console.WriteLine("✅ Seeded sample products.");
            }
        }
    }
}
