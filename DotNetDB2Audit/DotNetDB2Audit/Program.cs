/* Code for DotNetDB2Audit */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using AuditService;
using IBM.Data.DB2;

namespace dotNetDB2Audit
{
    class Program
    {

        public DataSet GetCPData()
        {
            DB2Connection db2Con = null;
            DB2DataReader CPReader = null;
            try
            {
                db2Con = new DB2Connection("Database=TestAudit;UserID=sa;Password=mypwd;Server=localhost");

                string CPSelect = "SELECT Query";

                DB2Command CPCommand = new DB2Command(CPSelect, db2Con);
                db2Con.Open();
                CPReader = CPCommand.ExecuteReader();

                DataSet dsReportsTo = new DataSet();

                DB2DataAdapter daReportsTo = new DB2DataAdapter(CPSelect, db2Con);

                daReportsTo.Fill(dsReportsTo);
                // always call Close when done reading.
                CPReader.Close();
                // always call Close when done with connection.
                db2Con.Close();
                CPCommand.Dispose();

                return dsReportsTo;
            }
            catch (Exception Ex)
            {
                string error = Ex.ToString();
                return null;
            }
            finally
            {
            }
        }

        static void Main(string[] args)
        {
            //QuickAudit AuditSrv = new QuickAudit();
            DataSet dsCP = GetCPData();
            Console.WriteLine("CPData");
            Console.WriteLine(DataSet.ToString());
            Console.WriteLine("============================");
        }
    }
}
