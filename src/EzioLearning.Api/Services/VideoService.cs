using Xabe.FFmpeg;

namespace EzioLearning.Api.Services
{
	public class VideoService
	{
		public static string BasePath = Path.Combine(Environment.CurrentDirectory, "wwwroot");
		public async Task<long> GetDurationFromVideo(string filePath)
		{
			filePath = Path.Combine(BasePath, filePath);
			if (!File.Exists(filePath)) return 0;

			var info = await FFmpeg.GetMediaInfo(filePath);
			return (long)info.VideoStreams.First().Duration.TotalSeconds;
		}
	}
}
