namespace LAB.DataScanner.Components.Persisters.SimpleDataPersister
{
    using LAB.DataScanner.Components.Persisters.SimpleDataPersister.Model;
    using Microsoft.EntityFrameworkCore;

    public class PersisterContext : DbContext
    {

        public DbSet<PersisterEntry> Entries { get; set; }

        private readonly DataSchema dataSchema;

        public PersisterContext(DataSchema dataSchema, DbContextOptions<PersisterContext> options)
            : base(options)
        {
            this.dataSchema = dataSchema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entity = modelBuilder.Entity<PersisterEntry>().ToTable(this.dataSchema.TableName);

            foreach (var item in this.dataSchema.Header.Keys)
            {
                entity.Property<string>(item);
            }
        }
    }
}
