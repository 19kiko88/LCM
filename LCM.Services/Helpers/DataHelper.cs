using LCM.Repositories;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LCM.Services.Helpers
{
    public class DataHelper
    {
        public class DataTableHelper 
        {
            /// <summary>
            /// Ref 1.：https://stackoverflow.com/questions/38865498/how-to-convert-datatable-to-generic-list-in-c-sharp                        
            /// DataTable 轉 List(大量資料會很慢)
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="dt"></param>
            /// <returns></returns>
            public List<T> BindList<T>(DataTable dt) where T : class, new()
            {
                try
                {
                    List<T> list = new List<T>();

                    foreach (var row in dt.AsEnumerable())
                    {
                        T obj = new T();

                        foreach (var prop in obj.GetType().GetProperties())
                        {
                            try
                            {
                                PropertyInfo propertyInfo = obj.GetType().GetProperty(prop.Name);
                                propertyInfo.SetValue(obj, Convert.ChangeType(row[prop.Name], propertyInfo.PropertyType), null);
                            }
                            catch
                            {
                                continue;
                            }
                        }

                        list.Add(obj);
                    }

                    return list;
                }
                catch
                {
                    return null;
                }



                //var fields = typeof(T).GetFields();

                //List<T> lst = new List<T>();

                //foreach (DataRow dr in dt.Rows)
                //{
                //    // Create the object of T
                //    var ob = Activator.CreateInstance<T>();

                //    foreach (var fieldInfo in fields)
                //    {
                //        foreach (DataColumn dc in dt.Columns)
                //        {
                //            // Matching the columns with fields
                //            if (fieldInfo.Name == dc.ColumnName)
                //            {
                //                // Get the value from the datatable cell
                //                object value = dr[dc.ColumnName];

                //                // Set the value into the object
                //                fieldInfo.SetValue(ob, value);
                //                break;
                //            }
                //        }
                //    }

                //    lst.Add(ob);
                //}

                //return lst;
            }

            /// <summary>
            /// Ref [Bulk Copy C# to PostgreSql]：https://stackoverflow.com/questions/38865498/how-to-convert-datatable-to-generic-list-in-c-sharp       
            /// PostgreSQL STDIN (Copy file content to table)
            /// </summary>
            /// <param name="tableName"></param>
            /// <param name="filePath"></param>
            /// <param name="delimiter">csv切割符號(ex: ',')</param>
            /// <param name="connString">連線字串</param>
            /// <returns></returns>
            public async Task<bool> CopyToInsert(string tableName, string filePath, string delimiter, string connString)
            {
                NpgsqlConnection conn = new NpgsqlConnection(connString);
                NpgsqlCommand cmd = new NpgsqlCommand();
                bool result = true;

                try
                {
                    conn.Open();
                    NpgsqlTransaction transaction = conn.BeginTransaction();
                    if (File.Exists(filePath))
                    {
                        try
                        {
                            NpgsqlCommand command = new NpgsqlCommand($"COPY {tableName} FROM '{filePath}' (DELIMITER '{delimiter}')", conn);
                            command.ExecuteNonQuery();
                        }
                        catch (Exception e)
                        {
                            result = false;
                            transaction.Rollback();
                            throw e;
                        }
                        finally
                        {
                            if (result)
                            {
                                transaction.Commit();
                            }
                            transaction.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    result = false;
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                    conn.Dispose();
                }

                return result;
            }

            /// <summary>
            /// Ref：https://www.c-sharpcorner.com/UploadFile/1a81c5/list-to-datatable-converter-using-C-Sharp/
            /// List to DataTable
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="items"></param>
            /// <returns></returns>
            public DataTable ToDataTable<T>(List<T> items)
            {
                DataTable dataTable = new DataTable(typeof(T).Name);
                //Get all the properties
                PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in Props)
                {
                    //Setting column names as Property names
                    dataTable.Columns.Add(prop.Name);
                }
                foreach (T item in items)
                {
                    var values = new object[Props.Length];
                    for (int i = 0; i < Props.Length; i++)
                    {
                        //inserting property values to datatable rows
                        values[i] = Props[i].GetValue(item, null);
                    }
                    dataTable.Rows.Add(values);
                }
                //put a breakpoint here and check datatable
                return dataTable;
            }
        }
    }
}
