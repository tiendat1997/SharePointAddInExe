using SharePointAddInAssignment.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SharePointAddInAssignmentWeb.Controllers
{
    [RoutePrefix("api/employees")]
    public class EmployeeApiController : ApiController
    {
        private IEmployeeService employeeService;      
        public EmployeeApiController(IEmployeeService employeeService)
        {
            this.employeeService = employeeService;
        }
        [HttpGet]
        [Route("")]
        public IHttpActionResult Get() 
        {
            var results = employeeService.GetEmployees();
            return Json(results);
        }
    }
}
