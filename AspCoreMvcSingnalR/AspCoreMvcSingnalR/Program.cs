using AspCoreMvcSingnalR.DatabaseEntity;
using AspCoreMvcSingnalR.SignalRHub;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(x => x.LoginPath = "/Home/Index"); //In case of unauhroize user clear auth cookis
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();
builder.Services.AddSingleton<RealTimeChatHub>();
builder.Services.AddDbContext<ChatDbContext>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();
app.MapHub<RealTimeChatHub>("/RealTimeChatHub");

app.Run();
