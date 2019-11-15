using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SharePointAddInAssignmentWeb.Helpers
{
    public static class SharepointHelper
    {
        public static void UpdateResouceFiles(Web web)
        {
            //Delete the folder if it exists
            Microsoft.SharePoint.Client.List list = web.Lists.GetByTitle("Style Library");
            IEnumerable<Folder> results = web.Context.LoadQuery<Folder>(list.RootFolder.Folders.Where(folder => folder.Name == "EmployeeTableWebPart"));
            web.Context.ExecuteQuery();
            Folder samplesJSfolder = results.FirstOrDefault();

            if (samplesJSfolder != null)
            {
                samplesJSfolder.DeleteObject();
                web.Context.ExecuteQuery();
            }

            samplesJSfolder = list.RootFolder.Folders.Add("EmployeeTableWebPart");
            web.Context.Load(samplesJSfolder);
            web.Context.ExecuteQuery();

            UploadFileToFolder(web, HostingEnvironment.MapPath("~/Scripts/JSLink-Samples/App.js"), samplesJSfolder);
            UploadFileToFolder(web, HostingEnvironment.MapPath("~/Scripts/bootstrap.min.js"), samplesJSfolder);
            UploadFileToFolder(web, HostingEnvironment.MapPath("~/Scripts/jquery-3.3.1.min.js"), samplesJSfolder);
            UploadFileToFolder(web, HostingEnvironment.MapPath("~/Content/App.css"), samplesJSfolder);
            UploadFileToFolder(web, HostingEnvironment.MapPath("~/Content/bootstrap.min.css"), samplesJSfolder);
        }
        public static void UploadFileToFolder(Web web, string filePath, Folder folder)
        {
            //Create a FileStream to the file to upload
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                //Create FileCreationInformation object to set file metadata
                FileCreationInformation flciNewFile = new FileCreationInformation();

                flciNewFile.ContentStream = fs;
                flciNewFile.Url = System.IO.Path.GetFileName(filePath);
                flciNewFile.Overwrite = true;

                //Upload file to SharePoint
                Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(flciNewFile);

                //Check in the file
                uploadFile.CheckIn("Resource Sample files", CheckinType.MajorCheckIn);

                folder.Context.Load(uploadFile);
                folder.Context.ExecuteQuery();
            }
        }
    }
}