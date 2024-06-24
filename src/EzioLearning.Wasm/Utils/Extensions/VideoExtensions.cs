namespace EzioLearning.Wasm.Utils.Extensions
{
	public static class VideoExtensions
	{
		public static string ShowDurationFromSecond(this long totalSeconds)
		{

			if (totalSeconds < 0)
			{
				return "Invalid duration";
			}

			var hours = totalSeconds / 3600;
			var minutes = (totalSeconds % 3600) / 60;
			var secs = totalSeconds % 60;

			return $"{(hours > 0 ? hours : 0)}h {(minutes > 0 ? minutes : 0)}m {(secs > 0 ? secs : 0)}s";
		}
	}

}
