using app.Server.Data.Entities.Identity;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace app.Server.Data
{
    public static class UserDbSeeder
    {
        public static async Task SeedData(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<UserEntity>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<RoleEntity>>();

            await db.Database.MigrateAsync();

            foreach (var roleName in new[] { "Admin", "User" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new RoleEntity(roleName));
            }

            if (!await db.Users.AnyAsync(u => u.Email == "admin@gmail.com"))
            {
                var admin = new UserEntity
                {
                    FirstName = "Юхим",
                    LastName = "Манько",
                    Email = "admin@gmail.com",
                    UserName = "admin@gmail.com",
                    LoginType = "Email",
                    Image = "https://via.placeholder.com/100"
                };
                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }

            if (!await db.Users.AnyAsync(u => u.Email.StartsWith("testuser")))
            {
                var faker = new Faker<UserEntity>()
                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                    .RuleFor(u => u.Email, f => $"testuser_{Guid.NewGuid()}@example.com")
                    .RuleFor(u => u.UserName, (f, u) => u.Email)
                    .RuleFor(u => u.LoginType, f => "Email")
                    .RuleFor(u => u.Image, f => f.Internet.Avatar());

                var users = faker.Generate(1000);

                foreach (var user in users)
                {
                    var createResult = await userManager.CreateAsync(user, "Test123!");
                    if (createResult.Succeeded)
                        await userManager.AddToRoleAsync(user, "User");
                }
            }
        }
    }
}
