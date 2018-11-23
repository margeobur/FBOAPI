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

        public override bool Equals(Object obj)
        {
            CombinedLink otherLink;
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                otherLink = (CombinedLink) obj;
            }

            if ((otherLink.NewC == null && this.NewC != null) ||
               (otherLink.NewC != null && this.NewC == null))
                return false;

            if ((otherLink.OldC == null && this.OldC != null) ||
               (otherLink.OldC != null && this.OldC == null))
                return false;

            if (NewC != null)
            {
                foreach (string name in this.NewC.GivenNames)
                {
                    if (!otherLink.NewC.GivenNames.Contains(name))
                        return false;
                }

                if (otherLink.NewC.GivenNames.Count() != this.NewC.GivenNames.Count())
                    return false;

                if (!(otherLink.Link.NewID.Equals(this.Link.NewID) &&
                    otherLink.NewC.Id.Equals(this.NewC.Id) &&
                    otherLink.NewC.Username.Equals(this.NewC.Username) &&
                    otherLink.NewC.Email.Equals(this.NewC.Email)))
                    return false;
            }

            if (OldC != null)
            {
                if (!(otherLink.Link.OldID.Equals(this.Link.OldID) &&
                    otherLink.OldC.Id.Equals(this.OldC.Id) &&
                    otherLink.OldC.Username.Equals(this.OldC.Username) &&
                    otherLink.OldC.FirstName.Equals(this.OldC.FirstName) &&
                    otherLink.OldC.Surname.Equals(this.OldC.Surname) &&
                    otherLink.OldC.Address.Equals(this.OldC.Address)))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Link);
        }
    }
}
