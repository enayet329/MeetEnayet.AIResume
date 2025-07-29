using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using OpenAI;
using System.ClientModel;

namespace MeetEnayet.AIResume.Kernels
{
	public static class KernelBuilder
	{
		public static Kernel BuildKernel(WebApplicationBuilder builder)
		{
			var kernelBuilder = Kernel.CreateBuilder();

			string apiKey = builder.Configuration["OpenAI:ApiKey"];
			string modelId = builder.Configuration["OpenAI:Model"];
			string baseUrl = builder.Configuration["OpenAI:BaseUrl"];

			if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(modelId))
				throw new InvalidOperationException("OpenAI:ApiKey or OpenAI:Model is missing in configuration.");

			var openAIClient = new OpenAIClient(
				new ApiKeyCredential(apiKey),
				new OpenAIClientOptions
				{
					Endpoint = new Uri(baseUrl)
				});

			kernelBuilder.Services.AddSingleton<IChatCompletionService>(new OpenAIChatCompletionService(
				modelId,
				openAIClient
			));

			return kernelBuilder.Build();
		}
	}
}
