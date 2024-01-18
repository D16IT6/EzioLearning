namespace EzioLearning.Domain.Common
{
    public static class ConnectionConstants
    {
        public static string ConnectionStringName { get; set; } = Common.ConnectionStringName.Development;
    }

    public record ConnectionStringName
    {
        public static string Development = "Development";
        public static string Production = "Production";
    }
}
