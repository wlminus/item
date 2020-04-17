using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace WebApplication1.Models
{
    [Table("Demo")]
    public class Demo
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }
        [Column("Name")]
        [StringLength(500)]
        public string Name { get; set; }
        [Column("Age")]
        public int Age { get; set; }
    }
}