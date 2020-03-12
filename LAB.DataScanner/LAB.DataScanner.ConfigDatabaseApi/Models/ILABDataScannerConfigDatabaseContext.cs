namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    using Microsoft.EntityFrameworkCore;

    public interface ILABDataScannerConfigDatabaseContext
    {
        DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        DbSet<ApplicationType> ApplicationTypes { get; set; }
        DbSet<Binding> Bindings { get; set; }
    }
}
