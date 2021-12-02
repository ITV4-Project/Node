using Microsoft.EntityFrameworkCore;
using NodeNetworking;
using NodeNetworking.NodeNetworking.DependencyInjection;
using NodeWebApi.Entities;
using NodeWebApi.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<NodeContext>(options =>
{
    options.UseSqlite("db.db");
});

builder.Services.AddRepositories();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
