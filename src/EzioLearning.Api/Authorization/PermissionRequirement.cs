using Microsoft.AspNetCore.Authorization;

namespace EzioLearning.Api.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; set; } = permission;
}