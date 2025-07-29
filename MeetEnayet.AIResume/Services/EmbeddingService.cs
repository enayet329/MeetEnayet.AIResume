using iTextSharp.text.pdf;
using System.Text;

namespace MeetEnayet.AIResume.Services
{
	public class EmbeddingService
	{
		private readonly string _pdfPath = "./Docs/enayet_resume.pdf"; // pdf file path

		public async Task<string> GetRelevantChunksAsync(string query)
		{
			if (!File.Exists(_pdfPath)) return "";

			var sb = new StringBuilder();
			using (var reader = new PdfReader(_pdfPath))
			{
				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					var text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, i);
					sb.AppendLine(text);
				}
			}

			// Mock semantic match for now
			return sb.ToString().Split("\n").Take(5).Aggregate((a, b) => a + "\n" + b);
		}
	}
}
