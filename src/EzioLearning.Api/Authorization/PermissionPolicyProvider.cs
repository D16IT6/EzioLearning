using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace EzioLearning.Api.Authorization;

public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; } = new(options);


    public override Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!policyName.StartsWith("Permission", StringComparison.OrdinalIgnoreCase))
            return FallbackPolicyProvider.GetPolicyAsync(policyName);

        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionRequirement(policyName));

        return Task.FromResult(policy.Build())!;
    }

    public override bool AllowsCachingPolicies => true;
}