using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Geekspace.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

// Numeric constraint keeps Resource actions on the default route.
app.MapControllerRoute(
    name: "resourceDetails",
    pattern: "Resource/{id:int}",
    defaults: new { controller = "Resource", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

   using (var scope = app.Services.CreateScope())
   {
       await SeedRolesAsync(scope.ServiceProvider);

       var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
       await dbContext.Database.MigrateAsync();
       await SeedData.InitializeAsync(dbContext);
   }
   app.Run();

   // Idempotently creates and assigns privileged roles.
   static async Task SeedRolesAsync(IServiceProvider services)
   {
       var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
       var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

       foreach (var role in new[] { "Root", "Admin" })
       {
           if (!await roleManager.RoleExistsAsync(role))
           {
               await roleManager.CreateAsync(new IdentityRole(role));
           }
       }

       await PromoteIfExistsAsync(userManager, "root@fosvcat.com", "Root");
       await PromoteIfExistsAsync(userManager, "admin@fosvcat.com", "Admin");
   }

   static async Task PromoteIfExistsAsync(UserManager<IdentityUser> userManager, string email, string role)
   {
       var user = await userManager.FindByEmailAsync(email);
       if (user != null && !await userManager.IsInRoleAsync(user, role))
       {
           await userManager.AddToRoleAsync(user, role);
       }
   }
