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
    public class CHUCVUsController : Controller
    {
        private readonly AppDbContext _context;

        public CHUCVUsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CHUCVUs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CHUCVUs.ToListAsync());
        }

        // GET: CHUCVUs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cHUCVU = await _context.CHUCVUs
                .FirstOrDefaultAsync(m => m.ChucVuId == id);
            if (cHUCVU == null)
            {
                return NotFound();
            }

            return View(cHUCVU);
        }

        // GET: CHUCVUs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CHUCVUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChucVuId,TenChucVu,TyLeDinhMuc")] CHUCVU cHUCVU)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cHUCVU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cHUCVU);
        }

        // GET: CHUCVUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cHUCVU = await _context.CHUCVUs.FindAsync(id);
            if (cHUCVU == null)
            {
                return NotFound();
            }
            return View(cHUCVU);
        }

        // POST: CHUCVUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChucVuId,TenChucVu,TyLeDinhMuc")] CHUCVU cHUCVU)
        {
            if (id != cHUCVU.ChucVuId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cHUCVU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CHUCVUExists(cHUCVU.ChucVuId))
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
            return View(cHUCVU);
        }

        // GET: CHUCVUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cHUCVU = await _context.CHUCVUs
                .FirstOrDefaultAsync(m => m.ChucVuId == id);
            if (cHUCVU == null)
            {
                return NotFound();
            }

            return View(cHUCVU);
        }

        // POST: CHUCVUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cHUCVU = await _context.CHUCVUs.FindAsync(id);
            if (cHUCVU != null)
            {
                _context.CHUCVUs.Remove(cHUCVU);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CHUCVUExists(int id)
        {
            return _context.CHUCVUs.Any(e => e.ChucVuId == id);
        }
    }
}
