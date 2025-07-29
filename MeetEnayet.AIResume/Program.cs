using MeetEnayet.AIResume.Kernels;
using MeetEnayet.AIResume.Kernels.Skills.PersonalitySkill;
using MeetEnayet.AIResume.Services;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<EmbeddingService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
var serviceProvider = app.Services;

// Create the kernel and register the PersonalitySkill plugin
var kernel = KernelBuilder.BuildKernel(builder);
kernel.Plugins.AddFromObject(
	new PersonalitySkill(serviceProvider.GetRequiredService<EmbeddingService>()),
	"PersonalitySkill"
);
builder.Services.AddSingleton(kernel);

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();