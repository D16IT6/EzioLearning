namespace EzioLearning.Domain.Common;

public abstract class AuditableEntity : AuditableTimeOnlyEntity
{
    public virtual Guid? CreatedBy { get; set; }
    public virtual Guid? ModifiedBy { get; set; }
}