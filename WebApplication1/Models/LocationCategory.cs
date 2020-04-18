using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("LocationCategory")]
    public class LocationCategory : BaseEntity
    {
        [Column("Name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Column("Description")]
        [StringLength(1000)]
        public string Description { get; set; }

        public virtual ICollection<House> Houses { get; set; }
    }
}