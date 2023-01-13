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


//builder.Services.Configure<IISServerOptions>(options =>
//{
//    options.MaxRequestBodySize = 2147483648;
//});

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
/*
app.UseEndpoints(endpoints =>
{//不管有無[Authenrization]，都會進行授權驗證
    endpoints.MapControllers().RequireAuthorization();
});
*/

app.MapControllers();

app.Run();
