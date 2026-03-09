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
    //[Authorize(Roles = "admin")]
    public class IngredientTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IngredientTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: IngredientTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.IngredientTypes.ToListAsync());
        }

        // GET: IngredientTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredientType = await _context.IngredientTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ingredientType == null)
            {
                return NotFound();
            }

            return View(ingredientType);
        }

        // GET: IngredientTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IngredientTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] IngredientType ingredientType)
        {
            if (ModelState.IsValid)
            {
                if (!this.IngredientTypeExists(ingredientType.Name))
                {
                    _context.Add(ingredientType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Name", $"The Ingredient Type Name {ingredientType.Name} already exists");
                }
            }
            return View(ingredientType);
        }

        // GET: IngredientTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredientType = await _context.IngredientTypes.FindAsync(id);
            if (ingredientType == null)
            {
                return NotFound();
            }
            return View(ingredientType);
        }

        // POST: IngredientTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name")] IngredientType ingredientType)
        {
            if (id != ingredientType.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (!this.IngredientTypeExists(ingredientType.Name))
                {
                    try
                    {
                        _context.Update(ingredientType);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!IngredientTypeExists(ingredientType.ID))
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
                else
                {
                    if (!this.IngredientTypeNameExists(ingredientType.Name, ingredientType.ID))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    ModelState.AddModelError("Name", $"The Ingredient Type {ingredientType.Name} already Exists");
                }
            }
            return View(ingredientType);
        }

        // GET: IngredientTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingredientType = await _context.IngredientTypes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (ingredientType == null)
            {
                return NotFound();
            }

            return View(ingredientType);
        }

        // POST: IngredientTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingredientType = await _context.IngredientTypes.FindAsync(id);
            if (ingredientType != null)
            {
                _context.IngredientTypes.Remove(ingredientType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngredientTypeExists(int id)
        {
            return _context.IngredientTypes.Any(e => e.ID == id);
        }
        private bool IngredientTypeExists(string? Name)
        {
            return _context.IngredientTypes.Any(e => e.Name == Name);
        }
        private bool IngredientTypeNameExists(string? Name,int id)
        {
            return _context.IngredientTypes.Any(e => e.Name == Name && e.ID != id);
        }

    }
}
