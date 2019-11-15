using SharePointAddInAssignment.Services;
using SharePointAddInAssignment.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SharePointAddInAssignmentWeb.Controllers
{
    public class EmployeeController : Controller
    {
        private IEmployeeService employeeService;
        public EmployeeController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }

        // GET: Employee
        [SharePointContextFilter]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult PublishEmployeeItem(EmployeeModel employeeModel)
        {
            GeneralResponseMessage msg = new GeneralResponseMessage();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            try
            {
                using (var clientContext = spContext.CreateUserClientContextForSPHost())
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
        [SharePointContextFilter]
        public ActionResult RemoveEmployeeItem(EmployeeModel employeeModel)
        {

            GeneralResponseMessage msg = new GeneralResponseMessage();
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            try
            {
                using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
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