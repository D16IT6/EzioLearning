using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace EzioLearning.Infrastructure.DbContext;

public class DataSeeder
{
    public async Task SeedAsync(EzioLearningDbContext context)
    {
        var roles = context.Roles;

        var adminGuid = Guid.Empty;
        if (!roles.Any()) adminGuid = await AddRole(context);

        var users = context.Users;
        if (!users.Any()) await AddUser(context, adminGuid);
    }

    private static async Task<Guid> AddRole(EzioLearningDbContext context)
    {
        var admin = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Admin",
            NormalizedName = "ADMIN",

            DisplayName = "Quản trị viên"
        };
        var teacher = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Teacher",
            NormalizedName = "TEACHER",

            DisplayName = "Giảng viên"
        };
        var student = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "Student",
            NormalizedName = "STUDENT",

            DisplayName = "Học viên"
        };
        var user = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "User",
            NormalizedName = "USER",

            DisplayName = "Người dùng"
        };
        await context.Roles.AddRangeAsync(admin, teacher, student, user);
        await context.SaveChangesAsync();
        return admin.Id;
    }

    private static async Task AddUser(EzioLearningDbContext context, Guid adminId)
    {
        var userId = Guid.NewGuid();
        var user = new AppUser
        {
            Id = userId,
            FirstName = "Talon",
            LastName = "Ezio",
            Email = "vuthemanh1707@gmail.com",
            NormalizedEmail = "VUTHEMANH1707@GMAIL.COM",
            NormalizedUserName = "TALONEZIO",
            UserName = "talonezio",
            SecurityStamp = Guid.NewGuid().ToString(),
            Avatar = "Uploads/Images/Users/default-user.png",
            LockoutEnabled = false,
            EmailConfirmed = true,
            PhoneNumber = "0988344814",
            PhoneNumberConfirmed = true
        };
        var passwordHasher = new PasswordHasher<AppUser>();
        user.PasswordHash = passwordHasher.HashPassword(user, "TalonEzio177!@#");

        await context.Users.AddAsync(user);

        if (!adminId.Equals(Guid.Empty))
            await context.UserRoles.AddAsync(new IdentityUserRole<Guid>
            {
                RoleId = adminId,
                UserId = userId
            });
        await context.SaveChangesAsync();
    }
}