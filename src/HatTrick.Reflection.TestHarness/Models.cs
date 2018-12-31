using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HatTrick.Reflection.TestHarness
{
    #region person
    public class Person
    {
        #region interface
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Address BillingAddress { get; set; }
        public bool HasPet => this.HouseholdPet != null;
        public Pet HouseholdPet { get; set; }
        #endregion
    }
    #endregion

    #region address
    public class Address
    {
        #region interface
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        #endregion
    }
    #endregion

    #region pet
    public class Pet
    {
        public PetType PetType;
        public string Name;
        public int Age;
    }
    #endregion


    #region pet type enum
    public enum PetType
    {
        Unknown,
        Cat,
        Dog,
    }
    #endregion
}
