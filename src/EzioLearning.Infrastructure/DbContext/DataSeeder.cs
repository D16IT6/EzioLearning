﻿using EzioLearning.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;
using EzioLearning.Core.SeedWorks.Constants;

namespace EzioLearning.Infrastructure.DbContext;

public class DataSeeder
{
    public async Task SeedAsync(EzioLearningDbContext context)
    {
        var roles = context.Roles;

        var adminGuid = Guid.Empty;
        if (!(await roles.AnyAsync())) adminGuid = await AddRole(context);

        await AddPermissions(context);

        var users = context.Users;
        if (!(await users.AnyAsync())) await AddUser(context, adminGuid);

    }

    private async Task AddPermissions(EzioLearningDbContext context)
    {
        var permissionRoleNames = GetPermissionNames(typeof(Permissions));

        var permissions = await context.Permissions.ToListAsync();

        foreach (var permission in permissionRoleNames.Select(permissionName => new AppPermission()
        {
            Name = permissionName,
            DisplayName = GetPermissionDescription(permissionName),
        }))
        {
            var first = permissions.FirstOrDefault(x => x.Name.Equals(permission.Name));
            if (first is null)
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();

    }
    private static List<string> GetPermissionNames(Type permissionsClass)
    {
        return (
            from typeInfo in permissionsClass.GetTypeInfo().DeclaredNestedTypes
            from field in typeInfo.GetTypeInfo().DeclaredFields
            select $"{permissionsClass.Name}.{typeInfo.Name}.{field.Name}").ToList();
    }
    private static string GetPermissionDescription(string permissionString)
    {
        var parts = permissionString.Split('.');
        if (parts.Length != 3)
        {
            throw new ArgumentException("Invalid permission string format");
        }
        var permissionName = parts[2];

        var permissionsType = typeof(Permissions);
        var nestedClassType = permissionsType.GetNestedType(parts[1]);
        var field = nestedClassType?.GetField(permissionName);
        if (field == null) return string.Empty;

        var descriptionAttribute =
            (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute))!;

        return descriptionAttribute.Description;
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
        var user = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = "User",
            NormalizedName = "USER",

            DisplayName = "Người dùng"
        };
        await context.Roles.AddRangeAsync(admin, teacher, user);

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