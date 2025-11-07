using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UmaPay.Interface.Repository;
using UmaPay.Repository.Entities;

namespace UmaPay.Repository
{
    public class DataContext : DbContext, IUnitOfWork
    {
        #region Properties

        private readonly IConfiguration _configuration;

        #endregion Properties

        #region Constrcutor   

        //public DataContext(){}

        public DataContext(DbContextOptions<DataContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }


        #endregion Constrcutor

        #region DbSet

        public DbSet<Application> Applications { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Gateway> Gateways { get; set; }
        public DbSet<GatewayApplication> GatewayApplications { get; set; }
        public DbSet<GatewayCountry> GatewayCountries { get; set; }
        public DbSet<TransactionInvoice> TransactionInvoices { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionStatus> TransactionStatuses { get; set; }
        public DbSet<TransactionStatusLog> TransactionStatusLogs { get; set; }

        #endregion DbSet

        #region override
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DataConnection"),
                     sqlServerOptionsAction: sqlOptions =>
                     {
                         sqlOptions.EnableRetryOnFailure(
                             maxRetryCount: 10,
                             maxRetryDelay: TimeSpan.FromSeconds(30),
                             errorNumbersToAdd: null);
                     });
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones adicionales de entidades si son necesarias

            // Configuración de Transaction
            modelBuilder.Entity<Transaction>()
                .HasMany<TransactionInvoice>()
                .WithOne(ti => ti.Transaction)
                .HasForeignKey(ti => ti.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.TransactionStatuses)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Customers)
                .WithMany()
                .HasForeignKey(t => t.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.GatewayApplications)
                .WithMany()
                .HasForeignKey(t => t.GatewayApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Countries)
                .WithMany()
                .HasForeignKey(t => t.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de TransactionInvoice
            modelBuilder.Entity<TransactionInvoice>()
                .HasOne(ti => ti.TransactionStatuses)
                .WithMany()
                .HasForeignKey(ti => ti.StatusId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<GatewayApplication>()
                .HasOne(ga => ga.Gateways)
                .WithMany(g => g.GatewayApplications)
                .HasForeignKey(ga => ga.GatewayId);

            modelBuilder.Entity<GatewayApplication>()
                .HasOne(ga => ga.Applications)
                .WithMany(a => a.GatewayApplications)
                .HasForeignKey(ga => ga.ApplicationId);


            modelBuilder.Entity<TransactionStatusLog>()
                .HasOne(t => t.Transactions)
                .WithMany()
                .HasForeignKey(t => t.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransactionStatusLog>()
                .HasOne(t => t.TransactionStatuses)
                .WithMany()
                .HasForeignKey(t => t.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Puedes agregar más configuraciones de relaciones aquí si es necesario
        }

        #endregion override
    }
}