using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.WebAPI;
using SharePointAddInAssignmentWeb.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharePointAddInAssignmentWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {

            User spUser = null;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            // Register the BusinessDocuments API	      
            WebAPIHelper.RegisterWebAPIService(this.HttpContext, "/api/provision/createnewpage");
            WebAPIHelper.RegisterWebAPIService(this.HttpContext, "/api/provision/uploadfile");

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }

            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
