using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace fboAPI.Models
{
    public class NewCustomer
    {
        [Key]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        // Learnt how to wrap a list of primitives like this from this stackoverflow post:
        // https://stackoverflow.com/questions/20711986/entity-framework-code-first-cant-store-liststring/20712012
        private List<String> _givenNames { get; set; }

        [NotMapped]
        public List<string> GivenNames
        {
            get { return _givenNames; }
            set { _givenNames = value; }
        }

        // Take the list of strings and join them with semicolons as the delimiter
        [Required]
        public string StringsAsString
        {
            get { return String.Join(';', _givenNames); }
            set { _givenNames = value.Split(';').ToList(); }
        }

        /* private List<Account> _accounts { get; set; }

        [NotMapped]
        public List<Account> Accounts
        {
            get { return _accounts; }
            set { _accounts = value; }
        } */
    }
}
