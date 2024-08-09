using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Radzen;
using StudentApp.Components;
using StudentApp.Hubs;
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
builder.Services.AddScoped<AttachmentService>();
builder.Services.AddScoped<ExcelExportService>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "StudentApp", Version = "v1" });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddSignalR();
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

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "StudentApp");
    c.RoutePrefix = "swagger";
    c.DisplayOperationId();
    c.DisplayRequestDuration(); ;
});

app.UseStaticFiles();
app.UseAntiforgery();
app.MapHub<StudentHub>("/studenthub");
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();
app.Run();
