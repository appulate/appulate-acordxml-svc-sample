using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using static System.Net.Mime.MediaTypeNames;

namespace AcordXmlServiceSample;
public class XmlAsStringInputFormatter : InputFormatter {
	private readonly string[] supportedMediaTypes = new[] { Application.Xml, Text.Xml };
	public XmlAsStringInputFormatter() {
		foreach (var supportedMediaType in supportedMediaTypes) {
			SupportedMediaTypes.Add(new MediaTypeHeaderValue(supportedMediaType));
		}
	}

	public override bool CanRead(InputFormatterContext context) {
		if (context == null) {
			throw new ArgumentNullException(nameof(context));
		}

		var contentType = context.HttpContext.Request.ContentType;
		return !string.IsNullOrEmpty(contentType) && supportedMediaTypes.Any(t => t.StartsWith(contentType));
	}
	public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context) {
		var request = context.HttpContext.Request;
		using (var reader = new StreamReader(request.Body)) {
			var content = await reader.ReadToEndAsync();
			return await InputFormatterResult.SuccessAsync(content);
		}
	}
}