using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity; // NECESAR PENTRU IDENTITY
using Cailean_Razvan_Zboruri.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Conexiunea la baza de date
builder.Services.AddDbContext<Cailean_Razvan_Zboruri.Data.AviationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AviationContext") ?? throw new InvalidOperationException("Connection string 'AviationContext' not found.")));

// 2. AICI ADĂUGĂM IDENTITY (Conturi + Roluri)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>() // Activam Rolurile (Admin/User)
    .AddEntityFrameworkStores<Cailean_Razvan_Zboruri.Data.AviationContext>();

// 3. Adaugam paginile Razor
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. Autentificarea trebuie sa fie inainte de Autorizare!
app.UseAuthentication(); // <-- ADAUGA ASTA PENTRU A STI CINE E LOGAT
app.UseAuthorization();  // <-- VERIFICA DACA ARE VOIE SA VADA PAGINA

app.MapRazorPages();

// ------------------------------------------------------------------
// 5. BLOCUL CARE CREEAZA AUTOMAT ROLURILE SI ADMINUL LA PORNIRE
// ------------------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    // Cream rolurile daca nu exista
    string[] roleNames = { "Admin", "User" };
    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Cream contul de Admin default
    var adminEmail = "admin@angelairlines.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
        var createPowerUser = await userManager.CreateAsync(adminUser, "Admin123!"); // Parola trebuie sa fie puternica
        if (createPowerUser.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
// ------------------------------------------------------------------
// Inițializare Stripe folosind cheia secretă din appsettings.json
Stripe.StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe")["SecretKey"];

app.Run();