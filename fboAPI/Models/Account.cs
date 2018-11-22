using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace fboAPI.Models
{
    public class Account
    {
        private Guid _id;
        [Key]
        public Guid ID
        {
            set { _id = value; }
            get { return _id; }
        }

        public string Number { get; set; }
    }
}
