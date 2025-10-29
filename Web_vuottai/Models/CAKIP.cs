using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Web_vuottai.Models;

[Table("CAKIP")]
public partial class CAKIP
{
    [Key]
    public int CaKipId { get; set; }

    [StringLength(100)]
    public string? TenCaKip { get; set; }

    [Column(TypeName = "decimal(4, 2)")]
    public decimal HeSo { get; set; }

    [InverseProperty("CaKip")]
    public virtual ICollection<TKB_CHITIET> TKB_CHITIETs { get; set; } = new List<TKB_CHITIET>();
}
