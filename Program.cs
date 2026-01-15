using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Adaugam suportul pentru conexiunea la baza de date SQL Server
builder.Services.AddDbContext<Cailean_Razvan_Zboruri.Data.AviationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AviationContext") ?? throw new InvalidOperationException("Connection string 'AviationContext' not found.")));

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
