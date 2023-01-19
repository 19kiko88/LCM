using ElmahCore;
using ElmahCore.Mvc;
using LCM.Repositories;
using LCM.Services.Implements;
using LCM.Services.Interfaces;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*Add Authentication (驗證)*/
builder.Services.AddAuthentication(IISDefaults.AuthenticationScheme);
builder.Services.AddAuthentication(Microsoft.AspNetCore.Authentication.Negotiate.NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*Add DB Context*/
builder.Services.AddDbContext<CAEDB01Context>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("CAEDB01Connection")));
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IDataService, DataService>();


//Ref：https://github.com/ElmahCore/ElmahCore
builder.Services.AddElmah(options =>
{
    options.Path = "lcm_elmah";
    options.OnPermissionCheck = Context => Context.User.Identity.IsAuthenticated;
    //options.LogPath = "~/logs";
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
    //套用CORS
    app.UseCors("allowCors");
}

/*授權驗證*/
app.UseAuthorization();//Middleware啟用授權功能，要搭配[Authenrization]，沒加上[Authenrization]還是不會進行授權驗證

//ELMAH must bwtween UseAuthorization and UseEndpoints
app.UseElmah();

//不管有無[Authenrization]，都會進行授權驗證
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers().RequireAuthorization();
});

app.MapControllers();

app.Run();
