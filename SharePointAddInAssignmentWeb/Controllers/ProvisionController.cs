using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.WebAPI;
using SharePointAddInAssignment.Services;
using SharePointAddInAssignment.Services.Models;
using SharePointAddInAssignmentWeb.Filters;
using SharePointAddInAssignmentWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SharePointAddInAssignmentWeb.Controllers
{
    [EnableCors(origins: "https://m365b832837.sharepoint.com,https://localhost:44370",
        headers: "*", methods: "*",
        SupportsCredentials = true)]  
    [RoutePrefix("api/provision")]
    [WebAPIContextFilter]
    public class ProvisionController : ApiController
    {
        private IEmployeeService employeeService;
        public ProvisionController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }
        // GET: Sharepoint

        [HttpGet]
        [Route("uploadfile")]
        public string UpdateLoadJSFile()
        {

            using (var clientContext = WebAPIHelper.GetClientContext(this.ControllerContext))
            {
                if (clientContext != null)
                {
                    SharepointHelper.UpdateResouceFiles(clientContext.Web);
                }
            }
            string message = "Success";
            return message;
        }

        [HttpGet]
        [Route("createnewpage")]
        public IHttpActionResult CreateNewPage()
        {
            GeneralResponseMessage result = new GeneralResponseMessage { Success = true, Message = "Add New Page succesfully" };
            try
            {
                using (var clientContext = WebAPIHelper.GetClientContext(this.ControllerContext))
                {
                    if (clientContext != null)
                    {
                        Web web = clientContext.Web;
                        clientContext.Load(web);
                        clientContext.ExecuteQueryRetry();

                        List sitePagesList = web.Lists.GetByTitle("Site Pages");
                        var existedSite = sitePagesList.RootFolder.Files.GetByUrl("/sites/develop.addins/SitePages/EmployeeTablePage.aspx");

                        clientContext.Load(sitePagesList);
                        clientContext.Load(sitePagesList.RootFolder);
                        clientContext.Load(existedSite);
                        clientContext.ExecuteQueryRetry();

                        if (existedSite == null)
                        {
                            sitePagesList.RootFolder.Files.AddTemplateFile("/sites/develop.addins/SitePages/EmployeeTablePage.aspx", TemplateFileType.StandardPage);
                            clientContext.ExecuteQuery();
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "The Page is currently existed on sharepoint site";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return Json(result);
            }

            return Json(result);
        }
      
        [HttpGet]
        [Route("create-employee-list")]
        public IHttpActionResult CreateEmployeeList()
        {
            GeneralResponseMessage result = new GeneralResponseMessage { Success = true, Message = "Create Employees List succesfully" };
            try
            {
                using (var clientContext = WebAPIHelper.GetClientContext(this.ControllerContext))
                {
                    if (clientContext != null)
                    {
                        var query = clientContext.Web.Lists
                                    .Where(l => l.Title == "Employees")                                  
                                    .Select(l => l);
                        var queryResult = clientContext.LoadQuery(query);
                        clientContext.ExecuteQuery();

                        var existedList = queryResult.FirstOrDefault();


                        if (existedList != null)
                        {
                            existedList.DeleteObject();
                            clientContext.ExecuteQuery();
                        }
                                     
                        // Create a new custom list    
                        ListCreationInformation creationInfo = new ListCreationInformation();
                        creationInfo.Title = "Employees";
                        creationInfo.Description = "Contains Employee Lists";
                        creationInfo.TemplateType = (int)ListTemplateType.GenericList;
                       

                        List newList = clientContext.Web.Lists.Add(creationInfo);
                        // Display the custom list Title property    
                        newList.Fields.AddFieldAsXml("<Field Name='EmployeeID' ID='{558465e2-9775-4ee6-9186-a6fce9fb3ec5}' DisplayName='EmployeeID' Type='Number' />", true, AddFieldOptions.AddFieldInternalNameHint);
                        newList.Fields.AddFieldAsXml("<Field Name='NationalID' ID='{13c4eee4-52a6-4033-8317-81b7f2186cfc}' DisplayName='NationalID' Type='Text' />", true, AddFieldOptions.AddFieldInternalNameHint);
                        newList.Fields.AddFieldAsXml("<Field Name='Name1' ID='{5e841fbb-bbb6-4a8a-87b2-97785b8630d6}' DisplayName='Name' Type='Text' />", true, AddFieldOptions.AddFieldInternalNameHint);
                        newList.Fields.AddFieldAsXml("<Field Name='JobTitle' ID='{c4e0f350-52cc-4ede-904c-dd71a3d11f7d}' SourceID='http://schemas.microsoft.com/sharepoint/v3' StaticName='JobTitle' Group='$Resources:core,Person_Event_Columns;' DisplayName='$Resources:core,Job_Title;' Type='Text' DelayActivateTemplateBinding='GROUP,SPSPERS,SITEPAGEPUBLISHING' />", true, AddFieldOptions.AddFieldInternalNameHint);

                        // Execute the query to the server.    
                        clientContext.ExecuteQuery();

                        // Hide title field
                        var titleFieldInDefaultView = newList.Fields.GetByTitle("Title");
                        clientContext.Load(titleFieldInDefaultView);
                        clientContext.ExecuteQuery();

                        titleFieldInDefaultView.Hidden = true;
                        titleFieldInDefaultView.Update();
                        clientContext.ExecuteQuery();

                        // Remove Title View fields out of default field
                        Microsoft.SharePoint.Client.View defaultView = newList.DefaultView;
                        ViewFieldCollection viewFieldCollection = defaultView.ViewFields;
                        clientContext.Load(viewFieldCollection);
                        clientContext.ExecuteQuery();

                        if (defaultView.ViewFields.Contains("LinkTitle"))
                        {
                            defaultView.ViewFields.Remove("LinkTitle");
                            defaultView.Update();
                            clientContext.ExecuteQuery();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return Json(result);
            }

            return Json(result);
        }

        [HttpPost]
        [Route("publish-employee-item")]
        public IHttpActionResult AddEmployeeItem(EmployeeModel employeeModel)
        {
            GeneralResponseMessage msg = new GeneralResponseMessage();
            try
            {
                using (var clientContext = WebAPIHelper.GetClientContext(this.ControllerContext))
                {
                    if (clientContext != null)
                    {
                        var web = clientContext.Web;
                        msg = employeeService.AddEmployeeListItemToSharepoint(clientContext, employeeModel);
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Success = false;
                msg.Message = ex.Message;
                return Json(msg);
            }

            return Json(msg);
        }

        [HttpPost]
        [Route("delete-employee-item")]
        public IHttpActionResult RemoveEmployeeItem(EmployeeModel employeeModel)
        {                
            GeneralResponseMessage msg = new GeneralResponseMessage();

            try
            {
                using (var clientContext = WebAPIHelper.GetClientContext(this.ControllerContext))
                {
                    if (clientContext != null)
                    {
                        var web = clientContext.Web;
                        msg = employeeService.RemoveEmployeeListItemOnSharepoint(clientContext, employeeModel);
                    }
                }
            }
            catch (Exception ex)
            {
                msg.Success = false;
                msg.Message = ex.Message;
                return Json(msg);
            }
            return Json(msg);
        }
    }
}