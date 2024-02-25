﻿namespace EzioLearning.Domain.Common
{
    public abstract record RoleConstants
    {
        public static string Admin = $"{nameof(Admin)}";
        public static string Teacher = $"{nameof(Teacher)}";
        public static string Student = $"{nameof(Student)}";
        public static string User = $"{nameof(User)}";
    }
}
