﻿namespace EzioLearning.Domain.Common;

public class AuditableTimeOnlyEntity
{
    public virtual DateTime CreatedDate { get; set; }
    public virtual DateTime? ModifiedDate { get; set; }
}