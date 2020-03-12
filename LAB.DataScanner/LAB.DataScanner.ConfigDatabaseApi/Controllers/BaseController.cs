namespace LAB.DataScanner.ConfigDatabaseApi.Controllers
{
    using System.Linq;
    using System.Net;
    using LAB.DataScanner.ConfigDatabaseApi.Models;
    using Microsoft.AspNet.OData;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    public abstract class BaseController<T> : ODataController where T : class
    {
        protected LABDataScannerConfigDatabaseContext Context { get; }

        protected BaseController(ILABDataScannerConfigDatabaseContext context)
        {
            this.Context = (LABDataScannerConfigDatabaseContext)context;
        }

        protected abstract DbSet<T> Entities { get; }

        protected abstract IQueryable<T> GetByKey(string key);

        [EnableQuery(MaxExpansionDepth = 10)]
        public virtual IQueryable<T> Get()
        {
            return this.Entities.AsQueryable();
        }

        [EnableQuery(MaxExpansionDepth = 10)]
        public virtual SingleResult<T> Get([FromODataUri] string key)
        {
            return SingleResult.Create<T>(this.GetByKey(key));
        }

        public virtual IActionResult Post (T entry)
        {
            if(!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            this.Entities.Add(entry);
            this.Context.SaveChanges();

            return Created(entry);
        }

        public virtual IActionResult Patch([FromODataUri] string key, Delta<T> entityDelta)
        {
            if(!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var entity = this.GetByKey(key).FirstOrDefault();
            if(entity ==null)
            {
                return this.NotFound();
            }

            entityDelta.Patch(entity);
            this.Context.SaveChanges();

            return this.Updated(entity);
        }

        public virtual IActionResult Delete([FromODataUri] string key)
        {
            var entity = this.GetByKey(key).FirstOrDefault();
            if(entity == null)
            {
                return this.NotFound();
            }

            this.Entities.Remove(entity);
            this.Context.SaveChanges();
            return this.StatusCode((int)HttpStatusCode.NoContent);
        }

    }
}
