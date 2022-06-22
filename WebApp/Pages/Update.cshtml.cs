using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Pages
{
    public class UpdateModel : PageModel
    {   
        [BindProperty]
        public static string UserName { get; }

        [BindProperty]
        public List<Row> RequestTemplateName { get; set; } 

        [BindProperty]
        public List<Row> RequestT { get; set; } 

        [BindProperty]
        public IFormFile Update { get; set; } 

        [BindProperty]
        public string notes { get; set; } 

        [BindProperty]
        public string updateFileName {get; set;} 

        [BindProperty]
        public string selectedName {get; set;} 



        // Helper functions autocreated by razor pages
        private readonly ILogger<UpdateModel> _logger;

        public UpdateModel(ILogger<UpdateModel> logger)
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
            using (SqlConnection connection = new SqlConnection(builded.ConnectionString)) 
                {
                    // Opens database connetion
                    connection.Open();

                    //SQL query we want to run on the database
                    String sql = "Select templateName FROM ContractTemplate WHERE isArchived = 0 ORDER BY templateName ASC";

                    // Creates a list of lists of objects to hold each row from the contract template table
                    List<Row> Table = new List<Row>();
                    List<Row> T = new List<Row>();

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
                                    while (reader.Read())
                                {
                                // Create a new Row (class) to then be put into table, if unchanged we will see "error"
                                Row rowCreator = new Row("error", "error", "error", "error", "error", "error", "error", "error", "error", "error"); 
                                // Adds each element on the row to the rowCreator
                                rowCreator.templateName = (reader.GetString(0)).ToString();
                                Table.Add(rowCreator);
                                }
                            }
                        }

                        // Assigns Rows to RequestRows so the HTML can get request the data
                        RequestTemplateName = Table;
                    } using (SqlCommand command = new SqlCommand(sql, connection)) {

                        // Create a reader which takes each row at a time and we can index each row by columns 
                        using (SqlDataReader r = command.ExecuteReader())
                         {
                        // Creates database conenction and opens it up
                        SqlCommand cmd = new SqlCommand();
                        cmd.CommandText = sql;
                        cmd.Connection = connection;
                        // While the table has rows we add data
                        if (r.HasRows)
                        {
                            // Loop while we got data
                            while (r.Read())
                            {
                                // Create a new Row (class) to then be put into table, if unchanged we will see "error"
                                Row rowC = new Row("error", "error", "error", "error", "error", "error", "error", "error", "error", "error"); 
                                // Adds each element on the row to the rowCreator
                                rowC.templateName = (r.GetString(0)).ToString();
                                // Testing output
                                Console.WriteLine("\t{0}", r.GetString(0));
                                // Add the row to the table list
                                T.Add(rowC);
                            }

                             // Assigns Rows to RequestRows so the HTML can get request the data
                            RequestT= T;
                        }
                             
                            // Close the database connections
                            connection.Close();
                        }                     
                    }
                }
            } catch (SqlException e) {
                Console.WriteLine(e.ToString());
            }
        }

        public async Task OnPostAsync() 
        {   
            // Create a variable to get a root path which is MocTemplates folder
            // Takes in 3 strings into a path
            // MocTemplates is the folder in the WebApp where the file contents go to 
            // (Note will change later to accommodate for JCC)
            // (Note folder was been moved to wwwroot folder so download can work)
            var filePath = Path.Combine("wwwroot/MocTemplates", Update.FileName);
            var filePathForDatabase = Path.Combine("./MocTemplates", Update.FileName);

            //Gets the username of the local machine
            var userRef = (Environment.UserName);

            // takes in the path variable and creates a new file in that specified folder
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Then asynchronously copies that file over into the filestream
                await Update.CopyToAsync(fileStream);

                // This message will pop up once a file has been uploaded successfully
                ViewData["Message"]=" The selected file " + Update.FileName + " Is updated successfully";
            }

            // Creates a string builder for the SQL connection
            SqlConnectionStringBuilder builded = new SqlConnectionStringBuilder();

            // Connection string to connect to the database
            builded.ConnectionString = "Server=tcp:jccserver.database.windows.net,1433;Initial Catalog=JCCBD;Persist Security Info=False;User ID=kenobi;Password=Liamiscolorblind123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            // SQL query stuff: creates connection, opens, and queries the database for the Upload Page
            SqlConnection connection = new SqlConnection(builded.ConnectionString);

            // Part that actually opens the database, runs the query commands
            SqlCommand cmd = new SqlCommand();

            //gets and sets the connection
            cmd.Connection = connection;

            //specifies how the command string should be interpreted
            cmd.CommandType = CommandType.Text;

            // Insert statement for putting values in the database
            // still figuring out how to insert into the Contract template table but it works on test
            // Run it on the backend
            // auto incremented field will not be needed in insert statement
            // variable for filepath
            // will ask mariah if it will be current user or system user.
            cmd.CommandText = "INSERT INTO ContractTemplate VALUES (@templateName, -1, SYSDATETIME(), NULL, @filePathName, 0, @createdBy, @notes, NULL)";

            // Represents a paramter to the SQL command and mapping it to the columns in the database
            // Takes in the name of the column and the type
            SqlParameter FileName = new SqlParameter("@filePathName", SqlDbType.VarChar);
            SqlParameter templateFile = new SqlParameter("@templateName", SqlDbType.VarChar);
            SqlParameter Notes = new SqlParameter("@notes", SqlDbType.VarChar);
            //Takes in the current user of the machine
            SqlParameter userName = new SqlParameter("@createdBy", SqlDbType.VarChar);

            // Sets the value of the user
            userName.Value = userRef;
            
            //Sets the value to the filePath - Crashes if no file is given
            FileName.Value = filePathForDatabase;
            // Add something here to prompt the user they need to add a file

            //sets the value of the templateName
            templateFile.Value = (String)Request.Form["selectedName"];

            //sets the value of the notes, makes it empty if null
            if (notes != null) {
                Notes.Value = notes;
            } else {
                Notes.Value = "";
            }

            //Adds the values to the column
            cmd.Parameters.Add(templateFile);
            cmd.Parameters.Add(FileName);
            cmd.Parameters.Add(Notes);
            cmd.Parameters.Add(userName);
            
            //open the connection
            cmd.Connection.Open();

            //executes the query
            cmd.ExecuteNonQuery();

            //close the connection
            cmd.Connection.Close();
        }
    }
        //Based on the template Name the user selects one needs to get the version number and template ID
        //For the non-archived template Name
        //Update Statement that updates current template ID to isArchived
        //and an insert statement that inserts the new record with version number++ the templateId in 
        //version superceeded field and the notes,sys_user should be same as upload page
      
}
