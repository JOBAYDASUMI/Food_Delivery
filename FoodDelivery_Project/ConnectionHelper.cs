using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery_Project
{
    public class ConnectionHelper
    {
        public static string ConString
        {
            get
            {
                string dbPath = Path.Combine(Path.GetFullPath(@"..\..\"), "fooddb.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dbPath};Initial Catalog=fooddb;Trusted_Connection=True";
            }


        }
    }
}
