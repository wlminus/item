using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    [Table("Transaction")]
    public class Transaction : BaseEntity
    {
        [Column("Date")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        [Column("FromLocation")]
        [StringLength(255)]
        public string FromLocation { get; set; }

        [Column("ToLocation")]
        [StringLength(255)]
        public string ToLocation { get; set; }

        [Column("FromStatus")]
        [StringLength(255)]
        public string FromStatus { get; set; }

        [Column("ToStatus")]
        [StringLength(255)]
        public string ToStatus { get; set; }

        [Column("IsVerified")]
        public Boolean IsVerified { get; set; }

        public int TransactionCategoryId { get; set; }
        public virtual TransactionCategory TransactionCategory { get; set; }
    }
}