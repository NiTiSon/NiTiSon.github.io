using System.Collections.Generic;

namespace NickName73.Site.Pages;

public sealed class MainPageBuilder : PageBuilder
{
	public MainPageBuilder(SiteBuilder builder, string page) : base(builder, page)
	{
	}
	public override void Build(IDictionary<PageBuilder, string> pages)
	{
		string page = builder.GetPagePreset("main");


		pages.Add(this, page);
	}
}
