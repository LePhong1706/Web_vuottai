using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("MONHOC")]
[Index("MaMon", Name = "UQ__MONHOC__4127737E88AA4761", IsUnique = true)]
public partial class MONHOC
{
    [Key]
    public int MonHocId { get; set; }

    [StringLength(50)]
    public string? MaMon { get; set; }

    [StringLength(200)]
    public string? TenMonHoc { get; set; }

    [InverseProperty("MonHoc")]
    public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
}
