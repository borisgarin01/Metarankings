using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
                      policy =>
                      {
                          policy.AllowAnyHeader()
                          .AllowAnyOrigin()
                          .AllowAnyMethod();
                      });
});

builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();


app.MapControllers();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.Run();