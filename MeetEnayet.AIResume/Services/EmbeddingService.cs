using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.Text;

namespace MeetEnayet.AIResume.Services
{
	public class EmbeddingService
	{
		private readonly string _pdfPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Docs", "Resume_Md_Enayet_Hossain.pdf");
		public async Task<string> GetRelevantChunksAsync(string query)
		{
			if (!File.Exists(_pdfPath)) return "";
			var sb = new StringBuilder();
			using (var reader = new PdfReader(_pdfPath))
			{
				for (int i = 1; i <= reader.NumberOfPages; i++)
				{
					var text = PdfTextExtractor.GetTextFromPage(reader, i);
					sb.AppendLine(text);
				}
			}
			return sb.ToString().Split("\n").Aggregate((a, b) => a + "\n" + b);
		}
	}
}