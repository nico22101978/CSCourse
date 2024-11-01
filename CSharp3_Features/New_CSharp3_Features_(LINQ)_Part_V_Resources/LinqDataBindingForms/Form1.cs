using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CommonTypes;

namespace LinqDataBindingForms
{
    public partial class Form1 : Form
    {
        private CommonTypes.Database.ExampleDataEntities _context;

        
        public Form1()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Writes the updated data back to the database.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (null != _context)
            {
                _context.SaveChanges();
            }

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


        private void Form1_Load(object sender, EventArgs e)
        {
            /*-----------------------------------------------------------------------------------*/
            // Databinding of LINQ to Objects Results to Windows Forms:

            // The object you want to set for the Datasource needs to implement IList, it is
            // sufficient to call ToArray() on the result. - Alas the deferred execution, one of
            // the key benefits of LINQ, is then no longer available. But it is still a twoway 
            // databinding. - I.e. we can read and write data in the DataGrid and the updated data
            // is stored back to the individual objects. On the other hand we can not _add_ new 
            // objects to the DataSource, because it is a readonly sequence IEnumerable<Person>:

            dataGridViewLinqToObjects.DataSource =
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
            // Databinding of LINQ to Entities to Windows Forms:

            // With LINQ to Entities the story is slighly different. We also use a LINQ query to
            // create the DataSource for the DataGridView and we have twoway databinding. - I.e. we
            // can read and write data in the DataGrid and the updated data is saved back to the
            // database when the Form is closing (see OnClosing()). But additionally we can also
            // _add_ new Persons, because EF's central type IQueryable<Person> enables us to
            // directly communicate with the database:

            dataGridViewLinqToEntities.DataSource = 
                from person in GetEntities()
                where person.Age < 40
                select person;
        }


        private void dataGridViewLinqToEntities_DataError(object sender,
            DataGridViewDataErrorEventArgs e)
        {
            errorMessage.Text = e.Exception.Message;
            e.Cancel = true;
        }
    }
}
