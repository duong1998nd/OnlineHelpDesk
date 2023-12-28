using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Discussion")]
    public class Discussion
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime? CreateDate { get; set; }
        public int TickerId { get; set; }
        public int AccountId { get; set; }

        public virtual Account Account { get; set; }
        public virtual Ticket Ticket { get; set; }

    }
}
