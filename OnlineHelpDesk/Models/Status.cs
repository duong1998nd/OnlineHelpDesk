using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Status")]
    public class Status
    {
        public Status()
        {
            Ticket = new HashSet<Ticket>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Display { get; set; }
        public ICollection<Ticket> Ticket { get; set; }
        
    }
}
