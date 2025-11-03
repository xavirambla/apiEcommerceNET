
/*
configura servicios,
usar aplicación
construir y ejecutar servidor web

*/


using System.Text;
using ApiEcommerce.Constants;
using ApiEcommerce.Data;
using ApiEcommerce.Models;
using ApiEcommerce.Repository;
using ApiEcommerce.Repository.IRepository;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mapster;
using MapsterMapper;
using ApiEcommerce.Mapping;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication;

[assembly: InternalsVisibleTo("ApiEcommerce.Tests")]

//using ApiEcommerce.Mapping;

var builder = WebApplication.CreateBuilder(args);

//  --- Registro de servicios 


// Add services to the container.
var dbConnectionString = builder.Configuration.GetConnectionString("ConexionSql");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(dbConnectionString)
  .UseSeeding((context, _) =>
  {
      var appContext = (ApplicationDbContext)context;
      // Seeding de Roles
      DataSeeder.SeedData(appContext);
  })
);


// añadimos cache
builder.Services.AddResponseCaching(options =>
{
  options.MaximumBodySize = 1024 * 1024;
  options.UseCaseSensitivePaths = true;
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddMapster();


// Registrar los mapeos de Mapster
MapsterConfig.RegisterMappings();



/*
// Registrar mapeos personalizados
CategoryMappingConfig.RegisterCategoryMappings();
ProductMappingConfig.RegisterProductMappings();
UserMappingConfig.RegisterUserMappings();
*/
// Registrar Mapster como servicio de inyección de dependencias
builder.Services.AddSingleton(TypeAdapterConfig.GlobalSettings);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// autenticación y autorización con Identity
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var secretKey = builder.Configuration.GetValue<string>("ApiSettings:SecretKey");
if (string.IsNullOrEmpty(secretKey))
{
  throw new InvalidOperationException("SecretKey no esta configurada");
}


// configuración de autenticación JWT

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddAuthentication("FakeAuth")
        .AddScheme<AuthenticationSchemeOptions, FakeAuthHandler>("FakeAuth", options => { });
}
else
{

    builder.Services.AddAuthentication(
        options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        }
    ).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;   // desactiva necesidad de usar https. En producción debe estar a true
        options.SaveToken = true;    // guarda el token en el contexto de autenticación
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,   // validamos el token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),  // clave secreta para validar
            ValidateIssuer = false,  // no se validará el emisor del token
                                     //        ValidateAudience = true   // no se valida el público de token sino se necesita a ciertos clientes
            ValidateAudience = false   // no se valida el público de token sino se necesita a ciertos clientes

        };
    }
    );
}

builder.Services.AddControllers(
    option=>
    {
    option.CacheProfiles.Add(CacheProfiles.Default10, CacheProfiles.Profile10);
    option.CacheProfiles.Add(CacheProfiles.Default20, CacheProfiles.Profile20);
    }
);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
//builder.Services.AddSwaggerGen();



// 🔹 Swagger (Swashbuckle)
/*
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ApiEcommerce",
        Version = "v1",
        Description = "API para gestión de categorías y productos de ecommerce"
    });
});

*/
builder.Services.AddSwaggerGen(
  options =>
  {
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
      Description = "Nuestra API utiliza la Autenticación JWT usando el esquema Bearer. \n\r\n\r" +
                    "Ingresa la palabra a continuación el token generado en login.\n\r\n\r" +
                    "Ejemplo: \"12345abcdef\"",
      Name = "Authorization",
      In = ParameterLocation.Header,
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer"
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
          Scheme = "oauth2",
          Name = "Bearer",
          In = ParameterLocation.Header
        },
        new List<string>()
      }
    });

      //documentar versiones
      options.SwaggerDoc("v1", new OpenApiInfo
      {
          Version = "v1",      	
          Title = "ApiEcommerce",
          Description = "API para gestión de categorías, usuarios y productos de ecommerce",
          TermsOfService = new Uri("https://example.com/terms"),
          Contact = new OpenApiContact
          {
              Name = "Xavi",
              Email = "xavi@gmail.com",
              Url = new Uri("https://twitter.com/xavi")
          },
          License = new OpenApiLicense
          {
              Name = "Licencia de uso",
              Url = new Uri("https://example.com/license")
          }
      });
    
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "ApiEcommerce v2",
        Description = "API para gestión de categorías, usuarios y productos de ecommerce",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Xavi",
            Email = "xavi@gmail.com",
            Url = new Uri("https://twitter.com/xavi")
        },
        License = new OpenApiLicense
        {
            Name = "Licencia de uso",
            Url = new Uri("https://example.com/license")
        }
    });    

  }
);
// cuando tenemos varias versiones de las api
var apiVersioningBuilder = builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
options.ReportApiVersions = true;  // informa a los clientes de las versiones disponibles
    /*
    options.ApiVersionReader= ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version")   // lee la versión desde la query string
        // ?api-version
        
        );
        */
}
    );
apiVersioningBuilder.AddApiExplorer(
    options =>
    {
        options.GroupNameFormat = "'v'VVV";  // formato de grupo de versiones
        options.SubstituteApiVersionInUrl = true;  // sustituir la versión en la url    //api/v{version}/products
    }
    );


builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowSpecificOrigin",
    options.AddPolicy(PolicyNames.AllowSpecificOrigin,
    builder =>
    {
        //builder.WithOrigins("http://localhost:3000")
        //builder.WithOrigins("http://localhost:3000", "http://localhost:5000")
        builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();

    }
    );
  }
);


var app = builder.Build();   //instancia web application


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();   // genera archivos json para la configuración de Swagger

    // le decimos la versión de la documentación a utilizar
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "v2");
        //options.RoutePrefix = string.Empty; // Abre Swagger directamente en la raíz    // entonces hay que poner swagger en la raiz
    });

app.MapGet("/index.html", () => Results.Redirect("/swagger"));


//    app.UseSwaggerUI();  // habilitar interfaz visual de Swagger
  //  app.MapOpenApi();
}



app.UseHttpsRedirection();   // redirigir automaticamente http a https
app.UseStaticFiles();  // habilitar ficheros estáticos  wwwroot
//app.UseCors("AllowSpecificOrigin");
app.UseCors(PolicyNames.AllowSpecificOrigin);
app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();  // middleware de autorizacion , proteger endpoints

app.MapControllers();   // buscar clases 

app.Run(); 


public partial class Program { }