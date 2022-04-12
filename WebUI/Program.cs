using Infrastructure.Common;
using Infrastructure.DataAccess;
using Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Extensions.Logging;

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
    });

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
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();