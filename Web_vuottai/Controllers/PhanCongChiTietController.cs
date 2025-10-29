using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Web_vuottai.Data;
using Web_vuottai.Models;

namespace Web_vuottai.Controllers
{
    public class PhanCongChiTietController : Controller
    {
        private readonly AppDbContext _db;
        public PhanCongChiTietController(AppDbContext db) => _db = db;

        // LIST với filter Năm/Kỳ
        public async Task<IActionResult> Index(string? namHoc, int? hocKy)
        {
            // 1) Lấy danh sách năm học từ CSDL (từ bảng TKB)
            var namHocs = await _db.THOIKHOABIEUs  // entity do Scaffold sinh ra
                .Select(t => t.NamHoc)
                .Distinct()
                .OrderByDescending(x => x)   // mới nhất trên cùng
                .ToListAsync();

            // 2) Chọn mặc định nếu chưa chọn
            if (string.IsNullOrWhiteSpace(namHoc) && namHocs.Count > 0)
                namHoc = namHocs.First();

            // 3) Query view: nếu không chọn học kỳ -> hiện cả năm (HK1 & HK2)
            var q = _db.Set<v_PhanCong_ChiTiet>()  // keyless view
                       .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(namHoc))
                q = q.Where(x => x.NamHoc == namHoc);

            if (hocKy.HasValue)                 // chỉ khi có giá trị 1 hoặc 2
                q = q.Where(x => x.HocKy == hocKy.Value);

            var data = await q
                .OrderBy(x => x.HoTen)
                .ThenBy(x => x.NgayHoc)
                .ToListAsync();

            ViewBag.NamHoc = namHoc;
            ViewBag.HocKy = hocKy;            // null = cả năm
            ViewBag.NamHocs = namHocs;         // dùng cho dropdown

            return View(data);
        }
        // GET: Create
        public async Task<IActionResult> Create()
        {
            await LoadLookups();
            return View(new PhanCongEditVm { NgayHoc = DateTime.Today, SoTiet = 4 });
        }

        // POST: Create -> gọi SP: sp_Tkb_Upsert + sp_TkbCt_Upsert + sp_PhanCong_Upsert
        [HttpPost]        
        public async Task<IActionResult> Create(PhanCongEditVm m)
        {
            if (!ModelState.IsValid) { await LoadLookups(); return View(m); }

            await using var tx = await _db.Database.BeginTransactionAsync();

            // 1) TKB header
            var pTkbId = new SqlParameter("@TkbId", SqlDbType.Int)
            { Direction = ParameterDirection.InputOutput, Value = DBNull.Value };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_Tkb_Upsert @TkbId OUTPUT, @NamHoc, @HocKy",
                pTkbId,
                new SqlParameter("@NamHoc", m.NamHoc),
                new SqlParameter("@HocKy", m.HocKy)
            );
            var tkbId = (int)pTkbId.Value;

            // 2) TKB_CHITIET
            var pTkbCtId = new SqlParameter("@TkbCtId", SqlDbType.Int)
            { Direction = ParameterDirection.InputOutput, Value = DBNull.Value };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_TkbCt_Upsert @TkbCtId OUTPUT, @TkbId, @LopId, @MonHocId, @LoaiHVId, @NgonNguId, @CaKipId, @NgayHoc, @SoTiet",
                pTkbCtId,
                new SqlParameter("@TkbId", tkbId),
                new SqlParameter("@LopId", m.LopId),
                new SqlParameter("@MonHocId", m.MonHocId),
                new SqlParameter("@LoaiHVId", m.LoaiHVId),
                new SqlParameter("@NgonNguId", m.NgonNguId),
                new SqlParameter("@CaKipId", m.CaKipId),
                new SqlParameter("@NgayHoc", m.NgayHoc),
                new SqlParameter("@SoTiet", m.SoTiet)
            );
            var tkbCtId = (int)pTkbCtId.Value;

            // 3) PHANCONG
            var pPcId = new SqlParameter("@PhanCongId", SqlDbType.Int)
            { Direction = ParameterDirection.InputOutput, Value = DBNull.Value };

            await _db.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_PhanCong_Upsert @PhanCongId OUTPUT, @GiangVienId, @TkbCtId, @VaiTro",
                pPcId,
                new SqlParameter("@GiangVienId", m.GiangVienId),
                new SqlParameter("@TkbCtId", tkbCtId),
                new SqlParameter("@VaiTro", (object?)m.VaiTro ?? DBNull.Value)
            );

            // 4) Tính lại tổng hợp để UI thấy ngay
            await _db.Database.ExecuteSqlRawAsync(
                "EXEC dbo.sp_Recalc_VuotTai_1GV @GiangVienId, @NamHoc, @HocKy",
                new SqlParameter("@GiangVienId", m.GiangVienId),
                new SqlParameter("@NamHoc", m.NamHoc),
                new SqlParameter("@HocKy", m.HocKy)
            );

            await tx.CommitAsync();
            return RedirectToAction(nameof(Index), new { namHoc = m.NamHoc, hocKy = m.HocKy });
        }

        // GET: Edit
        public async Task<IActionResult> Edit(int id) // id = PhanCongId
        {
            // Lấy record hiện tại từ view + join để biết TkbCtId
            var cur = await (from pc in _db.Set<v_PhanCong_ChiTiet>()
                             where pc.PhanCongId == id
                             select pc).AsNoTracking().FirstOrDefaultAsync();
            if (cur == null) return NotFound();

            // Tìm TkbCtId gốc
            var tkbCtId = await _db.Set<TKB_CHITIET>()
                .Where(x => x.NgayHoc == cur.NgayHoc && x.SoTiet == cur.SoTiet) // tốt nhất Bạn map entity thực TKB_CHITIET và truy theo khóa
                .Select(x => x.TkbCtId).FirstOrDefaultAsync();

            var vm = new PhanCongEditVm
            {
                PhanCongId = id,
                TkbCtId = tkbCtId,
                NamHoc = cur.NamHoc,
                HocKy = cur.HocKy ?? 1,
                SoTiet = cur.SoTiet,
                GiangVienId = cur.GiangVienId,
                VaiTro = cur.VaiTro
                // LopId/MonHocId/LoaiHVId/NgonNguId/CaKipId cần lấy từ bảng thật TKB_CHITIET
            };

            await LoadLookups();
            return View(vm);
        }

        // POST: Edit
        [HttpPost]
        public async Task<IActionResult> Edit(PhanCongEditVm m)
        {
            if (!ModelState.IsValid) { await LoadLookups(); return View(m); }

            // Cập nhật TKB_CHITIET
            //await _db.Database.ExecuteSqlInterpolatedAsync($@"
            //EXEC dbo.sp_TkbCt_Upsert 
            //     @TkbCtId   = {m.TkbCtId} OUTPUT,
            //     @TkbId     = (SELECT TOP 1 TkbId FROM dbo.THOIKHOABIEU WHERE NamHoc={m.NamHoc} AND HocKy={m.HocKy}),
            //     @LopId     = {m.LopId},
            //     @MonHocId  = {m.MonHocId},
            //     @LoaiHVId  = {m.LoaiHVId},
            //     @NgonNguId = {m.NgonNguId},
            //     @CaKipId   = {m.CaKipId},
            //     @NgayHoc   = {m.NgayHoc},
            //     @SoTiet    = {m.SoTiet}");

            await _db.Database.ExecuteSqlInterpolatedAsync($@"
                DECLARE @TkbId INT;
                SELECT TOP 1 @TkbId = TkbId 
                FROM dbo.THOIKHOABIEU 
                WHERE NamHoc = {m.NamHoc} AND HocKy = {m.HocKy};

                IF (@TkbId IS NULL)
                BEGIN
                    THROW 50001, '❌ Không tìm thấy Thời khóa biểu phù hợp với Năm học và Học kỳ. Bạn cần tạo TKB trước!', 1;
                END

                EXEC dbo.sp_TkbCt_Upsert 
                     @TkbCtId   = {m.TkbCtId} OUTPUT,
                     @TkbId     = @TkbId,
                     @LopId     = {m.LopId},
                     @MonHocId  = {m.MonHocId},
                     @LoaiHVId  = {m.LoaiHVId},
                     @NgonNguId = {m.NgonNguId},
                     @CaKipId   = {m.CaKipId},
                     @NgayHoc   = {m.NgayHoc},
                     @SoTiet    = {m.SoTiet};
                ");


            // Cập nhật PHANCONG
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC dbo.sp_PhanCong_Upsert 
                 @PhanCongId = {m.PhanCongId} OUTPUT,
                 @GiangVienId= {m.GiangVienId}, 
                 @TkbCtId    = {m.TkbCtId}, 
                 @VaiTro     = {m.VaiTro}");

            await _db.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC dbo.sp_Recalc_VuotTai_1GV 
                 @GiangVienId={m.GiangVienId}, @NamHoc={m.NamHoc}, @HocKy={m.HocKy}");

            TempData["ok"] = "Đã cập nhật.";
            return RedirectToAction(nameof(Index), new { namHoc = m.NamHoc, hocKy = m.HocKy });
        }

        // DELETE (nhanh gọn)
        [HttpPost]
        public async Task<IActionResult> Delete(int id, string namHoc, int hocKy, int giangVienId)
        {
            await _db.Database.ExecuteSqlInterpolatedAsync($@"DELETE dbo.PHANCONG WHERE PhanCongId={id}");
            await _db.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC dbo.sp_Recalc_VuotTai_1GV 
                 @GiangVienId={giangVienId}, @NamHoc={namHoc}, @HocKy={hocKy}");
            TempData["ok"] = "Đã xóa.";
            return RedirectToAction(nameof(Index), new { namHoc, hocKy });
        }

        // Load dropdowns cho form
        private async Task LoadLookups()
        {
            ViewBag.GiangVien = await _db.Set<GIANGVIEN>().AsNoTracking().ToListAsync();
            ViewBag.Lop = await _db.Set<LOP>().AsNoTracking().ToListAsync();
            ViewBag.MonHoc = await _db.Set<MONHOC>().AsNoTracking().ToListAsync();
            ViewBag.LoaiHV = await _db.Set<LOAIHOCVIEN>().AsNoTracking().ToListAsync();
            ViewBag.NgonNgu = await _db.Set<NGONNGU>().AsNoTracking().ToListAsync();
            ViewBag.CaKip = await _db.Set<CAKIP>().AsNoTracking().ToListAsync();
        }
    }
}
