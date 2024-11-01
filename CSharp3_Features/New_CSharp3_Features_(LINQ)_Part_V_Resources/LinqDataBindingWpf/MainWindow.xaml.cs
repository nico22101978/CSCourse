using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonTypes;

namespace LinqDataBindingWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CommonTypes.Database.ExampleDataEntities _context;


        public MainWindow()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Writes the updated data back to the database.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _context.SaveChanges();
            base.OnClosing(e);
        }


        /// <summary>
        /// Fills and retrives Person data from the database.
        /// </summary>
        /// <returns>The Persons.</returns>
        private System.Data.Objects.ObjectSet<CommonTypes.Database.Person> GetEntities()
        {
            _context = CommonTypes.EnitityFrameworkHelper.Context;

            if (!_context.Persons.Any())
            {
                foreach (var person in Person.Persons)
                {
                    _context.Persons.AddObject(
                        new CommonTypes.Database.Person
                        {
                            Name = person.Name,
                            Age = person.Age,
                            Company = person.Company,
                            State = person.State
                        });
                }

                _context.SaveChanges();
            }

            return _context.Persons;
        }


        protected override void OnInitialized(EventArgs e)
        {
            /*-----------------------------------------------------------------------------------*/
            // Databinding of LINQ to Objects Results to WPF:

            // The object you want to set for the Itemssource needs to implement IEnumerable. This
            // establishes only a oneway databinding. - I.e. we can only read data in the DataGrid
            // and the updated data:

            dataGridLinqToObjects.ItemsSource =
                (from person in Person.Persons
                 join company in Company.Companies
                     on person.Company equals company.PublicNasdaq
                 join state in State.StatesOfUs
                     on person.State equals state.USPS
                 select new
                 {
                     person.Name,
                     State = state.Name,
                     Company = company.Name
                 }).ToArray();


            /*-----------------------------------------------------------------------------------*/
            // Databinding of LINQ to Entities to WPF:

            // With LINQ to Entities the story is slighly different. We also use a LINQ query to
            // create the ItemsSource for the DataGrid and we have twoway databinding. - I.e. we
            // can read and write data in the DataGrid and the updated data is saved back to the
            // database when the Window is closing (see OnClosing()). But additionally we can also
            // _add_ new Persons, because EF's central type IQueryable<Person> enables us to
            // directly communicate with the database:
            
            dataGridLinqToEntities.ItemsSource =
                   from person in GetEntities()
                   where person.Age < 40
                   select person;

            base.OnInitialized(e);
        }
    }
}