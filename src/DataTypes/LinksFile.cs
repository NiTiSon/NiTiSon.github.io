using System;

namespace NickName73.Site.DataTypes;

[Serializable]
public sealed class LinksFile : IFileType
{
	public static string DefaultFileName => "links.yml";
	public LinksFile(params LinkType[] links)
	{
		Links = links;
	}
	public LinksFile()
	{

	}
	[YamlDotNet.Serialization.YamlMember(Alias = "links")]
	public LinkType[] Links { get; set; }
}
