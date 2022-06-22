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

// A class to hold all the atrributes from the repo table
public class Row
{
    // Can hold each column from the table
    public string templateID;
    public string templateName;
    public string version;
    public string dateCreated;
    public string dateSuperceded;
    public string filePathName;
    public string isArchived;
    public string createdBy;
    public string notes;
    public string versionSuperceded;
    

    // Contructor to assing all values above to the incoming values
    public Row(string id, string temp, string ver, string dc, string ds, string fpn, string arch, string cb, string note, string vs)
   {
       templateID = id;
       templateName = temp; 
       version = ver; 
       dateCreated = dc; 
       dateSuperceded = ds;
       filePathName = fpn; 
       isArchived = arch;
       createdBy = cb;
       notes = note;
       versionSuperceded = vs;

   }

    public string RequestTemplateID { get; set; }
    public string RequestTemplateName { get; set; }
    public string RequestVersion { get; set; }
    public string RequestDateCreated { get; set; }
    public string RequestDateSuperceded { get; set; }
    public string RequestFilePathName { get; set; }
    public string RequestIsArchived { get; set; }
    public string RequestCreatedBy { get; set; }
    public string RequestNotes { get; set; }
    public string RequestVersionSuperceded { get; set; }


}