using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonTypes
{
    /// <summary>
    /// A simple class State that represents a state of the US.
    /// </summary>
    public class State
    {
        public State(string USPS, int year, string name, string capital)
        {
            this.USPS = USPS;
            this.Year = year;
            this.Name = name;
            this.Capital = capital;
        }


        public string Name { get; set; }


        public string Capital { get; set; }


        /// <summary>
        /// Gets or sets the US Postal Service Code
        /// </summary>
        public string USPS { get; set; }


        public int Year { get; set; }


        public static readonly IEnumerable<State> StatesOfUs = new[]
            {
                new State("AL", 1819, "Alabama", "Montgomery"),
                #region more ...
                new State("AK", 1959, "Alaska", "Juneau"),
                new State("AZ", 1912, "Arizona", "Phoenix"),
                new State("AR", 1836, "Arkansas", "Little Rock"),
                new State("CA", 1850, "California", "Sacramento"),
                new State("CO", 1876, "Colorado", "Denver"),
                new State("CT", 1788, "Connecticut", "Hartford"),
                new State("DE", 1787, "Delaware", "Dover"),
                new State("FL", 1845, "Florida", "Tallahassee"),
                new State("GA", 1788, "Georgia", "Atlanta"),
                new State("HI", 1959, "Hawaii", "Honolulu"),
                new State("ID", 1890, "Idaho", "Boise"),
                new State("IL", 1818, "Illinois", "Springfield"),
                new State("IN", 1816, "Indiana", "Indianapolis"),
                new State("IA", 1846, "Iowa", "Des Moines"),
                new State("KS", 1861, "Kansas", "Topeka"),
                new State("KY", 1792, "Kentucky", "Frankfort"),
                new State("LA", 1812, "Louisiana", "Baton Rouge"),
                new State("ME", 1820, "Maine", "Augusta"),
                new State("MD", 1788, "Maryland", "Annapolis"),
                new State("MA", 1788, "Massachusetts", "Boston"),
                new State("MI", 1837, "Michigan", "Lansing"),
                new State("MN", 1858, "Minnesota", "Saint Paul"),
                new State("MS", 1817, "Mississippi", "Jackson"),
                new State("MO", 1821, "Missouri", "Jefferson City"),
                new State("MT", 1889, "Montana", "Helena"),
                new State("NE", 1867, "Nebraska", "Lincoln"),
                new State("NV", 1864, "Nevada", "Carson City"),
                new State("NH", 1788, "New Hampshire", "Concord"),
                new State("NJ", 1787, "New Jersey", "Trenton"),
                new State("NM", 1912, "New Mexico", "Santa Fe"),
                new State("NY", 1788, "New York", "Albany"),
                new State("NC", 1789, "North Carolina", "Raleigh"),
                new State("ND", 1889, "North Dakota", "Bismarck"),
                new State("OH", 1803, "Ohio", "Columbus"),
                new State("OK", 1907, "Oklahoma", "Oklahoma City"),
                new State("OR", 1859, "Oregon", "Salem"),
                new State("PA", 1787, "Pennsylvania", "Harrisburg"),
                new State("RI", 1790, "Rhode Island", "Providence"),
                new State("SC", 1788, "South Carolina", "Columbia"),
                new State("SD", 1889, "South Dakota", "Pierre"),
                new State("TN", 1796, "Tennessee", "Nashville"),
                new State("TX", 1845, "Texas", "Austin"),
                new State("UT", 1896, "Utah", "Salt Lake City"),
                new State("VT", 1791, "Vermont", "Montpelier"),
                new State("VA", 1788, "Virginia", "Richmond"),
                new State("WA", 1889, "Washington", "Olympia"),
                new State("WV", 1863, "West Virginia", "Charleston"),
                new State("WI", 1848, "Wisconsin", "Madison"),

                #endregion
                new State("WY", 1890, "Wyoming", "Cheyenne")
            };
    }
}
