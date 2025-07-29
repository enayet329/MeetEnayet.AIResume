using MeetEnayet.AIResume.Services;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace MeetEnayet.AIResume.Kernels.Skills.PersonalitySkill
{
	public class PersonalitySkill
	{
		private readonly EmbeddingService _embedding;

		public PersonalitySkill(EmbeddingService embedding)
		{
			_embedding = embedding;
		}

		[KernelFunction]
		[Description("Injects personalized context from PDF")]
		public async Task<string> InjectPersonalityAsync(string input)
		{
			var memory = await _embedding.GetRelevantChunksAsync(input);
			return $"Memory:\n{memory}\nUser: {input}\nEnayetAI:";
		}
	}
}