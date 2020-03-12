namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    using System.Linq;
    using LAB.DataScanner.ConfigDatabaseApi.Models;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationTypesController : BaseController<ApplicationType>
    {
        public ApplicationTypesController(ILABDataScannerConfigDatabaseContext context) : base(context)
        {
        }

        protected override DbSet<ApplicationType> Entities => this.Context.ApplicationTypes;

        protected override IQueryable<ApplicationType> GetByKey(string key)
        {
            return this.Context.ApplicationTypes.Where(w => w.TypeId == int.Parse(key));
        }
    }
}
