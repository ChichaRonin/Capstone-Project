using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
    public class UploadFileModel : PageModel
    {   
        [BindProperty]
        public static string UserName { get; }

        [BindProperty]
        public IFormFile Upload { get; set; }

        [BindProperty]
        public string template { get; set; }

        [BindProperty]
        public string notes { get; set; }

        [BindProperty, MaxLength(300)]
        public string MainText { get; set; }

        public async Task OnPostAsync()
        {
            // Create a variable to get a root path which is MocTemplates folder
            // Takes in 3 strings into a path
            // MocTemplates is the folder in the WebApp where the file contents go to 
            // (Note will change later to accommodate for JCC)
            // (Note folder was been moved to wwwroot folder so download can work)
            var filePath = Path.Combine("wwwroot/MocTemplates", Upload.FileName);
            var filePathForDatabase = Path.Combine("./MocTemplates", Upload.FileName);

            //Gets the username of the local machine
            var userRef = (Environment.UserName);

            // takes in the path variable and creates a new file in that specified folder
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                // Then asynchronously copies that file over into the filestream
                await Upload.CopyToAsync(fileStream);

                // This message will pop up once a file has been uploaded successfully
                ViewData["Message"] = " The selected file " + Upload.FileName + " Is uploaded successfully";
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
            cmd.CommandText = "INSERT INTO ContractTemplate VALUES (@templateName, 1, SYSDATETIME(), NULL, @filePathName, 0, @createdBy, @notes, NULL)";

            // Represents a paramter to the SQL command and mapping it to the columns in the database
            // Takes in the name of the column and the type
            SqlParameter FileName = new SqlParameter("@filePathName", SqlDbType.VarChar);

            //Takes in the template name created by the user
            SqlParameter templateFile = new SqlParameter("@templateName", SqlDbType.VarChar);

            //Takes in the notes that a user may make
            SqlParameter Notes = new SqlParameter("@notes", SqlDbType.VarChar);

            //Takes in the current user of the machine
            SqlParameter userName = new SqlParameter("@createdBy", SqlDbType.VarChar);

            userName.Value = userRef;

            //Sets the value to the filePath 
            FileName.Value = filePathForDatabase;

            //sets the value of the templateName
            templateFile.Value = template;

            //sets the value of the notes, makes it empty if null
            if (notes != null) {
                Notes.Value = notes;
            } else {
                Notes.Value = "";
            }

            //Adds the value to the column
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
}