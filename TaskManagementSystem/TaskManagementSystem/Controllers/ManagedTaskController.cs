using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem.Models;

namespace TaskManagementSystem
{
    public class ManagedTaskController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagedTaskController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ManagedTask
        public async Task<IActionResult> Index()
        {
            return View(await _context.ManagedTask.ToListAsync());
        }

        // GET: ManagedTask/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var managedTask = await _context.ManagedTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (managedTask == null)
            {
                return NotFound();
            }

            return View(managedTask);
        }

        // GET: ManagedTask/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ManagedTask/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Done,CreatedAt,LastUpdate")] ManagedTask managedTask)
        {
            if (ModelState.IsValid)
            {
                managedTask.Id = Guid.NewGuid();
                _context.Add(managedTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(managedTask);
        }

        // GET: ManagedTask/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var managedTask = await _context.ManagedTask.FindAsync(id);
            if (managedTask == null)
            {
                return NotFound();
            }
            return View(managedTask);
        }

        // POST: ManagedTask/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Done,CreatedAt,LastUpdate")] ManagedTask managedTask)
        {
            if (id != managedTask.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(managedTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManagedTaskExists(managedTask.Id))
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
            return View(managedTask);
        }

        // GET: ManagedTask/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var managedTask = await _context.ManagedTask
                .FirstOrDefaultAsync(m => m.Id == id);
            if (managedTask == null)
            {
                return NotFound();
            }

            return View(managedTask);
        }

        // POST: ManagedTask/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var managedTask = await _context.ManagedTask.FindAsync(id);
            if (managedTask != null)
            {
                _context.ManagedTask.Remove(managedTask);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ManagedTaskExists(Guid id)
        {
            return _context.ManagedTask.Any(e => e.Id == id);
        }
    }
}
