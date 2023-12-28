using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Account")]
    public class Account
    {
        public Account()
        {
            Discussion = new HashSet<Discussion>();
            TicketUser = new HashSet<Ticket>();
            TicketSupporter = new HashSet<Ticket>();
        }
        [Key]
        public int Id { get; set; }
        public string FName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public bool Status { get; set; }
        public string Avatar { get; set; }
        public int RoleId { get; set; }

        public Role Role { get; set; }
        public Category Category { get; set; }


        public ICollection<Discussion> Discussion { get; set; }
        public ICollection<Ticket> TicketUser { get; set; }
        public ICollection<Ticket> TicketSupporter { get; set; }
    }
}
