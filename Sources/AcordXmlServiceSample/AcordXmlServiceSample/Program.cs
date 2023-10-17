namespace AcordXmlServiceSample;
public class Program {
	public static void Main(string[] args) {
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.

		builder.Services.AddControllers();
		// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
		builder.Services.AddEndpointsApiExplorer();
		builder.Services.AddSwaggerGen();

		builder.Services.AddControllers(o => {
			o.InputFormatters.Add(new XmlAsStringInputFormatter());
		});
		builder.Services.AddScoped<FileService>();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		if (app.Environment.IsDevelopment()) {
			app.UseSwagger();
			app.UseSwaggerUI();
		}

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseAuthorization();

		app.MapControllers();

		app.Run();
	}
}