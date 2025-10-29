using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("NGONNGU")]
public partial class NGONNGU
{
    [Key]
    public int NgonNguId { get; set; }

    [StringLength(50)]
    public string? TenNgonNgu { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    public decimal HeSo { get; set; }

    [InverseProperty("NgonN")]
    public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
}
