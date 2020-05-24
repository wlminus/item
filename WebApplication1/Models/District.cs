using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class District : BaseEntity
    {
        [Column("Name")]
        [StringLength(100)]
        public string Name { get; set; }

        [Column("Type")]
        [StringLength(100)]
        public string Type { get; set; }

        [Column("ProvinceId")]
        public long ProvinceId { get; set; }
        public virtual Province Province { get; set; }
    }
}