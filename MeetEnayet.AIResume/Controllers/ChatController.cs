using MeetEnayet.AIResume.Models;
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
		private readonly Kernel _kernel;

		public ChatController(Kernel kernel)
		{
			_kernel = kernel;
		}

		[HttpPost]
		public async Task<ChatResponse> Post([FromBody] ChatRequest request)
		{
			var prompt = await _kernel.InvokeAsync<string>("PersonalitySkill", "InjectPersonalityAsync", new() { ["input"] = request.UserMessage });
			var chatService = _kernel.GetRequiredService<IChatCompletionService>();
			var response = await chatService.GetChatMessageContentAsync(prompt);
			return new ChatResponse { Response = response.Content };
		}
	}
}