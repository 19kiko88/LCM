using ElmahCore;
using ElmahCore.Mvc;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;

using LCM.Repositories;
using LCM.Services.Models;
using LCM.Services.Implements;
using LCM.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppSettings.PathSettings>(builder.Configuration.GetSection(AppSettings._PathSettings));

/*Add Authentication (����)*/
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Negotiate.NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddControllers();
builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "wwwroot";
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*Add DB Context*/
builder.Services.AddDbContext<CAEDB01Context>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CAEDB01Connection")));
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IDataService, DataService>();


//Ref�Ghttps://github.com/ElmahCore/ElmahCore
builder.Services.AddElmah<XmlFileErrorLog>(options =>
{
    options.Path = "ElmahWebLogs";
    options.LogPath = "./logs/ElmahXmlLogs";//�M��UseSpa�A�w�]�ؿ���wwwroot
    var allowedUsers = new string[] { "ASUS\\Abel_Hsu", "ASUS\\Bruenor_Hsu", "ASUS\\Homer_Chen" };
    options.OnPermissionCheck = Context =>
    {
        if (Context.User.Identity.IsAuthenticated && allowedUsers.Contains(Context.User.Identity.Name, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }
        else
        {
            return false;
        }
    };
});


/*CORS*/
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("allowCors",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
        });
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

if (app.Environment.IsDevelopment())
{
    //�M��CORS
    app.UseCors("allowCors");
}

/*���v����*/
app.UseAuthorization();//Middleware�ҥα��v�\��A�n�f�t[Authorize]�A�S�[�W[Authorize]�٬O���|�i����v����

//ELMAH must bwtween UseAuthorization and UseEndpoints
app.UseElmah();

//���ަ��L[Authenrization]�A���|�i����v����
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});

app.MapControllers();

//Setting SPA
app.UseSpaStaticFiles();
app.UseSpa(spa =>
{
    spa.Options.SourcePath = $"wwwroot";
    if (app.Environment.IsDevelopment())
    {
        spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
    }
});

app.Run();
