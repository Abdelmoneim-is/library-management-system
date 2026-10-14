using LibraryApplication;
using LibraryApplication.Profiles;
using LibraryInfrastructure;
using LibraryInfrastructure.Data;
using LibraryInfrastructure.Identity.Data;
using LibraryInfrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StoreDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddDbContext<StoreIdentityDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});

builder.Services.AddInfrastructureService(builder.Configuration);

// 2. ????? ????? ??? Application (???? ????? ??? IBookService ????? ????????)
builder.Services.ApplicationService();
builder.Services.Configure<URLSettings>(builder.Configuration.GetSection("URLSettings"));
// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<StoreIdentityDbContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // 1. تفعيل الـ Middleware الخاص بنا لمعالجة الأخطاء والـ Exceptions
    app.UseMiddleware<Admin.Dashboard.Middlewares.ExceptionHandlingMiddleware>();

    // 2. إعادة التوجيه لصفحات الأخطاء الشهيرة (زي 404)
    app.UseStatusCodePagesWithReExecute("/Error/NotFound");
}
else
{
    // في وضع الـ Development لو حابب تشوف الـ Developer Exception Page الحقيقية
    app.UseDeveloperExceptionPage();
    app.UseStatusCodePagesWithReExecute("/Error/NotFound");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Admin}/{action=Login}/{id?}");

app.Run();
