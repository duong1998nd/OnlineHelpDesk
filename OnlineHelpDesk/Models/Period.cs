using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Period")]
    public class Period
    {
        public Period()
        {
            Ticket = new HashSet<Ticket>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [DefaultValue(false)]
        public bool Status { get; set; }
        public virtual ICollection<Ticket> Ticket { get; set; }
    }
}
