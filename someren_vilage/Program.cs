using someren_vilage.Repositorie.ActivityRepo;
using someren_vilage.Repositorie.DrinkRepo;
using someren_vilage.Repositorie.LecturerRepo;
using someren_vilage.Repositorie.OrderRepo;
using someren_vilage.Repositorie.ParticipantRepo;
using someren_vilage.Repositorie.RoomRepo;
using someren_vilage.Repositorie.StudentRepo;
using someren_vilage.Repositorie.SupervisorRepo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRoomRepository, DbRoomRepository>();
builder.Services.AddScoped<IActivityRepository, DbActivityRepository>();
builder.Services.AddScoped<IStudentRepository, DbStudentRepository>();
builder.Services.AddScoped<ILecturerRepository, DbLecturerRepository>();
builder.Services.AddScoped<IDrinkRepository, DbDrinkRepository>();
builder.Services.AddScoped<IOrderRepository, DbOrderRepository>();
builder.Services.AddScoped<ISupervisorRepository, DBSupervisorRepository>();
builder.Services.AddScoped<IParticipantRepository, DbParticipantRepository>();



var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Redirect to Setup page if connection string is not configured
app.Use(async (context, next) =>
{
    string? connectionString = context.RequestServices.GetRequiredService<IConfiguration>()
        .GetConnectionString("SomerenDb");

    bool isSetupPath = context.Request.Path.StartsWithSegments("/Setup");
    bool isStaticFile = context.Request.Path.StartsWithSegments("/lib")
                     || context.Request.Path.StartsWithSegments("/css")
                     || context.Request.Path.StartsWithSegments("/js");

    if (string.IsNullOrWhiteSpace(connectionString) && !isSetupPath && !isStaticFile)
    {
        context.Response.Redirect("/Setup");
        return;
    }

    await next();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
