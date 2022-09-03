using System.Collections.Generic;

namespace NickName73.Site.Pages;

public sealed class MainPageBuilder : PageBuilder
{
	public MainPageBuilder(SiteBuilder builder, string page) : base(builder, page)
	{
	}
	public override string PageName => "Home";

	public override void Build(IDictionary<PageBuilder, string> pages)
	{
		string page = builder.GetPagePreset("main");

		page = page.Replace("{{PAGE_TITLE}}", this.PageName);

		page = page.Replace("{{NAV_ITEMS}}", builder.GetNavItems());

		pages.Add(this, page);
	}
}
