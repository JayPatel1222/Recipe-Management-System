using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvc2025TermProject.Data;
using mvc2025TermProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mvc2025TermProject.Controllers
{
    [Authorize]
    public class IngredientsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Ingredients
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Ingredients.Include(i => i.Type);
            return View(await applicationDbContext
                 .OrderBy(i => i.Name)
                .ToListAsync());
        }

        // GET: Ingredients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // GET: Ingredients/Create
        
        public IActionResult Create()
        {
            ViewData["IngredientTypeID"] = new SelectList(_context.IngredientTypes, "ID", "Name");
            return View();
        }

        // POST: Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create([Bind("Id,Name,IngredientTypeID,Details")] Ingredient ingredient)
        {
            if (ModelState.IsValid)
            {
                if (!this.IngredientExists(ingredient.Name))
                {
                    ingredient.Approved = false;
                    _context.Add(ingredient);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"The Ingredient Name {ingredient.Name} already exists");
                }
            }
            ViewData["IngredientTypeID"] = new SelectList(_context.IngredientTypes, "ID", "Name", ingredient.IngredientTypeID);
            return View(ingredient);
        }

        // GET: Ingredients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound();
            }
            ViewData["IngredientTypeID"] = new SelectList(_context.IngredientTypes, "ID", "Name", ingredient.IngredientTypeID);
            return View(ingredient);
        }

        // POST: Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IngredientTypeID,Details")] Ingredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
           
                if (this.NoChangeForIngredientName(ingredient.Name, ingredient.Id) || !this.IngredientExists(ingredient.Name))
                {
                    try
                    {
                        var tmpIngredient = await _context.Ingredients.FindAsync(id);
                        if (tmpIngredient != null)
                        {
                            if (tmpIngredient.Name == ingredient.Name && tmpIngredient.Details == ingredient.Details)
                                return RedirectToAction(nameof(Index));

                            tmpIngredient.NewName = ingredient.Name;
                            tmpIngredient.NewDetails = ingredient.Details;
                            tmpIngredient.Approved = false;
                            

                            _context.Update(tmpIngredient);
                            await _context.SaveChangesAsync();

                        }
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!IngredientExists(ingredient.Id))
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
                    ModelState.AddModelError("Name", $"The Ingredient {ingredient.Name} already Exists");
                }
            }
            ViewData["IngredientTypeID"] = new SelectList(_context.IngredientTypes, "ID", "Name", ingredient.IngredientTypeID);
            return View(ingredient);
        }

        // GET: Ingredients/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredient = await _context.Ingredients
                .Include(i => i.Type)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingredient == null)
            {
                return NotFound();
            }

            return View(ingredient);
        }

        // POST: Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredient = await _context.Ingredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.Ingredients.Remove(ingredient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(e => e.Id == id);
        }
        private bool IngredientExists(string? name)
        {
            return _context.Ingredients.Any(e => e.Name == name || e.NewName == name);
        }

        private bool NoChangeForIngredientName(string name, int id)
        {
            return _context.Ingredients.Any(i => i.Name == name && i.Id == id);
        }

    }
}
