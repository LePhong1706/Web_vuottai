using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_vuottai.Data;
using Web_vuottai.Models;

namespace Web_vuottai.Controllers
{
    public class VuotTaiTongQuanController : Controller
    {
        private readonly AppDbContext _db;
        public VuotTaiTongQuanController(AppDbContext db) => _db = db;
        public async Task<IActionResult> Index(string? q)
        {
            var query = _db.Set<v_VuotTai_TongQuan>().AsNoTracking();
            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.HoTen!.Contains(q));
            var data = await query.ToListAsync();
            return View(data);
        }
    }
}
