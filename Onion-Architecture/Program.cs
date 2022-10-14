using Microsoft.EntityFrameworkCore;
using OA.Repository;
using OA.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection"), b=>b.MigrationsAssembly("OA.Repository")
    ));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddTransient<IUserService , UserService>();

builder.Services.AddTransient<IUserInfoService , UserInfoService>();

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
