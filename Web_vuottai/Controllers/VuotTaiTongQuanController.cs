using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_vuottai.Data;
using Web_vuottai.Models;

namespace Web_vuottai.Controllers
{
    public class VuotTaiTongQuanController : Controller
    {
        private readonly AppDbContext _db;
        public VuotTaiTongQuanController(AppDbContext db) => _db = db;
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var query = from v in _db.v_VuotTai_TongQuans select v;
            
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(s => s.HoTen.Contains(searchString));
            }

            var data = await query.AsNoTracking().ToListAsync();
            return View(data);
        }
    }
}
