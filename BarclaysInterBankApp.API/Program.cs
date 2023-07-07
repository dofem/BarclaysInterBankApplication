using BarclaysInterBankApp.Application;
using BarclaysInterBankApp.Application.Utility;
using BarclaysInterBankApp.Infastructure;
using BarclaysInterBankApp.Infastructure.DataAccess;
using BarclaysInterBankApp.Infastructure.EmailUtility;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json",optional:false,reloadOnChange: true)
    .Build();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();



// Add services to the container.
builder.Services.AddApplicationService();
builder.Services.AddinfrastructureService();
builder.Services.AddControllers();
builder.Services.Configure<PaystackSettings>(configuration.GetSection("PaystackSettings"));
builder.Services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("BarclaysBankDb"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(logging =>
{
    logging.AddSerilog();
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthorization();

app.MapControllers();

app.Run();
