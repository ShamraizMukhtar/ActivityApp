using Microsoft.EntityFrameworkCore;
using Persistence;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//database connection
//for sql server the connection string is this
////"Data Source={PCNAME}\\SQLEXPRESS;Initial Catalog={DATABASENAME};Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False"

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});




var app = builder.Build();
//code for migration and creating db first time if not created
using var scope = app.Services.CreateScope();
var sServices = scope.ServiceProvider;
try
{
    var context = sServices.GetRequiredService<DataContext>();
    context.Database.Migrate();
    await Seed.SeedData(context);
}
catch (Exception ex)
{
    var logger = sServices.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occord during migration");

}
//code end db migration


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers();

// app.UseEndpoints(endpoints=>
// {
// endpoints.MapControllers();
// });



app.Run();
