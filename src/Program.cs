using Serilog;
using System;

namespace NickName73.Site;

public static class Program
{
	public static void Main(string[] args)
	{
		Log.Logger = new LoggerConfiguration()
			.WriteTo.Console()
			.WriteTo.File(".site/log")
#if DEBUG
			.MinimumLevel.Debug()
#else
			.MinimumLevel.Information()
#endif
			.CreateLogger();

		SiteBuilder builder = new("https://nickname73.github.io/", DateTime.Now, ".pages", ".presets", ".site");
		builder.Build();
		builder.Deploy();
	}
}