using ApplicationCore.Interfaces;
using ApplicationCore.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FoodDelivery.Pages.Admin.MenuItems
{
    public class UpsertModel : PageModel
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        [BindProperty]
        public MenuItem MenuItem { get; set; }

        [BindProperty]
        public List<int> SelectedFoodTypeIds { get; set; } = new List<int>();

        //creating these for a dropdown
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> FoodTypeList { get; set; }

        public UpsertModel(UnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment; //helps find server path to wwwroot
        }
        public void OnGet(int? id)
        {
            if (id != null)//Edit mode and I also track changes (true)
            {
                MenuItem = _unitOfWork.MenuItem.Get(
                    u => u.Id == id,
                    true,
                    "MenuItemFoodTypes.FoodType"
                );

                if (MenuItem != null)
                {
                    SelectedFoodTypeIds = MenuItem.MenuItemFoodTypes?
                    .Select(m => m.FoodTypeId)
                    .ToList() ?? new List<int>();
                }
                else
                {
                    MenuItem = new MenuItem();
                    SelectedFoodTypeIds = new List<int>();
                }
            }
            else
            {
                MenuItem = new MenuItem();
                SelectedFoodTypeIds = new List<int>();
            }

            var categories = _unitOfWork.Category.List();
            var foodTypes = _unitOfWork.FoodType.List();

            CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.ID.ToString(),
                Text = c.Name
            });

            FoodTypeList = foodTypes.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name
            });
        }

        public IActionResult OnPost(int? id)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            //if the menu item is new (create)

            if (MenuItem.Id == 0)
            {
                //was there an image submitted with the form
                if (files.Count > 0)
                {
                    //create a unique identifier for the image name
                    string fileName = Guid.NewGuid().ToString();

                    //create variable to hold the path to the images/menuItems subfolder
                    var uploads = Path.Combine(webRootPath, @"images\menuitems\");

                    //get and preserve the extension type
                    var extension = Path.GetExtension(files[0].FileName);

                    //create the complete full path string
                    var fullPath = uploads + fileName + extension;

                    using var fileStream = System.IO.File.Create(fullPath);
                    files[0].CopyTo(fileStream);

                    //associate the image to the MenuItem Ovject
                    MenuItem.Image = @"\images\menuitems\" + fileName + extension;
                }

                // Set a default description if no description is provided
                if (string.IsNullOrEmpty(MenuItem.Description))
                {
                    MenuItem.Description = "No description provided.";
                }

                if (MenuItem.MenuItemFoodTypes == null)
                {
                    MenuItem.MenuItemFoodTypes = new List<MenuItemFoodType>();
                }


                foreach (var foodTypeId in SelectedFoodTypeIds)
                {
                    MenuItem.MenuItemFoodTypes.Add(new MenuItemFoodType
                    {
                        FoodTypeId = foodTypeId
                    });
                }

                //add the MenuItem to the database
                _unitOfWork.MenuItem.Add(MenuItem);
            }
            //the item exists, so update it
            else
            {
                //get the original menu item from DB table (since tracking and binding is on)
                // Make sure to include the join table and FoodType navigation
                var objFromDb = _unitOfWork.MenuItem.Get(
                    m => m.Id == MenuItem.Id,
                    true,
                    "MenuItemFoodTypes"
                );

                if (objFromDb.MenuItemFoodTypes == null)
                {
                    objFromDb.MenuItemFoodTypes = new List<MenuItemFoodType>();
                }

                //was there an image submitted with the form
                if (files.Count > 0)
                {
                    //create a unique identifier for the image name
                    string fileName = Guid.NewGuid().ToString();

                    //create variable to hold the path to the images/menuItems subfolder
                    var uploads = Path.Combine(webRootPath, @"images\menuitems\");

                    //get and preserve the extension type
                    var extension = Path.GetExtension(files[0].FileName);

                    //if the item in the database has an image
                    var imagePath = Path.Combine(webRootPath, objFromDb.Image.TrimStart('\\'));

                    //if the image exisits, physically delete
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }

                    //create the complete full path string
                    var fullPath = uploads + fileName + extension;

                    using var fileStream = System.IO.File.Create(fullPath);
                    files[0].CopyTo(fileStream);

                    //associate the image to the MenuItem Ovject
                    MenuItem.Image = @"\images\menuitems\" + fileName + extension;
                }
                else
                {
                    //add image from the existing database item to the item we're updating
                    MenuItem.Image = objFromDb.Image;
                }

                // Set a default description if no description is provided
                if (string.IsNullOrEmpty(MenuItem.Description))
                {
                    MenuItem.Description = "No description provided.";
                }

                // Remove existing join records from DB
                var existingFoodTypes = _unitOfWork.MenuItemFoodType
                    .List(mft => mft.MenuItemId == objFromDb.Id)
                    .ToList();

                if (existingFoodTypes.Any())
                {
                    _unitOfWork.MenuItemFoodType.Delete(existingFoodTypes);
                }

                // Now add the selected ones
                foreach (var foodTypeId in SelectedFoodTypeIds)
                {
                    _unitOfWork.MenuItemFoodType.Add(new MenuItemFoodType
                    {
                        MenuItemId = objFromDb.Id,
                        FoodTypeId = foodTypeId
                    });
                }

                objFromDb.Name = MenuItem.Name;
                objFromDb.Price = MenuItem.Price;
                objFromDb.Description = MenuItem.Description;
                objFromDb.CategoryId = MenuItem.CategoryId;

                //update the existing item
                _unitOfWork.MenuItem.Update(objFromDb);
            }

                //Save Changes to DB
                _unitOfWork.Commit();

                //redirect to the Menu Items page
                return RedirectToPage("./Index");
        }
    }
}
