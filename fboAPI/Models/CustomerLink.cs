using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace fboAPI.Models
{
    public class CustomerLink
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ID { set; get; }

        public int OldID { get; set; } = -1;
        public String NewID { get; set; }
    }
}
