using System.ComponentModel;

namespace EzioLearning.Share.Common
{
    public static class Permissions
    {
        public static class Dashboard
        {
            [Description("Xem dashboard")]
            public const string View = $"{nameof(Permissions)}.{nameof(Dashboard)}.{nameof(View)}";
        }
        public static class Roles
        {
            [Description("Xem quyền")]
            public const string View = $"{nameof(Permissions)}.{nameof(Roles)}.{nameof(View)}";
            [Description("Tạo mới quyền")]
            public const string Create = $"{nameof(Permissions)}.{nameof(Roles)}.{nameof(Create)}";
            [Description("Sửa quyền")]
            public const string Edit = $"{nameof(Permissions)}.{nameof(Roles)}.{nameof(Edit)}";
            [Description("Xóa quyền")]
            public const string Delete = $"{nameof(Permissions)}.{nameof(Roles)}.{nameof(Delete)}";
        }
        public static class Users
        {
            [Description("Xem người dùng")]
            public const string View = $"{nameof(Permissions)}.{nameof(Users)}.{nameof(View)}";
            [Description("Tạo mới người dùng")]
            public const string Create = $"{nameof(Permissions)}.{nameof(Users)}.{nameof(Create)}";
            [Description("Sửa người dùng")]
            public const string Edit = $"{nameof(Permissions)}.{nameof(Users)}.{nameof(Edit)}";
            [Description("Xóa người dùng")]
            public const string Delete = $"{nameof(Permissions)}.{nameof(Users)}.{nameof(Delete)}";
        }
        public static class Accounts
        {
            [Description("Bình luận")]
            public const string Comment = $"{nameof(Permissions)}.{nameof(Accounts)}.{nameof(Comment)}";
            [Description("Đổi email mới")]
            public const string ChangeEmail = $"{nameof(Permissions)}.{nameof(Accounts)}.{nameof(ChangeEmail)}";
        }
        public static class Courses
        {
            [Description("Xem Khoá học")]
            public const string View = $"{nameof(Permissions)}.{nameof(Courses)}.{nameof(View)}";
            [Description("Tạo mới Khoá học")]
            public const string Create = $"{nameof(Permissions)}.{nameof(Courses)}.{nameof(Create)}";
            [Description("Sửa Khoá học")]
            public const string Edit = $"{nameof(Permissions)}.{nameof(Courses)}.{nameof(Edit)}";
            [Description("Xóa Khoá học")]
            public const string Delete = $"{nameof(Permissions)}.{nameof(Courses)}.{nameof(Delete)}";
        }
    }
}
