// import statements
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
public class ContractArchiveModel : PageModel
{
    // A list of Rows (class) representing the table to be requested from the HTML
    public List<Row> RequestTable { get; set; }

    // Helper functions autocreated by razor pages
    private readonly ILogger<ContractArchiveModel> _logger;

    public ContractArchiveModel(ILogger<ContractArchiveModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
        // Query the database and display it to the web console
        try
        {
            // Creates a string builder for the SQL connection
            SqlConnectionStringBuilder builded = new SqlConnectionStringBuilder();

            // Connection string to connect to the database
            builded.ConnectionString = "Server=tcp:jccserver.database.windows.net,1433;Initial Catalog=JCCBD;Persist Security Info=False;User ID=kenobi;Password=Liamiscolorblind123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            // Create a dateTime to be used in creating the Row (class)
            DateTime date_ = new DateTime(1900, 1, 1, 00, 00, 00);

            // SQL query stuff: creates connection, opens, and queries the database for the repo page
            using (SqlConnection connection = new SqlConnection(builded.ConnectionString))
            {
                // Opens database connetion
                connection.Open();

                //SQL query we want to run on the database
                String sql = "Select templateName, version, dateCreated, filePathName, notes FROM ContractTemplate WHERE isArchived = 1";

                // Creates a list of lists of objects to hold each row from the contract template table
                List<Row> Table = new List<Row>();

                // Part that actually opens the database, runs the query, adds items to the list, and get it ready for the HTML to get it
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Create a reader which takes each row at a time and we can index each row by columns 
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Creates database conenction and opens it up
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = sql;
                        cmd.Connection = connection;
                        // While the table has rows we add data
                        if (reader.HasRows)
                        {
                            // Loop while we got data
                            while (reader.Read())
                            {
                                // Create a new Row (class) to then be put into table, if unchanged we will see "error"
                                Row rowCreator = new Row("error", "error", "error", "error", "error", "error", "error", "error", "error", "error"); 
                                // Adds each element on the row to the rowCreator
                                rowCreator.templateName = (reader.GetString(0)).ToString();
                                rowCreator.version = (reader.GetInt32(1)).ToString();
                                rowCreator.dateCreated = (reader.GetDateTime(2)).Date.ToString("MM/dd/yyyy");
                                rowCreator.filePathName = (reader.GetString(3));
                                rowCreator.notes = (reader.GetString(4)).ToString();
                                // Testing output
                                Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", reader.GetString(0), reader.GetInt32(1), reader.GetDateTime(2), reader.GetString(4));
                                //Console.WriteLine("\t{0}\t{1}\t{2}\t{3}", )
                                // Add the row to the table list
                                Table.Add(rowCreator);
                            }
                        }
                        // Assigns Rows to RequestRows so the HTML can get request the data
                        RequestTable = Table;
                    }
                }
                // Close the database connections
                connection.Close();
            }
        }
        // Error catching
        catch (SqlException e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}
