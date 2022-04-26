using Application.Common.Identities;
using Application.Common.interfaces;
using Domain.Constants;
using Infrastructure.DataAccess;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;
using System.Text;
using WebUI.RealTime.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Employees App APIS",
        Version = "v1",
        Contact = new OpenApiContact()
        {
            Name = "Krishna Bogati",
            Email = "invincible.impervious@gmail.com",
            Url = new Uri("https://www.instagram.com/invincible_system/")
        },
        Description = "This is just to test anything that comes up in my mind..."
    });
    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",

    });
    config.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", config =>
{
    config.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod()
          .SetIsOriginAllowed(origin => true);

}));

builder.Services.AddSignalR();

builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.SetMinimumLevel(LogLevel.Trace);
    options.AddNLog("nlog.config");
});


/*Start Adding Connection String here*/
var connectionString = builder.Configuration.GetConnectionString("AppDb");
builder.Services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));
/*End of Adding Connection string section*/
builder.Services.AddScoped<IApplicationDbContext>(p => p.GetService<ApplicationDbContext>());
var assembly = AppDomain.CurrentDomain.Load("Application");

builder.Services.AddMediatR(assembly);

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.RequireAuthenticatedSignIn = false;
})
       // Adding Jwt Bearer
       .AddJwtBearer(options =>
       {
           options.SaveToken = true;
           options.RequireHttpsMetadata = false;
           options.TokenValidationParameters = new TokenValidationParameters()
           {
               // The signing key must match!
               ValidateIssuerSigningKey = true,
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:JwtKey"])),
               ValidateIssuer = false,
               ValidateAudience = false,
               ValidAudience = builder.Configuration["Tokens:JwtAudience"],
               ValidIssuer = builder.Configuration["Tokens:JwtIssuer"]
           };
       });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(Policies.AdminPolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppRoles.Admin);
    });

    options.AddPolicy(Policies.UserPolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppRoles.NormalUser);
    });
    options.AddPolicy(Policies.PublicPolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(AppRoles.Admin, AppRoles.NormalUser);
    });
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (context.Database.IsSqlServer())
        {
            context.Database.Migrate();
        }
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

        await ApplicationDbContextSeed.SeedDefaultUserAsync(userManager, roleManager, context);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "An error occurred while migrating or seeding the database.");

        throw;
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(a =>
    {
        a.SwaggerEndpoint("v1/swagger.json", "Testing API V1.0.0");
        a.EnablePersistAuthorization();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<NotificationHub>("/notify");


app.Run();