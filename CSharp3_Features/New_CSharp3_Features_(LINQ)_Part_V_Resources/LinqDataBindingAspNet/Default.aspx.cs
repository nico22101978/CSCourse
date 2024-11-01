using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CommonTypes;

namespace LinqDataBindingAspNet
{
    public partial class _Default : System.Web.UI.Page
    {
        private CommonTypes.Database.ExampleDataEntities _context;


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


        protected void Page_Load(object sender, EventArgs e)
        {
            /*-----------------------------------------------------------------------------------*/
            // Databinding of LINQ to Objects Results to Windows Forms:

            // The object you want to set for the GridView's DataSource needs to implement
            // IEnumerable, IListSource or IDataSource. We can only read data from a sequence, two-
            // way data binding is not possible on sequences:

            gridViewLinqToObjects.DataSource =
                 from person in Person.Persons
                 join company in Company.Companies
                     on person.Company equals company.PublicNasdaq
                 join state in State.StatesOfUs
                     on person.State equals state.USPS
                 select new
                 {
                     person.Name,
                     State = state.Name,
                     Company = company.Name
                 };
            gridViewLinqToObjects.DataBind();


            /*-----------------------------------------------------------------------------------*/
            // Databinding of LINQ to Entities to Asp.Net:

            // With LINQ to Entities the story is slighly different. We use a EntityDataSource 
            // for the GridView and establish a twoway databinding. - I.e. we can read and write
            // data in the DataGrid and the updated data is saved back to the database when the
            // editing mode has ended. It is also possible to delete and create Person objects, in
            // other words the databinding enables full CRUD.
            // The data binding to the GridView (see gridViewLinqToEntities) is completely defined
            // in the markup.
        }

        protected void DetailsView1_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (null != e.Exception)
            {
                lastErrorMessage.Text = e.Exception.Message;
                e.ExceptionHandled = true;
            }
            else
            { 
                gridViewLinqToEntities.DataBind();
            }
        }


        protected void gridViewLinqToEntities_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {
            if (null != e.Exception)
            {
                lastErrorMessage.Text = e.Exception.Message;
                e.ExceptionHandled = true;
            }
        }
    }
}
