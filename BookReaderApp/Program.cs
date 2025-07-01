using BookReaderApp.Data;
using BookReaderApp.Services;
using BookReaderApp.Components;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found. Please set it using User Secrets: dotnet user-secrets set \"ConnectionStrings:DefaultConnection\" \"your-connection-string\"");

builder.Services.AddDbContext<BookReaderContext>(options =>
    options.UseNpgsql(connectionString));

// Register services
builder.Services.AddScoped<IFileProcessingService, FileProcessingService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IBookmarkService, BookmarkService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map Razor components for .NET 8
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BookReaderContext>();
    await context.Database.EnsureCreatedAsync();
}

app.Run();