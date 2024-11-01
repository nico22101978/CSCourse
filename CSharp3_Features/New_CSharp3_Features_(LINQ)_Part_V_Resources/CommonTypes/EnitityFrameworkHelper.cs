//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//
//namespace CommonTypes
//{
//    public static class EnitityFrameworkHelper
//    {
//        private static Database.ExampleDataEntities _context;
//        public static Database.ExampleDataEntities Context
//        {
//            get
//            {
//                if (null == _context)
//                {
//                    string connectionString =
//                        System.Configuration.ConfigurationManager.ConnectionStrings[
//                            "ExampleDataEntities"].ConnectionString;
//                    _context = new CommonTypes.Database.ExampleDataEntities(connectionString);
//                }
//                return _context;
//            }
//        }
//    }
//}
