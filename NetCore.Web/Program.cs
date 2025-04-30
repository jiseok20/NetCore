using System.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//User 인터페이스가 UserService 클래스를 받기 위해 Services에 등록해야 함.
builder.Services.AddScoped<IUser,UserService>();

//DB접속정보, Migrations 프로젝트 지정 CodeFirstDbContext
builder.Services.AddDbContext<CodeFirstDbContext>(options => options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DefaultConnection"), 
                                                                                  sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));
//DB접속정보만 DBFirstDbContext
builder.Services.AddDbContext<DBFirstDbContext>(options => options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DBFirstDBConnection")));

//DataProtection , 여기에 직접 구현해도 상관없지만 Utils.Common에 함수 만들어서 적용함
Common.SetDataProtection(builder.Services, @"D:\DataProtector\", "NetCore", Enums.CryptoTpye.CngCbc);
                
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
