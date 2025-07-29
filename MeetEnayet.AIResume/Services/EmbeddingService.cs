namespace MeetEnayet.AIResume.Services
{
	public class EmbeddingService
	{
		public Task<string> GetRelevantChunksAsync(string query)
		{
			return Task.FromResult("Enayet built B2C airline booking systems with .NET 8 and AWS.");
		}
	}
}
