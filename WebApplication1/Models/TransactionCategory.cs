using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("TransactionCategory")]
    public class TransactionCategory : BaseEntity
    {
        [Column("Name")]
        [StringLength(500)]
        public string Name { get; set; }

        [Column("Decription")]
        [StringLength(1000)]
        public string Decription { get; set; }

    }
}