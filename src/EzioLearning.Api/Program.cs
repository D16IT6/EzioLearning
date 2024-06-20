namespace EzioLearning.Api;

public class Program
{
    public static void Main(string[] args)
    {
        Xabe.FFmpeg.FFmpeg.SetExecutablesPath(Path.Combine(Environment.CurrentDirectory,"Tools"));
        var builder = WebApplication.CreateBuilder(args);

        builder.ConfigureBuilder();
       
        var app = builder.Build();

        app.Configure();

        app.Run();
    }
}