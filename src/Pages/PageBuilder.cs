using Serilog;
using System;
using System.Collections.Generic;

namespace NickName73.Site.Pages;

public abstract class PageBuilder : IDisposable
{
	protected readonly SiteBuilder builder;
	protected readonly string page;
	public string FileName => page + ".html";
	public string PageUri => page is "index" ? builder.SiteUrl.ToString() : builder.SiteUrl + page;
	public PageBuilder(SiteBuilder builder, string page)
	{
		this.builder = builder;
		this.page = page;

		Log.Debug("Page: {0}\nUri: {1}\nFile name: {2}", page, PageUri, FileName);
	}
	public abstract void Build(IDictionary<PageBuilder, string> pages);
	public virtual void Dispose() { }
}
