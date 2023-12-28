using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Ticket")]
    public class Ticket
    {
        public Ticket()
        {
            Discussion = new HashSet<Discussion>();
            Photo = new HashSet<Photo>();
        }
        [Key]
        public int Id { get; set; }
        public string Tittle { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CategoryId { get; set; }
        public int PeriodId { get; set; }
        public int? SupporterId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        
        public Account Supporter { get; set; }
        public Account User { get; set; }
        public Period Period { get; set; }
        public Category Category { get; set; }
        public Status Status { get; set; }
        public ICollection<Discussion> Discussion { get; set; }
        public ICollection<Photo> Photo { get; set; }


    }
}
