using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Photo")]
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
