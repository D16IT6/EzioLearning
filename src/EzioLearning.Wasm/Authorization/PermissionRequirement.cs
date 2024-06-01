using Microsoft.AspNetCore.Authorization;

namespace EzioLearning.Wasm.Authorization;

public class PermissionRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; set; } = permission;
}