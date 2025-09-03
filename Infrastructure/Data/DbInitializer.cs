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

            if (!_db.MenuItem.Any())
            {
                var soupCat = _db.Category.First(c => c.Name == "Soup");
                var saladCat = _db.Category.First(c => c.Name == "Salad");
                var entreeCat = _db.Category.First(c => c.Name == "Entrees");
                var dessertCat = _db.Category.First(c => c.Name == "Dessert");
                var beverageCat = _db.Category.First(c => c.Name == "Beverages");

                var beefType = _db.FoodType.First(ft => ft.Name == "Beef");
                var chickenType = _db.FoodType.First(ft => ft.Name == "Chicken");
                var veggieType = _db.FoodType.First(ft => ft.Name == "Veggie");
                var seafoodType = _db.FoodType.First(ft => ft.Name == "Seafood");
                var sugarFreeType = _db.FoodType.First(ft => ft.Name == "Sugar Free");

                var menuItems = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Name = "ACC Salad",
                        Description = "Fresh garden salad with house dressing.",
                        Price = 6.50f,
                        CategoryId = saladCat.ID,
                        Image = @"\images\menuitems\ACC_Salad.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = veggieType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Caramel Brownie",
                        Description = "Rich chocolate brownie topped with caramel drizzle.",
                        Price = 4.50f,
                        CategoryId = dessertCat.ID,
                        Image = @"\images\menuitems\CaramelBrownie.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = sugarFreeType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Chicken Enchilada Chili",
                        Description = "Spicy chili with chicken and enchilada flavors.",
                        Price = 9.99f,
                        CategoryId = soupCat.ID,
                        Image = @"\images\menuitems\ChickenEnchiladaChili.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = chickenType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Chipotle Steak",
                        Description = "Grilled steak with smoky chipotle seasoning.",
                        Price = 14.99f,
                        CategoryId = entreeCat.ID,
                        Image = @"\images\menuitems\ChipotleSteak.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = beefType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Coke",
                        Description = "Classic Coca-Cola.",
                        Price = 1.99f,
                        CategoryId = beverageCat.ID,
                        Image = @"\images\menuitems\Coke.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Creme Brulee",
                        Description = "Vanilla custard topped with caramelized sugar.",
                        Price = 5.50f,
                        CategoryId = dessertCat.ID,
                        Image = @"\images\menuitems\CremeBrulee.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Grilled Cheese",
                        Description = "Melted cheese in toasted bread.",
                        Price = 5.99f,
                        CategoryId = entreeCat.ID,
                        Image = @"\images\menuitems\GrilledCheese.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = veggieType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Honest Tea",
                        Description = "Refreshing bottled tea.",
                        Price = 2.50f,
                        CategoryId = beverageCat.ID,
                        Image = @"\images\menuitems\HonestTea.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Lobster Soup",
                        Description = "Creamy soup with fresh lobster.",
                        Price = 12.99f,
                        CategoryId = soupCat.ID,
                        Image = @"\images\menuitems\LobsterSoup.jpeg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = seafoodType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Mango Berry Smoothie",
                        Description = "Smoothie made with mango and berries.",
                        Price = 3.99f,
                        CategoryId = beverageCat.ID,
                        Image = @"\images\menuitems\MangoBerry.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = veggieType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Sprite",
                        Description = "Refreshing lemon-lime soda.",
                        Price = 1.99f,
                        CategoryId = beverageCat.ID,
                        Image = @"\images\menuitems\Sprite.jpg"
                    },
                    new MenuItem
                    {
                        Name = "Turkey Avocado Sandwich",
                        Description = "Turkey with fresh avocado on whole grain bread.",
                        Price = 10.99f,
                        CategoryId = entreeCat.ID,
                        Image = @"\images\menuitems\TurkeyAvacado.jpg",
                        MenuItemFoodTypes = new List<MenuItemFoodType>
                        {
                            new MenuItemFoodType { FoodTypeId = chickenType.Id }
                        }
                    },
                    new MenuItem
                    {
                        Name = "Zero Water",
                        Description = "Pure bottled water.",
                        Price = 1.50f,
                        CategoryId = beverageCat.ID,
                        Image = @"\images\menuitems\ZeroWater.jpg"
                    }
                };

                _db.MenuItem.AddRange(menuItems);
                _db.SaveChanges();
            }


        }
    }
}
