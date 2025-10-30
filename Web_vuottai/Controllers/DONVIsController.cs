using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_vuottai.Data;
using Web_vuottai.Models;

namespace Web_vuottai.Controllers
{
    public class DONVIsController : Controller
    {
        private readonly AppDbContext _context;

        public DONVIsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DONVIs
        public async Task<IActionResult> Index()
        {
            return View(await _context.DONVIs.ToListAsync());
        }

        // GET: DONVIs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dONVI = await _context.DONVIs
                .FirstOrDefaultAsync(m => m.DonViId == id);
            if (dONVI == null)
            {
                return NotFound();
            }

            return View(dONVI);
        }

        // GET: DONVIs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DONVIs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DonViId,MaDonVi,TenDonVi")] DONVI dONVI)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dONVI);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dONVI);
        }

        // GET: DONVIs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dONVI = await _context.DONVIs.FindAsync(id);
            if (dONVI == null)
            {
                return NotFound();
            }
            return View(dONVI);
        }

        // POST: DONVIs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DonViId,MaDonVi,TenDonVi")] DONVI dONVI)
        {
            if (id != dONVI.DonViId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dONVI);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DONVIExists(dONVI.DonViId))
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
            return View(dONVI);
        }

        // GET: DONVIs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dONVI = await _context.DONVIs
                .FirstOrDefaultAsync(m => m.DonViId == id);
            if (dONVI == null)
            {
                return NotFound();
            }

            return View(dONVI);
        }

        // POST: DONVIs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dONVI = await _context.DONVIs.FindAsync(id);
            if (dONVI != null)
            {
                _context.DONVIs.Remove(dONVI);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DONVIExists(int id)
        {
            return _context.DONVIs.Any(e => e.DonViId == id);
        }
    }
}
