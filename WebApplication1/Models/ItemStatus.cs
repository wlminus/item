using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("ItemStatus")]
    public class ItemStatus : BaseEntity
    {
        [Column("Status")]
        [StringLength(255)]
        public string Status { get; set; }

        [Column("Decription")]
        [StringLength(1000)]
        public string Decription { get; set; }
        
        [Column("Code")]
        [StringLength(20)]
        public string Code { get; set; }

    }
}