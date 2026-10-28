using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Eventos")));
builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EventosContext>();
    if (!await db.Categorias.AnyAsync())
    {
        var categorias = new[]
        {
            new Categoria { Nombre = "Tecnología" },
            new Categoria { Nombre = "Negocios" },
            new Categoria { Nombre = "Cultura" }
        };
        db.Categorias.AddRange(categorias);
        await db.SaveChangesAsync();
        db.Eventos.AddRange(
            new Evento { Titulo = "Conferencia de IA", Fecha = DateTime.Today.AddDays(20).AddHours(18), Lugar = "Auditorio Central", Aforo = 200, Precio = 50, CategoriaId = categorias[0].Id, Modalidad = "Presencial" },
            new Evento { Titulo = "Taller de ASP.NET Core", Fecha = DateTime.Today.AddDays(30).AddHours(9), Lugar = "Laboratorio 3", Aforo = 40, Precio = 80, CategoriaId = categorias[0].Id, Modalidad = "Presencial" },
            new Evento { Titulo = "Seminario de emprendimiento", Fecha = DateTime.Today.AddDays(40).AddHours(19), Lugar = "Microsoft Teams", Aforo = 500, Precio = 0, CategoriaId = categorias[1].Id, Modalidad = "Virtual" },
            new Evento { Titulo = "Festival de arte digital", Fecha = DateTime.Today.AddDays(50).AddHours(16), Lugar = "Centro Cultural", Aforo = 300, Precio = 25, CategoriaId = categorias[2].Id, Modalidad = "Presencial" });
        await db.SaveChangesAsync();
    }

    if (!await db.Asistentes.AnyAsync())
    {
        db.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres", Dni = "70123456", Correo = "ana@example.com", Telefono = "987654321" },
            new Asistente { Nombres = "Bruno Díaz", Dni = "71234567", Correo = "bruno@example.com", Telefono = "986543210" },
            new Asistente { Nombres = "Carla Ramos", Dni = "72345678", Correo = "carla@example.com" },
            new Asistente { Nombres = "Diego Flores", Dni = "73456789", Correo = "diego@example.com" },
            new Asistente { Nombres = "Elena Soto", Dni = "74567890", Correo = "elena@example.com" });
        await db.SaveChangesAsync();
    }
}

app.Run();
