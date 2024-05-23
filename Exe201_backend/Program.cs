using Exe201_backend;

using Service.Service.System.Authen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Service.Repo;
using Service.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase();
/*builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<PostgresContext>()
                .AddDefaultTokenProviders();*/
/*builder.Services.AddTransient<UserManager<User>, UserManager<User>>();
builder.Services.AddTransient<SignInManager<User>, SignInManager<User>>();*/

builder.Services.AddTransient<IAuthenService, AuthenService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




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
