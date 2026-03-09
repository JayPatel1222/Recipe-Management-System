using EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<CustomUser> _userManager;
        private readonly IEmailSender _emailSender;
        private IWebHostEnvironment _environment;

        public RecipesController(ApplicationDbContext context, UserManager<CustomUser> userManager,IEmailSender emailSender, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _environment = environment;
        }

        // GET: Recipes
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            string? searchFilter,int? categoryId,DateTime? dateFrom,DateTime? dateTo,int? pageNum)
        {
            
            //const int PageSize = 5;       // When searching by Category, it should display 10 records at the time 
            int PageSize = 5;
            if (pageNum == null)
            {
                pageNum = 1;
            }

            ViewBag.Categories = _context.Categories.Select(cat => new
            {
                cat.Id,
                cat.Name,
                NameStatus = cat.Approved ? cat.Name : $"{cat.Name} --Not Approved--"
            }).OrderBy(c => c.Name).ToList();
            

            var browsedRecipes =  _context.Recipes.Select(r => r);
            var allRecipes =  _context.Recipes.Select(r => r);
            IQueryable<Recipe> query = _context.Recipes;

            bool showbackBtn = false;

            if (!string.IsNullOrEmpty(searchFilter))
            {
                query = query.Where(r => r.Name.Contains(searchFilter) 
                                    || r.RecipeIngredients.Any(i => i.Ingredient.Name == searchFilter)); // or if contains ingredient 

                ViewData["searchByName"] = searchFilter;
                showbackBtn = true;
            }

            if (categoryId != null)
            {
                query = query.Where(r => r.CategoryId == categoryId);
                ViewBag.categoryId = categoryId;
                PageSize = 10;
                showbackBtn = true;
            }

            if (dateFrom != null && dateTo != null)
            {
                var toDate = dateTo.Value.Date.AddDays(1);
                query = query.Where(r => r.CreationDate >= dateFrom && r.CreationDate < toDate);
                ViewBag.dateFrom = dateFrom;
                ViewBag.dateTo = dateTo;
                showbackBtn = true;
            }

            browsedRecipes = query;

            if (showbackBtn)
            {
                
                if (browsedRecipes.Count() == 0)
                {
                    ViewBag.NoRecipeFound = "Whoopss !!!  No Recipe Found";
                    ViewData["searchByName"] = searchFilter;
                    ViewBag.categoryId = categoryId;
                    ViewBag.dateFrom = dateFrom;
                    ViewBag.dateTo = dateTo;
                    ViewBag.showbkBtn = showbackBtn;
                    ViewBag.recipeCount = browsedRecipes.Count();
                    return View(await Pagination<Recipe>.CreateAsync(browsedRecipes.OrderBy(r => r.Name), (int)pageNum, PageSize));
                }
                else
                {
                    ViewBag.recipeCount = browsedRecipes.Count();
                    ViewBag.showbkBtn = showbackBtn;
                    return View(await Pagination<Recipe>.CreateAsync(browsedRecipes.OrderBy(r => r.Name),(int)pageNum,PageSize));
                }
            }

            // Move images from temp - uploaded folder if Image Status = True (Approved)
            var recipeList = _context.Recipes.Select(r => r); 
            foreach (var recipe in recipeList)
            {
                List<Image> images = recipe.Images != null ? recipe.Images.ToList() : new List<Image>();
                foreach (Image image in images)
                {
                    if (image.Status)
                        MoveImage(image);
                }
            }
            await _context.SaveChangesAsync(); 


            ViewBag.showbkBtn = showbackBtn;
            ViewBag.recipeCount = allRecipes.Count();

            // Added Include() to load Images collection 

            return View( await Pagination<Recipe>.CreateAsync(allRecipes.Include(r => r.Images).OrderBy(r => r.Name), (int)pageNum, 10));
            //return View(await Pagination<Recipe>.CreateAsync(recipeList, (int)pageNumber, PageSize));
        }
        // POST: Recipes/Share
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Share(string email, int recipeId, string message, string subject)
        {
            

            if (isEmailValid(email) && recipeId > 0)
            {
                Recipe recipe = _context.Recipes.Find(recipeId);
                var shareRecpe = new EmailMessage(

                    new string[] { email },
                    $"{(subject == null ? "The Recipe Town - Shared Recipe" : $"{subject}")}",
                    $"{(message == null ? "A recipe was share to you by one of our Website Visitor" : $"{message.ToUpper()}")}\n" +
                    $"Link to Recipe : {Request.Scheme}://{Request.Host}/Recipes/Details/{recipe.Id}"
                    );
                _emailSender.SendEmail(shareRecpe);

                TempData["ShareSuccess"] = "1";
            }
            else
            {
                TempData["emailRequired"] = "Email require";
                
            }

                string url = Request.Headers["Referer"].ToString();

            if (!url.Contains("?"))
                url += "?shared=1";
            else
                url += "&shared=1";

            return Redirect(url);
        }
        // POST: Recipes/Report
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Report(string email, int recipeId, string reason, string fullName)
        {
            if(!string.IsNullOrEmpty(fullName)
                && recipeId > 0
                && isEmailValid(email)
                && !string.IsNullOrEmpty(reason)
                )
            {
                Recipe recipe = _context.Recipes.Find(recipeId);
                var shareRecpe = new EmailMessage(

                    new string[] { "admin@TherecipeTown.ca" },
                    $"User has reported a recipe",
                    $"Reported By : {fullName.ToUpper()}\n" +
                    $"Reason : {reason}\n"+
                    $"Email of reporter: {email}\n"+
                    $"Link of the reported Recipe:{Request.Scheme}://{Request.Host}/Recipes/Details/{recipe.Id}" 
                   
                    );
                _emailSender.SendEmail(shareRecpe);
                TempData["ReportSuccess"] = "1";
            }

            
            string url = Request.Headers["Referer"].ToString();

            if (!url.Contains("?"))
                url += "?reported=1";
            else
                url += "&reported=1";

            return Redirect(url);
        }

        // GET: Recipes/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id,string email, int recipeId, string message, string subject)
        {
            ViewBag.ShowPrintbtn = true;
            if (id == null)
            {
                return NotFound();
            }
            if (email != null && recipeId > 0)
            {
                Recipe recipeToEmail = _context.Recipes.Find(recipeId);
                var shareRecpe = new EmailMessage(

                    new string[] { email },
                    $"{subject}",
                    $"{message}\nLink to Recipe : {Request.Scheme}://{Request.Host}/Recipes/Details/{recipeToEmail.Id}"
                    );
                _emailSender.SendEmail(shareRecpe);
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Images)
                .Include(r => r.Nutrition)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recipe == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null && recipe.User != null)
                ViewBag.IsOwner = recipe.User.Equals(user);
            else
                ViewBag.IsOwner = false;
            return View(recipe);
        }

        // GET: Recipes/Create
        public IActionResult Create()
        {
            ViewBag.IngredientTypes = new SelectList(_context.IngredientTypes.OrderBy(i => i.Name), "ID", "Name");

            ViewBag.Categories = _context.Categories.Select(cat => new
            {
                cat.Id,
                cat.Name,
                NameStatus = cat.Approved ? cat.Name : $"{cat.Name} --Not Approved--"
            }).OrderBy(c => c.Name).ToList();

            ViewBag.Ingredients = _context.Ingredients.Select(ing => new
            {
                ing.Id,
                ing.Name,
                NameStatus = ing.Approved ? ing.Name : $"{ing.Name} --Not Approved--",
                IngTypeId = ing.IngredientTypeID

            }).OrderBy(i => i.Name).ToList();

            return View(new RecipeCreateViewModel());
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CategoryId,Instructions,Tips,PrepTime,CookTime,Temperature,Servings,SelectedIngredients,Nutrition")] RecipeCreateViewModel recipeVM)
        {

            if (ModelState.IsValid && this.HasIngredients(recipeVM))
            {
                if (!this.RecipeExists(recipeVM.Name))
                {
                    var user = await _userManager.GetUserAsync(User);

                    StandardUser? currentUser = user as StandardUser;

                    Recipe recipe = new Recipe
                    {
                        User = currentUser,

                        Name = recipeVM.Name,
                        CategoryId = recipeVM.CategoryId,
                        Instructions = recipeVM.Instructions,
                        Tips = recipeVM.Tips,
                        PrepTime = recipeVM.PrepTime,
                        CookTime = recipeVM.CookTime,
                        Temperature = recipeVM.Temperature,
                        Servings = recipeVM.Servings,
                        CreationDate = DateTime.Now,
                        
                    };
                    if (recipeVM.Nutrition != null &&
                        (
                            (recipeVM.Nutrition.Calories ?? 0) > 0 ||
                            (recipeVM.Nutrition.Protein ?? 0) > 0 ||
                            (recipeVM.Nutrition.Carbs ?? 0) > 0 ||
                            (recipeVM.Nutrition.Fat ?? 0) > 0
                        ))
                    {
                        recipe.Nutrition = recipeVM.Nutrition;
                    }
                    _context.Add(recipe);
                    
                    await _context.SaveChangesAsync();

                    foreach (var ingredient in recipeVM.SelectedIngredients)
                    {
                        if (ingredient.Quantity > 0)
                        {
                            RecipeIngredient addedIngredient = new RecipeIngredient
                            {
                                IngredientId = ingredient.IngredientID,
                                RecipeId = recipe.Id,
                                Quantity = ingredient.Quantity,
                                Unit = ingredient.Unit
                            };
                            _context.Add(addedIngredient);
                        }

                    }
                    await _context.SaveChangesAsync();
                    
                    string status = "Draft"; // only draft allow for now - modify later
                    TempData["Confirmation"] = $"Recipe has been saved. Status: {status} ";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"The Recipe Name {recipeVM.Name} already exists");
                }
            }


            ViewBag.IngredientTypes = new SelectList(_context.IngredientTypes, "ID", "Name");
            ViewBag.Categories = _context.Categories.Select(cat => new
            {
                cat.Id,
                cat.Name,
                NameStatus = cat.Approved ? cat.Name : $"{cat.Name} --Not Approved--"
            }).OrderBy(c => c.Name).ToList();

            ViewBag.Ingredients = _context.Ingredients.Select(ing => new
            {
                ing.Id,
                ing.Name,
                NameStatus = ing.Approved ? ing.Name : $"{ing.Name} --Not Approved--",
                IngTypeId = ing.IngredientTypeID

            }).OrderBy(i => i.Name).ToList();
            return View(recipeVM);
        }

        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", recipe.CategoryId);
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CategoryId,Instructions,Tips,PrepTime,CookTime,Temperature,Servings")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", recipe.CategoryId);
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }
        private bool RecipeExists(string? name)
        {
            return _context.Recipes.Any(e => e.Name == name);
        }
        private bool HasIngredients(RecipeCreateViewModel recipeVM)
        {
            return (recipeVM != null && recipeVM.SelectedIngredients != null && recipeVM.SelectedIngredients.Count > 0);
        }
        [NonAction]
        private void MoveImage(Image image)
        {
            if (image == null) return;

            string path = Path.Combine(this._environment.WebRootPath, @"Uploads\UploadedImages", image.RecipeID.ToString());

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string sourceDir = Path.Combine(this._environment.WebRootPath, @"Uploads\TempFiles");
            string currentPath = Path.Combine(sourceDir, image.Name);

            string filePath = Path.Combine(path, image.Name);

            if (System.IO.File.Exists(currentPath))
            {
                try
                {
                    System.IO.File.Move(currentPath, filePath);

                    string urlPath = $"/Uploads/UploadedImages/{image.RecipeID}/{image.Name}";

                    image.Path = urlPath;
                }
                catch (System.IO.IOException ex)
                {
                    return;
                }
            }

        }

        [NonAction]
        private bool isEmailValid(string email)
        {
            if (email == null || email.Trim() == "") return false; 

            try
            {
                var newEmail = new MailAddress(email);
                return newEmail.Address == email; 
            } 
            catch (Exception ex)
            {
                return false; 
            }
        }
    }
}
