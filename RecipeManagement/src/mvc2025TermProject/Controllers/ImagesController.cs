using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment _environment;
        public ImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Images.Include(i => i.Recipe);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Recipe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            ImageAddViewModel imageVM = new ImageAddViewModel();

            ViewData["RecipeID"] = new SelectList(_context.Recipes, "Id", "Name");
            return View(imageVM);
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,AltText,RecipeID")] ImageAddViewModel imageVM, IFormFile uploadedImage)
        {
            if (ModelState.IsValid && uploadedImage != null)
            {
                (bool imageValid, string feedback) = this.ImageValid(uploadedImage);

                var recipe = await _context.Recipes.FindAsync(imageVM.RecipeID);

                bool canAddImage = recipe.Images.Count < 7;

                if (imageValid && canAddImage)
                {
                    string path = Path.Combine(this._environment.WebRootPath, @"Uploads\TempFiles");
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    string fileName = Path.GetFileName(uploadedImage.FileName);
                    string filePath = Path.Combine(path, fileName);

                    Image image = new Image
                    {
                        Description = imageVM.Description,
                        AltText = imageVM.AltText,
                        RecipeID = imageVM.RecipeID,
                        Path = filePath,
                        Name = fileName,
                        UploadedDate = DateTime.Now,
                        Status = false
                    };

                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadedImage.CopyTo(stream);
                    }

                    _context.Add(image);
                    await _context.SaveChangesAsync();

                    TempData["Confirmation"] = $"Image successfully uploaded. Waiting for approval";
                    return RedirectToAction("Index", "Recipes");
                }
                else if (!canAddImage)
                {
                    ModelState.AddModelError("", $"Recipe {recipe.Name} has reached the Image upload limit");
                }
                else
                {
                    ModelState.AddModelError("", feedback);
                }

            }
            else if (uploadedImage == null)
            {
                ModelState.AddModelError("", "You must select a file");
            }
            ViewData["RecipeID"] = new SelectList(_context.Recipes, "Id", "Name", imageVM.RecipeID);
            return View(imageVM);

        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            ViewData["RecipeID"] = new SelectList(_context.Recipes, "Id", "Instructions", image.RecipeID);
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Path,Name,Description,AltText,UploadedDate,RecipeID,Status")] Image image)
        {
            if (id != image.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.Id))
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
            ViewData["RecipeID"] = new SelectList(_context.Recipes, "Id", "Instructions", image.RecipeID);
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var image = await _context.Images
                .Include(i => i.Recipe)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image != null)
            {
                _context.Images.Remove(image);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [NonAction]
        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }

        [NonAction]
        private (bool, string) ImageValid(IFormFile image)
        {

            const int imageSizeLimit = 2 * 1024 * 1024;

            if (image.Length <= imageSizeLimit)
            {
                string fileType = image.ContentType;

                if (fileType == "image/jpg" ||fileType == "image/jpeg" || fileType == "image/png" || fileType == "image/gif")
                    return (true, "");
                else
                    return (false, $"Unsupported image type: {fileType}");
            }
            return (false, $"Image exceeds maximum size (2 MB)");
        }

    }
}
