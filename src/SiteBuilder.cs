using NickName73.Site.Pages;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace NickName73.Site;

public class SiteBuilder
{
	private readonly DateTime beginTime;
	private readonly string dpages, dpreset, ddeploy;
	private readonly Dictionary<PageBuilder, string> pages;
	private readonly YamlDotNet.Serialization.IDeserializer deserializer;
	public readonly string SiteUrl;
	public SiteBuilder(string siteUrl, DateTime date, string pagesDirectory, string presetsDirectory, string? deployDirectory = null)
	{
		SiteUrl = siteUrl;
		beginTime = date;
		dpages = pagesDirectory;
		dpreset = presetsDirectory;
		ddeploy = deployDirectory ?? Environment.CurrentDirectory;

		pages = new(32);

		deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
			.Build();
	}
	public string Env(string envName)
		=> envName switch
		{
			"BUILD_TIME" => beginTime.ToString(),
			_ => envName.ToUpper(),
		};
	public string GetPagePreset(string name)
	{
		string path = Path.Combine(dpages, (name.EndsWith(".html") ? name : name + ".html"));
		if (!File.Exists(path))
		{
			Log.Fatal("File not found {0}", path);
			Environment.Exit(-1);
		}
		return File.ReadAllText(path);
	}
	public T GetPreset<T>(string name)
	{
		string path = Path.Combine(dpreset, (name.EndsWith(".yml") ? name : name + ".yml"));
		if (!File.Exists(path))
		{
			Log.Fatal("File not found {0}", path);
			Environment.Exit(-1);
		}
		string yml = File.ReadAllText(path);
		return deserializer.Deserialize<T>(yml);
	}
	public void Build()
	{
		using MainPageBuilder mainPage = new(this, "index");
		mainPage.Build(pages);
	}
	public void Deploy()
	{
		foreach (KeyValuePair<PageBuilder, string> pair in pages)
		{
			string dest = Path.Combine(ddeploy, pair.Key.FileName);
			File.Create(dest).Close();

			File.WriteAllText(dest, pair.Value);
		}
	}
}