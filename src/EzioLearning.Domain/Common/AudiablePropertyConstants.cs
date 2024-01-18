
namespace EzioLearning.Domain.Common
{
    public static class AudiablePropertyConstants
    {
        public static string CreatedDate = nameof(IAuditableEntity<Guid>.CreatedDate);
        public static string ModifiedDate = nameof(IAuditableEntity<Guid>.ModifiedDate);
    }
}
