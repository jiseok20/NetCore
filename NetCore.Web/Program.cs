using System.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Utilities.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//User �������̽��� UserService Ŭ������ �ޱ� ���� Services�� ����ؾ� ��.
builder.Services.AddScoped<IUser,UserService>();

//DB��������, Migrations ������Ʈ ���� CodeFirstDbContext
builder.Services.AddDbContext<CodeFirstDbContext>(options => options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DefaultConnection"), 
                                                                                  sqlServerOptionsAction: mig => mig.MigrationsAssembly(assemblyName: "NetCore.Migrations")));
//DB���������� DBFirstDbContext
builder.Services.AddDbContext<DBFirstDbContext>(options => options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString(name: "DBFirstDBConnection")));

//DataProtection , ���⿡ ���� �����ص� ��������� Utils.Common�� �Լ� ���� ������
Common.SetDataProtection(builder.Services, @"D:\DataProtector\", "NetCore", Enums.CryptoTpye.CngCbc);

//�ſ������� ���α���
builder.Services.AddAuthentication(defaultScheme : CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => 
                {
                    options.AccessDeniedPath = "/Membership/Forbidden";
                    options.LoginPath = "/Membership/Login";
                });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
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

//�ſ�������
app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
