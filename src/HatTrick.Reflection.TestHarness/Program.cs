using System;
using System.Collections.Generic;

namespace HatTrick.Reflection.TestHarness
{
    class Program
    {
        #region internals
        static Person _person;
        #endregion

        #region main
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            InitPerson();

            //reflect from class 'properties'
            ReflectPropertiesFromClass();

            //reflect from class 'fields'
            ReflectFieldsFromClass();

            //reflect from anonymous type
            ReflectFromAnonymousType();

            //reflect from dictionary
            ReflectFromDictionary();

            //null on no item exists
            NullOnNoItemExists();

            //throw ex on no item exists
            ThrowExOnNoItemExists();

            sw.Stop();
            Console.WriteLine($"processed in {sw.ElapsedMilliseconds} milliseconds, press [Enter] to exit");
            Console.ReadLine();
        }
        #endregion

        #region init person
        static void InitPerson()
        {
            _person = new Person()
            {
                FirstName = "Charlie",
                LastName = "Brown",
                DateOfBirth = DateTime.Parse("1998-05-05"),
                BillingAddress = new Address()
                {
                    Line1 = "1001 Main St.",
                    Line2 = "Suite 246",
                    City = "Plano",
                    State = "TX",
                    Zip = "75075"
                },
                HouseholdPet = new Pet()
                {
                    Name = "Jorge",
                    Age = 8,
                    PetType = PetType.Dog
                }
            };
        }
        #endregion

        #region reflect properties from class
        static void ReflectPropertiesFromClass()
        {
            Person p = _person;

            string firstName = p.ReflectItem<string>("FirstName");
            string lastName = p.ReflectItem<string>("LastName");
            DateTime? DOB = p.ReflectItem<DateTime?>("DateOfBirth");

            string addressLine1 = p.ReflectItem<string>("BillingAddress.Line1");
            string addressLine2 = p.ReflectItem<string>("BillingAddress.Line2");
            string addressCity = p.ReflectItem<string>("BillingAddress.City");
            string addressState = p.ReflectItem<string>("BillingAddress.State");
            string addressZip = p.ReflectItem<string>("BillingAddress.Zip");
        }
        #endregion

        #region reflcet fields from class
        static void ReflectFieldsFromClass()
        {
            Person p = _person;

            string petName = p.ReflectItem<string>("HouseholdPet.Name");
            int petAge = p.ReflectItem<int>("HouseholdPet.Age");
            PetType petType = p.ReflectItem<PetType>("HouseholdPet.PetType");
        }
        #endregion

        #region reflcet from anonymous
        static void ReflectFromAnonymousType()
        {
            var p = new
            {
                FirstName = "Charlie",
                LastName = "Brown",
                DateOfBirth = DateTime.Parse("1998-05-05"),
                BillingAddress = new
                {
                    Line1 = "1001 Main St.",
                    Line2 = "Suite 246",
                    City = "Plano",
                    State = "TX",
                    Zip = "75075"
                },
                HouseholdPet = new
                {
                    Name = "Jorge",
                    Age = 8,
                    PetType = PetType.Dog
                }
            };

            string firstName = p.ReflectItem<string>("FirstName");
            string lastName = p.ReflectItem<string>("LastName");
            DateTime? DOB = p.ReflectItem<DateTime?>("DateOfBirth");

            string addressLine1 = p.ReflectItem<string>("BillingAddress.Line1");
            string addressLine2 = p.ReflectItem<string>("BillingAddress.Line2");
            string addressCity = p.ReflectItem<string>("BillingAddress.City");
            string addressState = p.ReflectItem<string>("BillingAddress.State");
            string addressZip = p.ReflectItem<string>("BillingAddress.Zip");

            string petName = p.ReflectItem<string>("HouseholdPet.Name");
            int petAge = p.ReflectItem<int>("HouseholdPet.Age");
            PetType petType = p.ReflectItem<PetType>("HouseholdPet.PetType");
        }
        #endregion

        #region reflect from dictionary
        static void ReflectFromDictionary()
        {
            IDictionary<string, object> p = new Dictionary<string, object>()
            {
                { "FirstName", "Charlie" },
                { "LastName", "Brown" },
                { "DateOfBirth", DateTime.Parse("1998-05-05") },
                { "BillingAddress", new Dictionary<string, object>()
                    {
                        { "Line1", "1001 Main St." },
                        { "Line2", "Suite 246" },
                        { "City", "Plano" },
                        { "State", "TX" },
                        { "Zip", "75075" }
                    }
                },
                { "HouseholdPet", new Dictionary<string, object>()
                    {
                        { "Name", "Jorge" },
                        { "Age", 8 },
                        { "PetType", PetType.Dog }
                    }
                }
            };

            string firstName = p.ReflectItem<string>("FirstName");
            string lastName = p.ReflectItem<string>("LastName");
            DateTime? DOB = p.ReflectItem<DateTime?>("DateOfBirth");

            string addressLine1 = p.ReflectItem<string>("BillingAddress.Line1");
            string addressLine2 = p.ReflectItem<string>("BillingAddress.Line2");
            string addressCity = p.ReflectItem<string>("BillingAddress.City");
            string addressState = p.ReflectItem<string>("BillingAddress.State");
            string addressZip = p.ReflectItem<string>("BillingAddress.Zip");

            string petName = p.ReflectItem<string>("HouseholdPet.Name");
            int petAge = p.ReflectItem<int>("HouseholdPet.Age");
            PetType petType = p.ReflectItem<PetType>("HouseholdPet.PetType");

            //resolve dictionary first class properties
            ICollection<string> keys = p.ReflectItem<ICollection<string>>("Keys");
            ICollection<object> values = p.ReflectItem<ICollection<object>>("Values");
            int count = p.ReflectItem<int>("Count");

        }
        #endregion

        #region null on no item exists
        static void NullOnNoItemExists()
        {
            Person p = _person;

            string petFirstName = p.ReflectItem<string>("HouseholdPet.FirstName", false);
        }
        #endregion

        #region throw ex on no item exists
        static void ThrowExOnNoItemExists()
        {
            Person p = _person;

            try
            {
                string petFirstName = p.ReflectItem<string>("HouseholdPet.FirstName");
            }
            catch (NoItemExistsException nie)
            {
                string msg = nie.Message;
            }
        }
        #endregion
    }
}
