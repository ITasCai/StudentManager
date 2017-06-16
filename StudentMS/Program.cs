using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using DateAccess;
using DataAccess;

namespace Deam
{

    namespace StudentMS
    {
        class Program
        {
            static void Main(string[] args)
            {
                //DataSet ds = SqlHelper.ExecuteDataSet(CommandType.Text, "SELECT * FROM Students", null);

                //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                //{
                //    DataRow dr = ds.Tables[0].Rows[i];
                //    for (int j = 0; j< ds.Tables[0].Columns.Count; j++)
                //    {
                //        Console.Write(dr[j] + "\t");
                //    }
                //    Console.WriteLine();
                //}

                string strsql1 = "insert into department(deptname) values('众阳')";
                string strsql2 = "insert into department(deptname) values('众阳')";

                try
                {
                    SqlHelper.BeginTransaction();
                    SqlHelper.ExecuteTransaction(CommandType.Text, strsql1);
                    SqlHelper.ExecuteTransaction(CommandType.Text, strsql2);
                    SqlHelper.CommitTransaction();

                }
                catch (Exception ex)
                {
                    SqlHelper.RollBackTransaction();
                    Console.WriteLine(ex.Message);
                    throw;
                }

                Console.ReadKey();
            }
        }
    }
}
