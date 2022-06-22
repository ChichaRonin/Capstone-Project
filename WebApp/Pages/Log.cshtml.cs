using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Html;

namespace WebApp.Pages;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ContractLogModel : PageModel
{
    //list to  keep all the variables from the table in 
    public List<Row> LogTable { get; set; } 

    //helper functions
    private readonly ILogger<ContractLogModel> _logger;
    public  ContractLogModel(ILogger<ContractLogModel> logger)
    {
        _logger = logger;
    }
    public void OnGet()
    {
        //Query Data from the database and display it on web server
        try 
            {
                SqlConnectionStringBuilder builded = new SqlConnectionStringBuilder();

                //connection with the database server
                builded.ConnectionString="Insert here";

                using (SqlConnection connection = new SqlConnection(builded.ConnectionString))
                {
                    //making connection with the server
                    connection.Open();       

                    //sql query to get all data from the database that will be displayed in log
                    //get all data that waas stored within one year from the current date
                    String sql = "SELECT * FROM ContractTemplate WHERE dateCreated > DATEADD(year,-1, GETDATE()) ORDER BY dateCreated Desc";
                    //creating new list for logtable
                    List<Row> Table = new List<Row>();
                    
                    //sql connection information
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {   
                        //creates reader to read information from the swl table
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            SqlCommand cmd = new SqlCommand();
                            cmd.CommandText = sql;
                            cmd.Connection = connection;

                            //while reader is still able to read another row
                            if (reader.HasRows) 
                            {
                                ///continue to loop through the data
                                while (reader.Read())
                                {
                                    Row rowCreate = new Row("error", "error", "error", "error", "error", "error", "error", "error", "error", "error");
                                    //Adds each element on the row to the rowCreator
                                    //added all fields
                                    rowCreate.templateID = (reader.GetInt32(0)).ToString();
                                    rowCreate.templateName = (reader.GetString(1)).ToString(); 
                                    rowCreate.version = (reader.GetInt32(2)).ToString();
                                    rowCreate.dateCreated = reader.GetDateTime(3).Date.ToString("MM/dd/yyyy");
                                    //if the value in the database is null then print out the string
                                    rowCreate.dateSuperceded = reader.IsDBNull(4) ? " " : reader.GetDateTime(4).Date.ToString("MM/dd/yyyy");
                                    //file path was removed at request of client
                                    //rowCreate.filePathName = (reader.GetString(5)).ToString(); 
                                    rowCreate.isArchived = (reader.GetBoolean(6).ToString());
                                    rowCreate.createdBy= (reader.GetString(7)).ToString(); 
                                    rowCreate.notes = (reader.GetString(8)).ToString();
                                    //if the value in the database is null then print out the string
                                    //rowCreate.versionSuperceded = reader.IsDBNull(9) ? "NULL" : reader.GetInt32(9).ToString();
                                    //Testing output
                                    /*Console.WriteLine("\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}", 
                                        reader.GerInt32(0), reader.GetString(1), reader.GetInt32(2), reader.GetDateTime(3), reader.GetDateTime(4),
                                        reader.GetString(5),reader.GetBit(6), reader.getString(7), reader.GetString(8), reader.GetInt32(9)); */
                                    //Add the row to the table list
                                    Table.Add(rowCreate);
                                }
                            }
                            //place table into logtable
                            //logtable will go to log.cshtml
                            LogTable  = Table;
                        }
                    }
                    connection.Close();                    
                }
            }
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
