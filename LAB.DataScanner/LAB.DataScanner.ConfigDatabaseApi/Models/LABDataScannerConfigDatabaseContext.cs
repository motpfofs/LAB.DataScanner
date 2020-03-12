namespace LAB.DataScanner.ConfigDatabaseApi.Models
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    public partial class LABDataScannerConfigDatabaseContext : DbContext, ILABDataScannerConfigDatabaseContext
    {
        public LABDataScannerConfigDatabaseContext()
        {
        }

        public LABDataScannerConfigDatabaseContext(DbContextOptions<LABDataScannerConfigDatabaseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationInstance> ApplicationInstances { get; set; }
        public virtual DbSet<ApplicationType> ApplicationTypes { get; set; }
        public virtual DbSet<Binding> Bindings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<ApplicationInstance>(entity =>
            {
                entity.HasKey(e => e.InstanceId)
                    .HasName("PK_component.ApplicationInstance");

                entity.ToTable("ApplicationInstance", "component");

                entity.Property(e => e.InstanceId).HasColumnName("InstanceID");

                entity.Property(e => e.ConfigJson).IsRequired();

                entity.Property(e => e.InstanceName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.ApplicationInstance)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_component.ApplicationInstance_meta.ApplicationType");
            });

            modelBuilder.Entity<ApplicationType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK_ApplicationType_TypeID");

                entity.ToTable("ApplicationType", "meta");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.ConfigTemplateJson).IsRequired();

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.TypeVersion)
                    .IsRequired()
                    .HasMaxLength(12);
            });

            modelBuilder.Entity<Binding>(entity =>
            {
                entity.HasKey(e => new { e.PublisherInstanceId, e.ConsumerInstanceId })
                    .HasName("PK__Binding__A15D3DE1D6853961");

                entity.ToTable("Binding", "binding");

                entity.Property(e => e.PublisherInstanceId).HasColumnName("PublisherInstanceID");

                entity.Property(e => e.ConsumerInstanceId).HasColumnName("ConsumerInstanceID");

                entity.HasOne(d => d.ConsumerInstance)
                    .WithMany(p => p.BindingConsumerInstance)
                    .HasForeignKey(d => d.ConsumerInstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK2_binding.Binding_component.ApplicationInstance");

                entity.HasOne(d => d.PublisherInstance)
                    .WithMany(p => p.BindingPublisherInstance)
                    .HasForeignKey(d => d.PublisherInstanceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK1_binding.Binding_component.ApplicationInstance");
            });
        }
    }
}
