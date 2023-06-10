using SecureSend.Application;
using SecureSend.Infrastructure;
using SecureSend.Infrastructure.EF.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SqlServerOptions>(
    builder.Configuration.GetSection(nameof(SqlServerOptions)))
    .Configure<FileStorageOptions>(
    builder.Configuration.GetSection(nameof(FileStorageOptions)));


builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddSpaStaticFiles(config =>
{
    config.RootPath = "ClientApp/dist";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSpaStaticFiles();

app.UseSpa(config =>
{
    config.Options.SourcePath = "ClientApp";
});

app.UseInfrastructure();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
