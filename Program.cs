var builder = WebApplication.CreateBuilder(args);
// Usuga MVC
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";  // gdzie przekierowa�, je�li nie zalogowany
        options.AccessDeniedPath = "/Account/AccessDenied"; // gdy brak uprawnie�
    });

builder.Services.AddAuthorization();
var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.Run();
