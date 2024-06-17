/* Code for DotNetDB2ssl */
using System;
using IBM.Data.DB2;

namespace dotNetSSLTest
{
    class Program
    {
        static void Main(string[] args)
        {
            DB2Command MyDB2Command = null;
            // Use the dsn alias that you defined in db2dsdriver.cfg with the db2cli writecfg command
            String MyDb2ConnectionString = "database=alias;uid=sa;pwd=TestPwd1;"; 
            DB2Connection MyDb2Connection = new DB2Connection(MyDb2ConnectionString);
            MyDb2Connection.Open();
            MyDB2Command = MyDb2Connection.CreateCommand();
            MyDB2Command.CommandText = "SELECT branch_code, city from GOSALES.BRANCH";
            Console.WriteLine(MyDB2Command.CommandText);

            DB2DataReader MyDb2DataReader = null;
            MyDb2DataReader = MyDB2Command.ExecuteReader();
            Console.WriteLine("BRANCH\tCITY");
            Console.WriteLine("============================");
            while (MyDb2DataReader.Read())
            {
                for (int i = 0; i <= 1; i++)
                {
                    try
                    {
                        if (MyDb2DataReader.IsDBNull(i))
                        {
                            Console.Write("NULL");
                        }
                        else
                        {
                            Console.Write(MyDb2DataReader.GetString(i));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.Write(e.ToString());
                    }
                    Console.Write("\t"); 

                }
                Console.WriteLine("");
            }
            MyDb2DataReader.Close();
            MyDB2Command.Dispose();
            MyDb2Connection.Close();
        }
    }
}
