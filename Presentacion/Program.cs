using Infraestructura.ServiciosExternos;
using LogicaAccesoDatos.EF;
using LogicaAccesoDatos.EF.Repositorios;
using LogicaAplicacion.CasosDeUso.AuditoriaPrestamos;
using LogicaAplicacion.CasosDeUso.Equipos;
using LogicaAplicacion.CasosDeUso.ObjetosCelestes;
using LogicaAplicacion.CasosDeUso.Observaciones;
using LogicaAplicacion.CasosDeUso.Prestamos;
using LogicaAplicacion.CasosDeUso.Usuarios;
using LogicaAplicacion.InterfacesCasosDeUso;
using LogicaAplicacion.InterfacesCasosDeUso.AuditoriaPrestamos;
using LogicaAplicacion.InterfacesCasosDeUso.Equipos;
using LogicaAplicacion.InterfacesCasosDeUso.ObjetosCelestes;
using LogicaAplicacion.InterfacesCasosDeUso.Observaciones;
using LogicaAplicacion.InterfacesCasosDeUso.Prestamos;
using LogicaAplicacion.InterfacesCasosDeUso.Usuarios;
using LogicaAplicacion.InterfacesServicios;
using StellarMinds.InterfacesRepositorios;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<ManagerContext>();

// Inicializamos Repositorios
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();
builder.Services.AddScoped<IRepositorioEquipo, RepositorioEquipo>();
builder.Services.AddScoped<IRepositorioPrestamo, RepositorioPrestamo>();
builder.Services.AddScoped<IRepositorioObservacion, RepositorioObservacion>();
builder.Services.AddScoped<IRepositorioAuditadoPrestamo, RepositorioAuditadoPrestamo>();
builder.Services.AddScoped<IRepositorioObjetoCeleste, RepositorioObjetoCeleste>();

// Registrar Casos de Uso
// Usuarios
builder.Services.AddScoped<IAgregarUsuario, AgregarUsuarioCU>();
builder.Services.AddScoped<ILoginUsuario, LoginUsuarioCu>();
builder.Services.AddScoped<IObtenerTodosUsuarios, ObtenerTodosUsuariosCU>();

// Equipos
builder.Services.AddScoped<IAgregarEquipo, AgregarEquipoCU>();
builder.Services.AddScoped<IRemoverEquipo, EliminarEquipoCU>();
builder.Services.AddScoped<IActualizarEquipo, ActualizarEquipoCU>();
builder.Services.AddScoped<IObtenerTodosEquipos, ObtenerTodosEquiposCU>();
builder.Services.AddScoped<IObtenerEquipoPorId, ObtenerEquipoPorIdCU>();
builder.Services.AddScoped<IObtenerTodosEquiposDisponibles, ObtenerTodosEquiposDisponiblesCU>();

// Prestamo
builder.Services.AddScoped<IAltaPrestamo, AltaPrestamoCU>();
builder.Services.AddScoped<IObtenerPrestamosUsuario, ObtenerPrestamosUsuarioCU>();
builder.Services.AddScoped<IDevolverPrestamo, DevolverPrestamoCU>();
builder.Services.AddScoped<IObtenerPrestamosVigentes, IObtenerPrestamosVigentes>();
builder.Services.AddScoped<IObtenerPrestamoPorId, ObtenerPrestamoPorId>();
builder.Services.AddScoped<IObtenerPrestamosEnPeriodo, ObtenerPrestamosEnPeriodo>();

// Auditado prestamos
builder.Services.AddScoped<IAltaAuditoriaPrestamo, AltaAuditoriaPrestamoCU>();

// Observaciones
builder.Services.AddScoped<IEvaluarObservacion, EvaluarObservacionCU>();
builder.Services.AddScoped<IAltaObservacion, AltaObservacionCU>();

// Servicios externos
builder.Services.AddHttpClient<IServicioEvaluacionGemini, ServicioEvaluacionGemini>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(25);
});

// Objetos Celestes
builder.Services.AddScoped<IObtenerTodosOCEvaluacion, ObtenerTodosOCEvaluacionCU>();
builder.Services.AddScoped<IObtenerOCEvaluacionIDCU, ObtenerOCEvaluacionIdCU>();

// Agregar sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
