using Microsoft.EntityFrameworkCore;
using Radzen;
using StudentApp.Components;
using StudentApp.JSServices;
using StudentApp.Models;
using StudentApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var Connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentManagementContext>(options => options.UseSqlServer(Connection));
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddRadzenComponents();
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7083/") });
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<ProgramsService>();
builder.Services.AddScoped<StorageHelper>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRouting();
app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

app.Run();
