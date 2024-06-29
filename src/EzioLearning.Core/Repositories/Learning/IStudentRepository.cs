﻿using EzioLearning.Core.SeedWorks;
using EzioLearning.Domain.Entities.Learning;

namespace EzioLearning.Core.Repositories.Learning
{
    public interface IStudentRepository : IRepository<Student,Guid>
    {
    }
}
