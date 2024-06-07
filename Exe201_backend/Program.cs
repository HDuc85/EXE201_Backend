using Exe201_backend;
using Microsoft.OpenApi.Models;
using Service.Service;
using Service.Interface;
using System.Reflection;
using FluentValidation;
using System;
using Data.ViewModel.Authen;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Data.ViewModel.Helper;
using Service.Helper;
using Service.Repo;
using Service.Service.System.Product;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();


builder.Services.AddSwaggerGen(options =>
{
    options.UseDateOnlyTimeOnlyStringConverters();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        In = ParameterLocation.Header,
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Input only token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },


            },
            new List<string>()
          }
        });


    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


builder.Services.AddAuthorization();

// Add database 
builder.Services.AddDatabase();

//Add Identity
builder.Services.AddIdentity<User, Role>(options => {
    options.SignIn.RequireConfirmedEmail = true;
})
             .AddEntityFrameworkStores<PostgresContext>()
             .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromMinutes(15);
});


// Add Authen
builder.Services.AddTokenBearer();

//Add validator extension
builder.Services.AddValidatorsFromAssemblyContaining<StartupBase>();


builder.Services.AddMvc();
//builder.Services.AddControllers();
builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });

//Add EmailConfig 
builder.Services.AddEmailConfig();
builder.Services.AddScoped<IRepository<Product>, Repository<Product>>();
builder.Services.AddScoped<IRepository<Size>, Repository<Size>>();
builder.Services.AddScoped<IRepository<Brand>, Repository<Brand>>();
builder.Services.AddScoped<IRepository<Color>, Repository<Color>>();
builder.Services.AddScoped<IRepository<ProductVariant>, Repository<ProductVariant>>();
//Add Service
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
//builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISizeService, SizeService>();
builder.Services.AddScoped<ITokenHandler, TokenHandler>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<IEmailTemplateReader, EmailTemplateReader>();

builder.Services.AddScoped<IMediaHelper, MediaHelper>();

builder.Services.AddScoped<IBoxService, BoxService>();


builder.Services.AddScoped<PasswordHasher<User>>();
//Add validator
builder.Services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();




app.MapControllers();
app.UseRouting();
app.MapDefaultControllerRoute();

app.UseAuthorization();
app.UseAuthentication();
app.Run();
