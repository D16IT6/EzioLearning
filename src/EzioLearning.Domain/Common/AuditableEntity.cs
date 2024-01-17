
namespace EzioLearning.Domain.Common;

internal class AuditableEntity<T>
{
    public DateTime CreatedDate { get; set; }
    public T? CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public T? ModifiedBy { get; set; }
}

