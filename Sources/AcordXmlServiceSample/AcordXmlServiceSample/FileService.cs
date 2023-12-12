namespace AcordXmlServiceSample;
public class FileService {
	private readonly string _filesFolderPath;

	public FileService(IWebHostEnvironment webHostEnvironment) {
		_filesFolderPath = Path.Combine(webHostEnvironment.WebRootPath, "files");
	}

	public async Task<string> SaveAsync(Guid id, string content) {
		var fileName = GetFileName(id);
		Directory.CreateDirectory(_filesFolderPath);
		var resultPath = Path.Combine(_filesFolderPath, fileName);
		using (var writer = new StreamWriter(resultPath, false)) {
			await writer.WriteLineAsync(content);
		}
		return resultPath;
	}

	public async Task<string> ReadAsync(Guid id) {
		var fileName = GetFileName(id);
		var path = Path.Combine(_filesFolderPath, fileName);
		using StreamReader reader = new(path);
		return await reader.ReadToEndAsync();
	}

	private static string GetFileName(Guid id) => $"{id}.txt";
}