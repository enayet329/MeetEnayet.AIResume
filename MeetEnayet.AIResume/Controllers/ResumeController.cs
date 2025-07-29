using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace MeetEnayet.AIResume.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ResumeController : ControllerBase
	{
		private readonly string _pdfPath = Path.Combine(AppContext.BaseDirectory, "Docs", "Resume_Md_Enayet_Hossain.pdf");

		[HttpPost("upload")]
		public async Task<IActionResult> UploadResume(IFormFile file, [FromQuery] string key)
		{
			if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

			if (key != "EnayetAI2025") return Unauthorized("Invalid key.");

			try
			{
				Directory.CreateDirectory(Path.GetDirectoryName(_pdfPath));

				if (System.IO.File.Exists(_pdfPath))
				{
					System.IO.File.SetAttributes(_pdfPath, FileAttributes.Normal);
					await RetryDeleteAsync(_pdfPath, 5, TimeSpan.FromMilliseconds(500));
				}

				using (var stream = new FileStream(_pdfPath, FileMode.Create))
				{
					await file.CopyToAsync(stream);
				}

				return Ok("File uploaded successfully.");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Error: {ex.Message}");
			}
		}

		private async Task RetryDeleteAsync(string path, int maxRetries, TimeSpan delay)
		{
			for (int i = 0; i < maxRetries; i++)
			{
				try
				{
					System.IO.File.Delete(path);
					return;
				}
				catch (IOException) when (i < maxRetries - 1)
				{
					await Task.Delay(delay);
				}
			}
			throw new IOException($"Failed to delete file after {maxRetries} attempts.");
		}
	}
}