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
using Service.Helper.Email;
using Service.Helper.Media;
using Service.Helper.Address;
using Exe201_backend.Middleware;


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
builder.Services.AddControllers();

//Add EmailConfig 
builder.Services.AddEmailConfig();

//Add FirebaseConfig
builder.Services.AddFirebaseConfig();

//Add Service
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IStoreService, StoreService>();

builder.Services.AddScoped<ITokenHandler, TokenHandler>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<IEmailTemplateReader, EmailTemplateReader>();
builder.Services.AddScoped<IMediaHelper, MediaHelper>();
builder.Services.AddScoped<IAddressHelper, AddressHelper>();


builder.Services.AddScoped<PasswordHasher<User>>();

//Add validator
builder.Services.AddScoped<IValidator<LoginRequest>, LoginValidator>();




var app = builder.Build();




// Configure the HTTP request pipeline.

    app.UseSwagger();
    app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthentication();


app.MapControllers();
app.UseRouting();
app.MapDefaultControllerRoute();

app.UseAuthorization();
app.UseMiddleware<BannedUserMiddleware>();

app.Run();