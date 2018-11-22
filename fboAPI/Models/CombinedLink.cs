using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace fboAPI.Models
{
    public class CombinedLink
    {
        public CustomerLink Link { get; set; }
        public NewCustomer NewC { get; set; }
        public OldCustomer OldC { get; set;  }
    }
}
