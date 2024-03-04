namespace EzioLearning.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureBuilder();


        var app = builder.Build();

        app.Configure();

        app.Run();
    }
}