using Exe201_backend;
using Service.Service.System.Authen;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Service.Repo;
using Service.Models;
using Service.Service.System.Product;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabase();
/*builder.Services.AddIdentity<User, Role>()
                .AddEntityFrameworkStores<PostgresContext>()
                .AddDefaultTokenProviders();*/
/*builder.Services.AddTransient<UserManager<User>, UserManager<User>>();
builder.Services.AddTransient<SignInManager<User>, SignInManager<User>>();*/

builder.Services.AddTransient<IAuthenService, AuthenService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddControllers();
builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
            });
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
builder.Services.AddIdentity<User, Role>( options => {
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
builder.Services.AddScoped<ITokenHandler,TokenHandler>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();
builder.Services.AddScoped<IEmailTemplateReader, EmailTemplateReader>();
builder.Services.AddScoped<IMediaHelper, MediaHelper>();


builder.Services.AddScoped<PasswordHasher<User>>();

//Add validator
builder.Services.AddScoped<IValidator<LoginRequest>, LoginValidator>();

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
