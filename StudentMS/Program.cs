using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DateAccess;
using System.Data;
using System.Data.SqlClient;


namespace StudentMS
{
    class Program
    {
        static void Main(string[] args)
        {
            // DataSet da= SqlHelper.ExecuteDataSet(CommandType.Text,"SELECT*FROM Students",null);


            string strSQL1= "INSERT INTO Department(DeptName) VALUES('中央')";
            string strSQL2 = "INSERT INTO Department(DeptName) VALUES('中央')";


            try
            {

                SqlHelper.BeginTransaction();
                SqlHelper.ExecuteDataTable();
            }
            catch (Exception)
            {

                throw;
            }
            Console.ReadKey();
        }
    }
}
