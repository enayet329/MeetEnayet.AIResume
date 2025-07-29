using MeetEnayet.AIResume.Models;
using MeetEnayet.AIResume.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace MeetEnayet.AIResume.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ChatController : ControllerBase
	{
		private readonly IChatCompletionService _chatService;
		private readonly EmbeddingService _embedding;

		public ChatController(Kernel kernel, EmbeddingService embedding)
		{
			_chatService = kernel.GetRequiredService<IChatCompletionService>();
			_embedding = embedding;
		}

		[HttpPost]
		public async Task<ChatResponse> Post([FromBody] ChatRequest request)
		{
			var memory = await _embedding.GetRelevantChunksAsync(request.UserMessage);
			var prompt = $"You are Enayet, a software engineer. Use the following context from your resume to respond with clarity, humility, and a technical tone:\n{memory}\n\nUser: {request.UserMessage}\nEnayetAI:";
			var response = await _chatService.GetChatMessageContentAsync(prompt);
			return new ChatResponse { Response = response.Content };
		}
	}
}