using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using SolidEdu.Shared;
using Ecommerce.WebApi.Repositories;
using Ecommerce.IdentityJWT.Authentication;
using IdentityServer4.AccessTokenValidation;

var builder = WebApplication.CreateBuilder(args);
/*<Integerated authentiacate by Dongbh*/
ConfigurationManager configuration = builder.Configuration;
/*Create connect to DB via Entity Framework core*/
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("ConnStrSQLServerDB")));

/*For Identity*/
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

/*Add authentication*/
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddIdentityServerAuthentication("Bearer", options =>
{
    options.ApiName = "customerApi";
    options.Authority = "https://localhost:7241";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("customerApiReadPolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "customerApi.read");
    });

    options.AddPolicy("customerApiWritePolicy", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "customerApi.write");
    });
});

/*</Integerated authentiacate by Dongbh*/


//Enable CORS to protected APIs by Dongbh
builder.Services.AddCors();

// Add services to the container. (connect to sql server)
builder.Services.AddEcommerceContext();


builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setup =>
{
    // Include 'SecurityScheme' to use JWT Authentication
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();//newed (IoC)
var app = builder.Build();

app.UseCors(configurePolicy: options =>
{
	options.WithMethods("GET", "POST", "PUT", "DELETE");
	options.WithOrigins(
	"https://localhost:5002" // allow requests from the client (diff domain)
	);
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
	app.UseSwaggerUI(c =>
	{
		c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ecommerce Service API Version 1");
	});

}

app.UseHttpsRedirection();
app.UseRouting();

//Authentication by Dongbh
app.UseAuthentication();
app.UseAuthorization();
//security header https
app.UseMiddleware<SecurityHeaders>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
