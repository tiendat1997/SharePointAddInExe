using SharePointAddInAssignment.Entities;
using SharePointAddInAssignment.Repositories;
using SharePointAddInAssignment.Services;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using Unity;
using Unity.Lifetime;
using Unity.WebApi;

namespace SharePointAddInAssignmentWeb
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            container
               .RegisterType<DbContext, AdventureWorkEntity>(new ContainerControlledLifetimeManager())
               .RegisterType<IUnitOfWork, UnitOfWork>(new PerThreadLifetimeManager())
               .RegisterType<IEmployeeRepository, EmployeeRepository>(new TransientLifetimeManager())
               .RegisterType<IEmployeeService, EmployeeService>(new TransientLifetimeManager());

            DependencyResolver.SetResolver(new Unity.AspNet.Mvc.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}