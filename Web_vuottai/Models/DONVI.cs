using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_vuottai.Models;

[Table("DONVI")]
[Index("MaDonVi", Name = "UQ__DONVI__DDA5A6CEC55437AC", IsUnique = true)]
public partial class DONVI
{
    [Key]
    public int DonViId { get; set; }

    [StringLength(20)]
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)] // Thêm attribute này
    public string? MaDonVi { get; set; }

    [StringLength(200)]
    public string TenDonVi { get; set; } = null!;

    [InverseProperty("DonVi")]
    public virtual ICollection<GIANGVIEN> GIANGVIENs { get; set; } = new List<GIANGVIEN>();
}
