using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public void Initialize()
        {
            _db.Database.EnsureCreated();

            //migrations if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception)
            {

            }
            if (_db.Category.Any())
            {
                return; //DB has been seeded
            }

            //create roles if they are not created
            //SD is a “Static Details” class we will create in Utility to hold constant strings for Roles

            _roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.DriverRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.KitchenRole)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();

            //Create at least one "Super Admin" or “Admin”.  Repeat the process for other users you want to seed

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "christianmartin@mail.weber.edu",
                Email = "christianmartin@mail.weber.edu",
                FirstName = "Christian",
                LastName = "Martin",
                PhoneNumber = "5555555555",
            }, "Admin123*").GetAwaiter().GetResult();

            ApplicationUser user = _db.ApplicationUser.FirstOrDefault(u => u.Email == "christianmartin@mail.weber.edu");

            _userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult();


            var Category = new List<Category>
                {
                new Category{ Name = "Soup", DisplayOrder = 1},
                new Category{ Name = "Salad", DisplayOrder=2},
                new Category{ Name = "Entrees", DisplayOrder=3},
                new Category{ Name = "Dessert", DisplayOrder=4},
                new Category{ Name = "Beverages", DisplayOrder=5}
                };
            foreach (var c in Category)
            {
                _db.Category.Add(c);
            }
            _db.SaveChanges();

            var foodtype = new List<FoodType>
                {
                new FoodType{ Name = "Beef"},
                new FoodType{ Name = "Chicken"},
                new FoodType{ Name = "Veggie"},
                new FoodType{ Name = "Sugar Free"},
                new FoodType{ Name = "Seafood"}
                };
            foreach (var f in foodtype)
            {
                _db.FoodType.Add(f);
            }

            _db.SaveChanges();
        }
    }
}
