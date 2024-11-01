using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonTypes
{
    // Our toy: class Person with some properties and ctors.
    public class Person : IComparable<Person>, IEquatable<Person>
    {
        // Dctor.
        public Person()
        { }


        // Another ctor awaiting a string (the name).
        public Person(string name)
        {
            Name = name;
        }


        /// <summary>
        /// The US Postal Service Code (USPS) of the state.
        /// </summary>
        public string State { get; set; }


        public string Company { get; set; }


        public int Age { get; set; }


        public string Name { get; set; }


        public IEnumerable<Person> Friends { get; set; }


        public override string ToString()
        {
            return string.Format("Name: {0}, Age: {1}, State: {2}", Name, Age, State);
        }


        public int CompareTo(Person other)
        {
            return Age.CompareTo(other.Age);
        }


        public override int GetHashCode()
        {
            return Age.GetHashCode() ^
                    (null != Name
                        ? Name.GetHashCode()
                        : 0);
        }


        public override bool Equals(object obj)
        {
            return obj != null && GetType().Equals(obj.GetType()) && Equals((Person)obj);
        }


        public bool Equals(Person other)
        {
            return null != other && Age.Equals(other.Age) && object.Equals(Name, other.Name);
        }


        public static readonly IEnumerable<Person> Persons = new[]
            {
                new Person("Patricia") {Age = 38, State = "ID", Company="YHOO",
                    Friends = new[]{new Person("Bill"), new Person("Henry")}},
                new Person("Angela") {Age = 40, State = "UT", Company="AVID",
                    Friends = new[]{new Person("Roger"), new Person("Tim")}},
                new Person("Caroline") {Age = 35, State = "MA", Company="CSCO"},
                new Person("Dana") {Age = 40, State = "NM", Company="NVDA",
                    Friends = new[]{new Person("Henry"), new Person("Peter")}},
                new Person("Tory") {Age = 40, State = "UT", Company="SBUX"},
                new Person("Bonnie") {Age = 37, State = "UT", Company="AVID",
                    Friends = new[]{new Person("Roger"), new Person("Bill")}},
                new Person("Conny") {Age = 32, State = "CO", Company="GRMN",
                    Friends = new[]{new Person("Gary"), new Person("Lex")}}
            };
    }
}
