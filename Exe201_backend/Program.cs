using Exe201_backend;
using Microsoft.OpenApi.Models;

using Service.Service;
using Service.Interface;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// Add database 
builder.Services.AddDatabase();

// Add Authen
builder.Services.AddTokenBearer();




builder.Services.AddMvc();
builder.Services.AddControllers();

//Add Service
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenHandler,TokenHandler>();
builder.Services.AddScoped<IUserTokenService, UserTokenService>();






var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
