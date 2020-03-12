namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    using System.Linq;
    using LAB.DataScanner.ConfigDatabaseApi.Models;
    using Microsoft.AspNet.OData;
    using Microsoft.EntityFrameworkCore;

    public class BindingsController : BaseController<Binding>
    {
        public BindingsController(ILABDataScannerConfigDatabaseContext context ) : base(context)
        {
        }

        protected override DbSet<Binding> Entities => this.Context.Bindings;

        [EnableQuery(MaxExpansionDepth = 10)]
        public SingleResult<Binding> Get([FromODataUri] string publisherId, [FromODataUri] string consumerId)
        {
            return new SingleResult<Binding>(
                this.Context.Bindings.Where(w =>
                    w.PublisherInstanceId == int.Parse(publisherId)
                    &&
                    w.ConsumerInstanceId == int.Parse(consumerId)
                ));
        }

        protected override IQueryable<Binding> GetByKey(string key)
        {
            return this.Context.Bindings.Where(w => (w.PublisherInstanceId == int.Parse(key) || w.ConsumerInstanceId == int.Parse(key)));
        }
    }
}
