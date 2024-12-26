using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddDbContext<ApplicationContext>(options => options
        .UseNpgsql("Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres")
        .UseSnakeCaseNamingConvention());

builder.Services.AddHostedService<TransactionProducer>();

var host = builder.Build();

host.Services.GetRequiredService<ApplicationContext>().Database.EnsureCreated();

await host.RunAsync();

public record Transaction(string From, string To, string Currency, decimal Amount, DateTimeOffset Timestamp);

public class ApplicationContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>().HasKey(m => m.Timestamp);
    }
}

public class TransactionProducer(IServiceProvider serviceProvider, ILogger<TransactionProducer> logger) : BackgroundService
{
    private static readonly Random Random = new();
    private static readonly string[] Currencies = ["USD", "EUR", "GBP", "JPY", "CNY"];
    private static readonly string[] Accounts = ["Cash", "Investment", "Credit Card", "Loan", "Savings"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var from = Accounts[Random.Next(Accounts.Length)];
            var transaction = new Transaction(
                From: from,
                To: Accounts.Where(a => a != from).OrderBy(_ => Random.Next()).First(),
                Currency: Currencies[Random.Next(Currencies.Length)],
                Amount: Random.Next(1000),
                Timestamp: DateTimeOffset.UtcNow);

            context.Transactions.Add(transaction);
            await context.SaveChangesAsync(stoppingToken);

            logger.LogInformation("Transaction added: {Transaction}", transaction);

            await Task.Delay(1000, stoppingToken);
        }
    }
}
