using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using HatTrick.Reflection;

namespace HatTrick.Reflection.TestHarness
{
    class Program
    {
        static Person _person;

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

        static void InitPerson()
        {
            _person = new Person()
            {
                FullName = "Charlie Brown",
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

        static void ReflectPropertiesFromClass()
        {
            Person p = _person;

            string firstName = (string)ReflectionHelper.Expression.ReflectItem(p, "FirstName");
            string lastName = (string)ReflectionHelper.Expression.ReflectItem(p, "LastName");
            DateTime? DOB = (DateTime?)ReflectionHelper.Expression.ReflectItem(p, "DateOfBirth");

            string addressLine1 = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Line1");
            string addressLine2 = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Line2");
            string addressCity = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.City");
            string addressState = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.State");
            string addressZip = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Zip");
        }

        static void ReflectFieldsFromClass()
        {
            Person p = _person;

            string petName = (string)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.Name");
            int petAge = (int)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.Age");
            PetType petType = (PetType)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.PetType");
        }

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

            string firstName = (string)ReflectionHelper.Expression.ReflectItem(p, "FirstName");
            string lastName = (string)ReflectionHelper.Expression.ReflectItem(p, "LastName");
            DateTime? DOB = (DateTime?)ReflectionHelper.Expression.ReflectItem(p, "DateOfBirth");

            string addressLine1 = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Line1");
            string addressLine2 = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Line2");
            string addressCity = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.City");
            string addressState = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.State");
            string addressZip = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Zip");

            string petName = (string)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.Name");
            int petAge = (int)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.Age");
            PetType petType = (PetType)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.PetType");
        }

        static void ReflectFromDictionary()
        {
            Dictionary<string, object> p = new Dictionary<string, object>()
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

            string firstName = (string)ReflectionHelper.Expression.ReflectItem(p, "FirstName");
            string lastName = (string)ReflectionHelper.Expression.ReflectItem(p, "LastName");
            DateTime? DOB = (DateTime?)ReflectionHelper.Expression.ReflectItem(p, "DateOfBirth");

            string addressLine1 = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Line1");
            string addressLine2 = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Line2");
            string addressCity = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.City");
            string addressState = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.State");
            string addressZip = (string)ReflectionHelper.Expression.ReflectItem(p, "BillingAddress.Zip");

            string petName = (string)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.Name");
            int petAge = (int)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.Age");
            PetType petType = (PetType)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.PetType");
        }

        static void NullOnNoItemExists()
        {
            Person p = _person;

            string petFirstName = (string)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.FirstName", false);
        }

        static void ThrowExOnNoItemExists()
        {
            Person p = _person;

            try
            {
                string petFirstName = (string)ReflectionHelper.Expression.ReflectItem(p, "HouseholdPet.FirstName");
            }
            catch (NoItemExistsException nie)
            {
                string msg = nie.Message;
            }
        }
    }
}
