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
    public class GIANGVIENsController : Controller
    {
        private readonly AppDbContext _context;

        public GIANGVIENsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: GIANGVIENs
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.GIANGVIENs.Include(g => g.ChucVu).Include(g => g.DonVi);
            return View(await appDbContext.ToListAsync());
        }

        // GET: GIANGVIENs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gIANGVIEN = await _context.GIANGVIENs
                .Include(g => g.ChucVu)
                .Include(g => g.DonVi)
                .FirstOrDefaultAsync(m => m.GiangVienId == id);
            if (gIANGVIEN == null)
            {
                return NotFound();
            }

            return View(gIANGVIEN);
        }

        // GET: GIANGVIENs/Create
        public IActionResult Create()
        {
            ViewData["ChucVuId"] = new SelectList(_context.CHUCVUs, "ChucVuId", "ChucVuId");
            ViewData["DonViId"] = new SelectList(_context.DONVIs, "DonViId", "DonViId");
            return View();
        }

        // POST: GIANGVIENs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GiangVienId,MaGV,HoTen,DonViId,ChucVuId")] GIANGVIEN gIANGVIEN)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gIANGVIEN);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChucVuId"] = new SelectList(_context.CHUCVUs, "ChucVuId", "ChucVuId", gIANGVIEN.ChucVuId);
            ViewData["DonViId"] = new SelectList(_context.DONVIs, "DonViId", "DonViId", gIANGVIEN.DonViId);
            return View(gIANGVIEN);
        }

        // GET: GIANGVIENs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gIANGVIEN = await _context.GIANGVIENs.FindAsync(id);
            if (gIANGVIEN == null)
            {
                return NotFound();
            }
            ViewData["ChucVuId"] = new SelectList(_context.CHUCVUs, "ChucVuId", "ChucVuId", gIANGVIEN.ChucVuId);
            ViewData["DonViId"] = new SelectList(_context.DONVIs, "DonViId", "DonViId", gIANGVIEN.DonViId);
            return View(gIANGVIEN);
        }

        // POST: GIANGVIENs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("GiangVienId,MaGV,HoTen,DonViId,ChucVuId")] GIANGVIEN gIANGVIEN)
        {
            if (id != gIANGVIEN.GiangVienId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gIANGVIEN);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GIANGVIENExists(gIANGVIEN.GiangVienId))
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
            ViewData["ChucVuId"] = new SelectList(_context.CHUCVUs, "ChucVuId", "ChucVuId", gIANGVIEN.ChucVuId);
            ViewData["DonViId"] = new SelectList(_context.DONVIs, "DonViId", "DonViId", gIANGVIEN.DonViId);
            return View(gIANGVIEN);
        }

        // GET: GIANGVIENs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gIANGVIEN = await _context.GIANGVIENs
                .Include(g => g.ChucVu)
                .Include(g => g.DonVi)
                .FirstOrDefaultAsync(m => m.GiangVienId == id);
            if (gIANGVIEN == null)
            {
                return NotFound();
            }

            return View(gIANGVIEN);
        }

        // POST: GIANGVIENs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gIANGVIEN = await _context.GIANGVIENs.FindAsync(id);
            if (gIANGVIEN != null)
            {
                _context.GIANGVIENs.Remove(gIANGVIEN);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GIANGVIENExists(int id)
        {
            return _context.GIANGVIENs.Any(e => e.GiangVienId == id);
        }
    }
}
