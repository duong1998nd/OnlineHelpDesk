using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineHelpDesk.Models
{
    [Table("Category")]
    public class Category
    {
        public Category()
        {
            Ticket = new HashSet<Ticket>();
        }
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [DefaultValue(false)]
        public bool Status { get; set; }
        public ICollection<Ticket> Ticket { get; set; }
    }
}
