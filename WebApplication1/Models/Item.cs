using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("Item")]
    public class Item : BaseEntity
    {
        [Column("Code")]
        [StringLength(20)]
        public string Code { get; set; }

        [Column("Name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Column("Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        [Column("Brand")]
        [StringLength(255)]
        public string Brand { get; set; }

        [Column("Price")]
        public double Price { get; set; }

        public int ItemCategoryId { get; set; }
        public virtual ItemCategory ItemCategory { get; set; }
    }
}