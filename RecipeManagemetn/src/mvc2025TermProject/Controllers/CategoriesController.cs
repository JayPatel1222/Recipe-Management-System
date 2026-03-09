using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Details")] Category category)
        {
            if (ModelState.IsValid)
            {
                if (!this.CategoryExists(category.Name))
                {
                    category.Approved = false;
                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"The category Name {category.Name} already exists");
                }
            }
            return View(category);
        }

        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Details")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
     
                if (this.NoChangeForCategoryName(category.Name, category.Id) || !this.CategoryExists(category.Name))
                {
                    try
                    {
                        
                        var tmpCategory = await _context.Categories.FindAsync(id);
                        if (tmpCategory != null)
                        {
                            if (tmpCategory.Name == category.Name && tmpCategory.Details == category.Details)
                                return RedirectToAction(nameof(Index));

                            tmpCategory.NewName = category.Name;
                            tmpCategory.NewDetails = category.Details;
                            tmpCategory.Approved = false;

                            _context.Update(tmpCategory);
                            await _context.SaveChangesAsync();

                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CategoryExists(category.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    TempData["Message"] = "A decision about your change will be made as soon as possible.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"The Category {category.Name} Already Exists");
                }
            }
            return View(category);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
        private bool CategoryExists(string? Name)
        {
            return _context.Categories.Any(c => c.Name == Name || c.NewName == Name);
        }

        private bool NoChangeForCategoryName(string name, int id)
        {
            return _context.Categories.Any(c => c.Name == name && c.Id == id);
        }

       
    }
}
