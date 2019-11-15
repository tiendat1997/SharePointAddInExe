using Microsoft.SharePoint.Client;
using SharePointAddInAssignment.Repositories;
using SharePointAddInAssignment.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointAddInAssignment.Services
{
    public interface IEmployeeService
    {
        bool IsExsitedOnSharepoint(ClientContext clientContext, EmployeeModel item);
        GeneralResponseMessage AddEmployeeListItemToSharepoint(ClientContext clientContext, EmployeeModel item);
        List<EmployeeModel> GetEmployees();
        GeneralResponseMessage RemoveEmployeeListItemOnSharepoint(ClientContext clientContext, EmployeeModel employeeModel);
    }
    public class EmployeeService : IEmployeeService
    {
        private IUnitOfWork unitOfWork;
        public EmployeeService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public GeneralResponseMessage AddEmployeeListItemToSharepoint(ClientContext clientContext, EmployeeModel employeeModel)
        {
            GeneralResponseMessage responseMsg = new GeneralResponseMessage { Success = true, Message = "Add Employee Item Succesfully" };
            var employeeList = clientContext.Web.Lists.GetByTitle("Employees");
            
            if (IsExsitedOnSharepoint(clientContext, employeeModel) == false)
            {
                ListItemCreationInformation itemCreateInfo = new ListItemCreationInformation();
                ListItem newItem = employeeList.AddItem(itemCreateInfo);
                newItem["EmployeeID"] = employeeModel.ID;
                newItem["NationalID"] = employeeModel.NationalID;
                newItem["Name1"] = employeeModel.Name;
                newItem["JobTitle"] = employeeModel.JobTitle;
                newItem.Update();
                clientContext.ExecuteQuery();
            }
            else
            {
                responseMsg.Success = false;
                responseMsg.Message = "Employee is already existed in sharepoint list";
            }
            return responseMsg;
        }

        public List<EmployeeModel> GetEmployees()
        {
            var employees = unitOfWork.EmployeeRepository
                                .GetAll().Take(10)
                                .Select(e => new EmployeeModel
                                {
                                    ID = e.BusinessEntityID,
                                    NationalID = e.NationalIDNumber,
                                    JobTitle = e.JobTitle,
                                    Name = e.Person.FirstName + " " + e.Person.LastName + " " + e.Person.MiddleName
                                }).ToList();
            return employees;
        }

        public bool IsExsitedOnSharepoint(ClientContext clientContext, EmployeeModel employeeModel)
        {
            var employeeList = clientContext.Web.Lists.GetByTitle("Employees");
            CamlQuery camlQuery = new CamlQuery();
            camlQuery.ViewXml = $"<View>  " +
                                 $"	<Query>  " +
                                 $"		<Where>  " +
                                 $"			<Eq>  " +
                                 $"				<FieldRef Name='EmployeeID'/>  " +
                                 $"				<Value Type='Number'>{employeeModel.ID.ToString()}</Value>  " +
                                 $"			</Eq>  " +
                                 $"		</Where>  " +
                                 $"	</Query>  " +
                                 $"	<RowLimit>1</RowLimit>  " +
                                 $" </View>  ";
            var listItems = employeeList.GetItems(camlQuery);
            clientContext.Load(listItems);
            clientContext.ExecuteQuery();
            return listItems.Count > 0;
        }

        public GeneralResponseMessage RemoveEmployeeListItemOnSharepoint(ClientContext clientContext, EmployeeModel employeeModel)
        {
            GeneralResponseMessage responseMsg = new GeneralResponseMessage { Success = true, Message = "Remove Employee Item Succesfully" };
            var employeeList = clientContext.Web.Lists.GetByTitle("Employees");

            if (IsExsitedOnSharepoint(clientContext, employeeModel))
            {
                CamlQuery camlQuery = new CamlQuery();
                camlQuery.ViewXml = $"<View>  " +
                                     $"	<Query>  " +
                                     $"		<Where>  " +
                                     $"			<Eq>  " +
                                     $"				<FieldRef Name='EmployeeID'/>  " +
                                     $"				<Value Type='Number'>{employeeModel.ID.ToString()}</Value>  " +
                                     $"			</Eq>  " +
                                     $"		</Where>  " +
                                     $"	</Query>  " +
                                     $"	<RowLimit>1</RowLimit>  " +
                                     $" </View>  ";
                var listItems = employeeList.GetItems(camlQuery);
                clientContext.Load(listItems);
                clientContext.ExecuteQuery();

                var foundItem = listItems.FirstOrDefault();

                foundItem.DeleteObject();
                clientContext.ExecuteQuery();
            }
            else
            {
                responseMsg.Success = false;
                responseMsg.Message = "Employee Item is not existed on sharepoint";
            }
            return responseMsg;
        }
    }
}
