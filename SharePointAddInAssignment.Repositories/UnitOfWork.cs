using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharePointAddInAssignment.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository EmployeeRepository { get; }
        void Commit();
    }
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext context;
        private IEmployeeRepository employeeRepository;

        public IEmployeeRepository EmployeeRepository
        {
            get
            {
                return employeeRepository ?? (employeeRepository = new EmployeeRepository(context));
            }
        }

        public UnitOfWork(DbContext context)
        {
            this.context = context;
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Commit()
        {
            var entries = context.ChangeTracker
                                        .Entries()
                                        .Where(e => e.State != EntityState.Unchanged)
                                        .ToList();
            // track if nothing changes
            if (entries.Count == 0)
            {
                throw new Exception("Problem unhandled DbContext");
            }
            context.SaveChanges();
        }
    }
}
