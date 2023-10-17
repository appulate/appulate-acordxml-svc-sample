using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace AcordXmlServiceSample.Controllers;
[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase {
	private readonly FileService _fileService;
	public FilesController(FileService fileService) {
		_fileService = fileService;
	}

	[HttpGet]
	[Route("{id}")]
	public async Task<ActionResult<string>> Get(Guid id) {
		var result = await _fileService.ReadAsync(id);
		return Content(result, Text.Plain);
	}
}