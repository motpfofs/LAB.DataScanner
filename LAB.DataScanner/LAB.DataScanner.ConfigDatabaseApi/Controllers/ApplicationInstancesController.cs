namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    using System.Linq;
    using LAB.DataScanner.ConfigDatabaseApi.Models;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationInstancesController : BaseController<ApplicationInstance>
    {
        public ApplicationInstancesController(ILABDataScannerConfigDatabaseContext context) : base(context)
        {
        }

        protected override DbSet<ApplicationInstance> Entities => this.Context.ApplicationInstances;

        protected override IQueryable<ApplicationInstance> GetByKey(string key)
        {
            return this.Context.ApplicationInstances.Where(w => w.InstanceId == int.Parse(key));
        }
    }
}