using Microsoft.EntityFrameworkCore;
using NodeNetworking.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLedger();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var switchMappings = new Dictionary<string, string>() {
    { "-p", "port" },
};
builder.Configuration.AddCommandLine(args, switchMappings);

// Add GossipProtocolOptions from Configuration (appsettings.json)
builder.Services.Configure<GossipProtocolOptions>(
    builder.Configuration.GetSection("GossipProtocol"));

builder.Services.AddGossipProtocol();

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
