using NickName73.Site.DataTypes;
using NickName73.Site.Pages;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NickName73.Site;

public class SiteBuilder
{
	private readonly DateTime beginTime;
	private readonly string dpages, dpreset, ddeploy;
	private readonly Dictionary<PageBuilder, string> pages;
	private readonly YamlDotNet.Serialization.Deserializer deserializer;
	public readonly string SiteUrl;
	public SiteBuilder(string siteUrl, DateTime date, string pagesDirectory, string presetsDirectory, string? deployDirectory = null)
	{
		SiteUrl = siteUrl;
		beginTime = date;
		dpages = pagesDirectory;
		dpreset = presetsDirectory;
		ddeploy = deployDirectory ?? Environment.CurrentDirectory;

		pages = new(32);

		deserializer = new();
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
	public T GetPreset<T>() where T : IFileType
	{
		if (String.IsNullOrWhiteSpace(T.DefaultFileName))
		{
			Log.Fatal("Type {0} default type is null", typeof(T).Name);
			Environment.Exit(-1);
		}
		return GetPreset<T>(T.DefaultFileName);
	}
	public T GetPreset<T>(string name, bool throwIfNotFound = true) where T : IFileType
	{
		string path = Path.Combine(dpreset, (name.EndsWith(".yml") ? name : name + ".yml"));
		if (!File.Exists(path))
		{
			if (throwIfNotFound)
			{
				Log.Fatal("File not found {0}", path);
				Environment.Exit(-1);
			} else
			{
				Log.Warning("File not found {0}", path);
				return default!;
			}
		}
		string yml = File.ReadAllText(path);
		try
		{
			return deserializer.Deserialize<T>(yml);
		}
		catch(Exception ex)
		{
			Log.Fatal(ex, "Fatal error during deserialization");
			Environment.Exit(-2);
			return default;
		}
	}
	public string GetNavItems()
	{
		StringBuilder sb = new();

		sb.Append("\n<!-- NavBar -->\n");
		GetNavLinks(sb);
		GetNavProjects(sb);
		sb.Append("\n<!-- NavBarEnd -->\n");

		return sb.ToString();
	}
	public void GetNavLinks(StringBuilder sb)
	{
		string dropdownPreset = GetPagePreset("dropdown");
		dropdownPreset = dropdownPreset.Replace("{{DROPDOWN_NAME}}", "Links");

		LinksFile links = GetPreset<LinksFile>();
		string linkPreset = GetPagePreset("dropdown-link");

		int index = dropdownPreset.IndexOf("{{DROPDOWN_CONTENT}}");
		if (index is -1)
		{
			Log.Fatal("Invalid page preset {0}", "dropdown.yml");
			Environment.Exit(-1);
		}
		sb.Append(dropdownPreset.Substring(0, index));

		foreach (LinkType linkType in links.Links)
		{
			sb.Append(linkPreset.Replace("{{DROPDOWN_CONTENT_NAME}}", linkType.Name).Replace("{{DROPDOWN_CONTENT_LINK}}", linkType.Url));
			Log.Debug("Added link {0} to page", linkType.Name);
		}
		sb.Append(dropdownPreset.Substring(index + "{{DROPDOWN_CONTENT}}".Length));
	}
	public void GetNavProjects(StringBuilder sb)
	{
		string dropdownPreset = GetPagePreset("dropdown");
		dropdownPreset = dropdownPreset.Replace("{{DROPDOWN_NAME}}", "Projects");

		int index = dropdownPreset.IndexOf("{{DROPDOWN_CONTENT}}");
		if (index is -1)
		{
			Log.Fatal("Invalid page preset {0}", "dropdown.yml");
			Environment.Exit(-1);
		}
		sb.Append(dropdownPreset.Substring(0, index));

		ProjectsFile projects = GetPreset<ProjectsFile>();
		string linkPreset = GetPagePreset("dropdown-link");
		string titlePreset = GetPagePreset("dropdown-title");

		foreach ((ProjectType? proj, string str) pair in projects.Iterate(this))
		{
			bool isTitle = pair.proj is null;
			string title = isTitle ? pair.str.Substring(1) : pair.proj.ProjectName;

			if (isTitle)
			{
				if (pair.str[0] is not '!')
					continue;
				sb.Append(titlePreset.Replace("{{DROPDOWN_CONTENT_NAME}}", title));
				Log.Debug("Added title {0} to page", pair.str);
				continue;
			}
			else
			{
				sb.Append(linkPreset.Replace("{{DROPDOWN_CONTENT_NAME}}", title).Replace("{{DROPDOWN_CONTENT_LINK}}", "/project/" + pair.str));
				Log.Debug("Added link {0} to page", title);
			}
		}
		sb.Append(dropdownPreset.Substring(index + "{{DROPDOWN_CONTENT}}".Length));
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