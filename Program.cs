using Microsoft.AspNetCore.Http;
using tl2_tp8_2025_BautistaAlvarez.Repositories;
using tl2_tp8_2025_BautistaAlvarez.Interfaces;
using tl2_tp8_2025_BautistaAlvarez.Services;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//agrego esto--------------
builder.Services.AddDistributedMemoryCache();
// Servicios de Sesión y Acceso a Contexto (CLAVE para la autenticación)
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
    {
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    });
// Registro de la Inyección de Dependencia (TODOS AddScoped)
builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IPresupuestosRepository, PresupuestosRepository>();
builder.Services.AddScoped<IUserRepository, UsuarioRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

//------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
// Configuración del Pipeline de Middleware
// El orden es CRUCIAL: UseSession debe ir ANTES de UseRouting/UseAuthorization
app.UseSession();//------ agrego tambien y tiene que estar en ese orden: routing, session y authorizataion----------------

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();// Necesario si usa atributos, aunque aquí lo haremos manual

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
