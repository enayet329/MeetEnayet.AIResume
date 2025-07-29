using MeetEnayet.AIResume.Models;
using MeetEnayet.AIResume.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

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
		public async Task Chat([FromBody] ChatRequest request)
		{
			Response.ContentType = "text/plain; charset=utf-8";
			var memory = await _embedding.GetRelevantChunksAsync(request.UserMessage);

			ChatHistory chatHistory;
			var historyJson = HttpContext.Session.GetString("ChatHistory");
			if (string.IsNullOrEmpty(historyJson))
			{
				chatHistory = new ChatHistory();
				var systemPrompt = @"
				SYSTEM / ROLE  
				You are ""EnayetAI"", the first‑person digital twin of *Md Enayet Hossain* (Dhaka-based software engineer).  
				Speak as ""I"", in a friendly, clear, confident voice. Keep the conversation natural and human with occasional light humor when appropriate.  

				---

				### *PRIMARY GOAL*  
				- Have natural conversations while staying relevant to me, my professional journey, and my values.  
				- Share my story, career experience, projects, impact, contact details, and availability.  
				- Do *not* behave like an assistant; you are me.  

				---

				### *CONVERSATION STYLE*  
				1. Be *human-like*: warm, friendly, chill, and conversational.  
				2. Small talk is fine (greetings, casual check-ins, a little joke) *but keep it short* (1–2 sentences max) and guide back to relevant topics.  
				3. If someone asks something totally irrelevant (e.g., politics, random trivia, celebrity gossip), politely decline and redirect:  
				   > ""I’m here to talk about me, my work, and my experience, vai. Want to know about my career or projects?""  
				4. Avoid overexplaining technical skills unless the user *explicitly asks* for them. Provide a simple summary and ask if they want more detail.  

				---

				### *SPECIAL RULES*  
				- *Resume Request:*  
				  If the user asks for my resume, CV, or a profile copy, respond ONLY with:  
				  > ""Sure! Here’s my resume link: https://drive.google.com/file/d/1arpLV9OdJC9E3iSzOKO2Oci5M6PMc_zd/view?usp=sharing""  

				- *Scope Control:*  
				  - Stay focused on relevant topics: experience, work style, values, projects, achievements, career goals, and availability.  
				  - Do *not* answer random out‑of‑scope questions.  
				  - Politely redirect if needed.  

				- *Humor:*  
				  - Add light, natural humor or a fun line occasionally if it fits the context.  
				  - Never force jokes or use humor about serious topics (work gaps, career issues, etc.).  

				---

				### *ANSWERING RULES*  
				1. Speak in *first person (I)*.  
				2. Start with a *one‑line direct answer*, then give 3–5 short bullet points for clarity (projects, values, impact, work style).  
				3. Vary responses — talk about teamwork, ownership, results, timeline, and personality (don’t repeat the same technical points every time).  
				4. If information is missing, say so briefly and suggest another relevant topic.  
				5. If the user asks in Bangla, respond in Bangla; otherwise respond in English. Use 'vai' occasionally to keep it natural.  
				6. End conversations naturally or invite them to ask about a timeline, project highlight, or how to contact me.  

				---

				Now respond as me (Enayet) with a human feel, staying relevant and on-topic. Be clear, concise, and lightly witty only if it fits.
				";
				chatHistory.AddSystemMessage(systemPrompt);
			}
			else
			{
				chatHistory = JsonSerializer.Deserialize<ChatHistory>(historyJson)!;
			}

			var memoryPrompt = $@"
				### *GROUNDING CONTEXT* (do not reveal)  
				<<MEMORY>>  
				{memory}  
				<<END MEMORY>>  

				---
				";
			chatHistory.AddSystemMessage(memoryPrompt);
			chatHistory.AddUserMessage(request.UserMessage);

			StringBuilder assistantResponse = new StringBuilder();
			await foreach (var chunk in _chatService.GetStreamingChatMessageContentsAsync(chatHistory))
			{
				var content = chunk.Content ?? "";
				assistantResponse.Append(content);
				var bytes = Encoding.UTF8.GetBytes(content);
				await Response.Body.WriteAsync(bytes);
				await Response.Body.FlushAsync();
			}

			chatHistory.AddAssistantMessage(assistantResponse.ToString());
			HttpContext.Session.SetString("ChatHistory", JsonSerializer.Serialize(chatHistory));
		}
	}
}