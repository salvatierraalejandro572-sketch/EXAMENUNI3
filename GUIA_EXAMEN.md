# Guía de Defensa — Examen Unidad 3
### Mini-Jira · ASP.NET Core MVC · EF Core · Patrón Repositorio

---

# PARTE 1: Guion de Presentación

> Lo que está en **"DI:"** es lo que dices en voz alta.
> Lo que está en **"MUESTRA:"** es lo que tienes en pantalla mientras lo dices.

---

## Paso 1 — Abrir la aplicación y presentar el proyecto

**MUESTRA:** La página de inicio (`/Home/Index`) en el navegador.

**DI:**
> "Mi proyecto es un sistema de gestión de proyectos y tareas inspirado en Jira. Está desarrollado con ASP.NET Core MVC, Entity Framework Core en modo Code-First, SQL Server y el Patrón Repositorio. La aplicación permite a cada usuario registrado crear sus propios proyectos y asignarles tareas, con un aislamiento total de datos entre cuentas."

---

## Paso 2 — Mostrar que sin sesión no se puede crear

**MUESTRA:** Ir a `/Proyectos` → hacer clic en "Crear nuevo proyecto".

**DI:**
> "Lo primero que se nota es que la aplicación protege las acciones sensibles. Si intento crear un proyecto sin haber iniciado sesión, ASP.NET Identity me redirige automáticamente a la página de Login. Esto lo controlo con el atributo `[Authorize]` sobre las acciones del controlador."

---

## Paso 3 — Registrar o iniciar sesión

**MUESTRA:** Ir a `/Identity/Account/Register`, crear una cuenta y mostrar que redirige al home.

**DI:**
> "El sistema de autenticación es manejado completamente por ASP.NET Core Identity, integrado con Entity Framework. Cuando un usuario se registra, Identity guarda su información en la tabla `AspNetUsers` de SQL Server, con la contraseña hasheada automáticamente. No manejo contraseñas en texto plano en ningún punto de la aplicación."

---

## Paso 4 — Crear un Proyecto

**MUESTRA:** Ir a `/Proyectos/Create` y primero enviar el formulario vacío para demostrar validación.

**DI:**
> "El formulario tiene validaciones del lado del servidor definidas con Data Annotations en el modelo. El campo Nombre es obligatorio y tiene un máximo de 100 caracteres. Si lo dejo vacío y envío, el `ModelState` detecta el error y me lo muestra sin tocar la base de datos."

**MUESTRA:** Llenar el formulario correctamente y enviarlo.

**DI:**
> "Al enviar datos válidos, el controlador asigna automáticamente el `UserId` del usuario autenticado al proyecto y llama al repositorio para guardarlo. El proyecto queda vinculado a mi cuenta en SQL Server."

---

## Paso 5 — Crear una Tarea asociada al Proyecto

**MUESTRA:** Ir a `/Tareas/Create`, mostrar el dropdown de proyectos.

**DI:**
> "Al crear una tarea, el dropdown solo muestra mis proyectos, no los de otros usuarios. El repositorio filtra por el `UserId` del usuario activo. La tarea queda vinculada al proyecto a través de la clave foránea `ProyectoId`."

**MUESTRA:** Crear la tarea y mostrar que aparece en `/Tareas`.

---

## Paso 6 — Mostrar el aislamiento de datos entre cuentas

**MUESTRA:** Cerrar sesión → registrar una segunda cuenta → ir a `/Proyectos`.

**DI:**
> "Esta es la funcionalidad clave: el aislamiento de datos. Con esta segunda cuenta la lista aparece vacía aunque la primera cuenta tiene proyectos creados. El método `GetAllByUserAsync` en el repositorio filtra con `WHERE UserId = @userId`, garantizando que ningún usuario vea datos de otro."

---

## Paso 7 — Explicar el recorrido completo del dato

**MUESTRA:** Abrir `ProyectosController.cs` en el IDE.

**DI:**
> "El recorrido completo de un dato al crear un proyecto es:
>
> 1. El navegador envía un POST HTTP a `/Proyectos/Create`.
> 2. El método `Create` del controlador recibe el modelo mapeado por el Model Binder de ASP.NET.
> 3. Se valida con `ModelState.IsValid`. Si hay errores, regresa la vista. Si está bien, continúa.
> 4. Se asigna el `UserId` del usuario autenticado al proyecto.
> 5. El controlador llama a `_proyectoRepository.AddAsync(proyecto)`.
> 6. El repositorio hace `_context.Proyectos.AddAsync(proyecto)` y luego `SaveChangesAsync()`.
> 7. Entity Framework genera el `INSERT INTO Proyectos` y lo ejecuta contra SQL Server.
> 8. El controlador redirige al Index con `RedirectToAction(nameof(Index))`."

---

# PARTE 2: Guía "Salvavidas" — Explicación Línea por Línea

> Si la licenciada señala el código y pregunta "¿Qué hace esto exactamente?", usa esta sección.

---

## Program.cs — Inyección de Dependencias

### Registro del DbContext
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
```
**Qué hace:** Registra el `ApplicationDbContext` en el contenedor de dependencias. Le dice que use SQL Server con la cadena de conexión del `appsettings.json`. Esto permite que cualquier clase (controladores, repositorios) reciba el contexto automáticamente sin instanciarlo manualmente — eso es la inyección de dependencias.

---

### Registro de Identity
```csharp
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>();
```
**Qué hace:** Registra el sistema de autenticación. `IdentityUser` es el modelo de usuario por defecto. `RequireConfirmedAccount = false` permite login directo sin verificar email. `AddEntityFrameworkStores` le dice a Identity que guarde sus datos (usuarios, roles, tokens) en nuestro propio `ApplicationDbContext`, es decir, en SQL Server, en tablas que empiezan con `AspNet`.

---

### Registro del Patrón Repositorio
```csharp
builder.Services.AddScoped<IProyectoRepository, ProyectoRepository>();
builder.Services.AddScoped<ITareaRepository, TareaRepository>();
```
**Qué hace:** Registra el Patrón Repositorio. `AddScoped` crea una instancia nueva por cada request HTTP. Cuando el controlador declara `IProyectoRepository` en su constructor, ASP.NET inyecta automáticamente un `ProyectoRepository`. El controlador solo conoce la interfaz, nunca la implementación concreta — eso es el principio de inversión de dependencias (la D de SOLID).

---

### Middleware de Seguridad
```csharp
app.UseAuthentication();
app.UseAuthorization();
```
**Qué hace:** Activan el middleware de seguridad. El orden importa: primero `UseAuthentication` (¿quién eres?) y luego `UseAuthorization` (¿qué puedes hacer?). Si se invierten, el atributo `[Authorize]` no funcionaría correctamente.

---

## ApplicationDbContext.cs — El Contexto de Base de Datos

### Herencia de IdentityDbContext
```csharp
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
```
**Qué hace:** `ApplicationDbContext` hereda de `IdentityDbContext`, lo que agrega automáticamente todas las tablas de Identity a nuestra base de datos. El parámetro `DbContextOptions` lo inyecta ASP.NET con la configuración registrada en `Program.cs` (cadena de conexión, proveedor SQL Server).

---

### DbSets (las tablas)
```csharp
public DbSet<Proyecto> Proyectos { get; set; }
public DbSet<Tarea> Tareas { get; set; }
```
**Qué hace:** Un `DbSet<T>` representa una tabla en SQL Server. EF usa estas propiedades para crear y gestionar las tablas `Proyectos` y `Tareas`. Son la puerta de entrada para todas las consultas LINQ que luego se traducen a SQL automáticamente.

---

### Relación Proyecto → Usuario (FK con Cascade Delete)
```csharp
entity.HasOne<IdentityUser>()
      .WithMany()
      .HasForeignKey(p => p.UserId)
      .OnDelete(DeleteBehavior.Cascade);
```
**Qué hace:** Define que un `IdentityUser` puede tener muchos proyectos (`WithMany()`), pero cada proyecto pertenece a un solo usuario (`HasOne`). `HasForeignKey` indica que `UserId` en la tabla `Proyectos` referencia `AspNetUsers.Id`. `OnDelete(Cascade)` significa que si se elimina el usuario, sus proyectos se eliminan automáticamente.

---

### Relación Tarea → Proyecto (FK con Cascade Delete)
```csharp
entity.HasOne(t => t.Proyecto)
      .WithMany(p => p.Tareas)
      .HasForeignKey(t => t.ProyectoId)
      .OnDelete(DeleteBehavior.Cascade);
```
**Qué hace:** Define la relación 1:N entre Proyecto y Tarea. Al borrar un proyecto, todas sus tareas se eliminan. Encadenado con el anterior: **Usuario eliminado → Proyectos eliminados → Tareas eliminadas**.

---

## Repositorios — Async / Await

### Método asíncrono de consulta filtrada
```csharp
public async Task<List<Proyecto>> GetAllByUserAsync(string userId)
{
    return await _context.Proyectos
        .Where(p => p.UserId == userId)
        .Include(p => p.Tareas)
        .OrderBy(p => p.FechaInicio)
        .ToListAsync();
}
```
**Qué hace línea por línea:**
- `async Task<List<Proyecto>>` → el método es asíncrono y devolverá una lista de proyectos cuando termine.
- `await` → suspende el método hasta que la BD responda, **sin bloquear el hilo del servidor**. El servidor puede atender otras peticiones mientras espera.
- `.Where(p => p.UserId == userId)` → genera `WHERE UserId = @userId` en SQL. Aquí está el aislamiento de datos.
- `.Include(p => p.Tareas)` → hace un JOIN con Tareas para traer los datos relacionados en una sola consulta (Eager Loading).
- `.ToListAsync()` → ejecuta la consulta y devuelve los resultados como lista.

---

### Método asíncrono de inserción
```csharp
public async Task AddAsync(Proyecto proyecto)
{
    await _context.Proyectos.AddAsync(proyecto);
    await _context.SaveChangesAsync();
}
```
**Qué hace:** `AddAsync` registra el objeto en el contexto de EF con estado `Added`. En este punto **aún no hay nada en SQL Server**. `SaveChangesAsync()` es el momento en que EF genera y ejecuta el `INSERT INTO Proyectos (...)`. Si `SaveChanges` no se llama, el dato nunca llega a la base de datos.

---

## Controllers — [Authorize] y Filtrado por Usuario

### Atributo [Authorize]
```csharp
[Authorize]
public IActionResult Create()
{
    return View();
}
```
**Qué hace:** El atributo `[Authorize]` es procesado por el middleware de autorización antes de que el método se ejecute. Si no hay sesión activa, ASP.NET redirige automáticamente a `/Identity/Account/Login`. Sin este atributo, cualquiera podría acceder a la vista de creación aunque no esté registrado.

---

### Asignación del UserId al crear
```csharp
proyecto.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
```
**Qué hace:** `User` representa al usuario autenticado en el request actual. `FindFirstValue(ClaimTypes.NameIdentifier)` extrae el ID único del usuario desde su token de sesión (un "claim"). Ese ID es el mismo que está en `AspNetUsers.Id`. Al asignarlo al proyecto, garantizamos que quede vinculado al dueño correcto.

---

### Verificación de propiedad antes de editar/eliminar
```csharp
if (proyecto == null || proyecto.UserId != userId) return NotFound();
```
**Qué hace:** Antes de mostrar o modificar un proyecto, comparo el `UserId` del proyecto con el del usuario autenticado. Si no coinciden, devuelvo un 404. Esto evita que un usuario pueda editar o eliminar el proyecto de otra persona aunque conozca su ID en la URL — es una validación de autorización a nivel de recurso.

---

## Modelos — Validaciones con Data Annotations

### Required y MaxLength
```csharp
[Required(ErrorMessage = "El nombre es obligatorio")]
[MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
public string Nombre { get; set; } = string.Empty;
```
**Qué hace:** Los atributos definen reglas de validación. `[Required]` impide que el campo esté vacío. `[MaxLength(100)]` limita la longitud. Cuando el formulario se envía, ASP.NET ejecuta estas validaciones en el servidor y el resultado queda en `ModelState.IsValid`. Si es `false`, el controlador regresa la vista con mensajes de error sin tocar la base de datos.

---

### ForeignKey y propiedad de navegación
```csharp
[ForeignKey(nameof(ProyectoId))]
public Proyecto? Proyecto { get; set; }
```
**Qué hace:** Le indica a EF que la propiedad de navegación `Proyecto` está relacionada con la columna `ProyectoId`. `nameof(ProyectoId)` devuelve el nombre como string en tiempo de compilación, evitando errores de tipeo. El `?` indica que puede ser null si no se cargó con `.Include()`.

---

## Diagrama del Flujo Completo del Dato

```
[Formulario HTML]
      |
      | POST /Proyectos/Create
      v
[ProyectosController.Create(Proyecto proyecto)]
      |
      | ¿ModelState.IsValid? → NO → regresa vista con errores
      |          SÍ
      | proyecto.UserId = User.FindFirstValue(...)
      v
[IProyectoRepository.AddAsync(proyecto)]
      |
      v
[ProyectoRepository]
      | _context.Proyectos.AddAsync(proyecto)  ← registra en memoria
      | _context.SaveChangesAsync()            ← genera y ejecuta SQL
      v
[SQL Server]
      | INSERT INTO Proyectos (Nombre, UserId, FechaInicio, ...)
      | VALUES (@Nombre, @UserId, @FechaInicio, ...)
      v
[RedirectToAction("Index")] → el usuario ve su proyecto en la lista
```

---

*Guía generada para la defensa del Examen Unidad 3 — DES320 Taller de Diseño de Aplicaciones*
