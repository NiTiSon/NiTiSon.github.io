using System;

namespace NickName73.Site.DataTypes;

[Serializable]
public sealed class LinkType
{
	public LinkType(string type, string url, string name)
	{
		Type = type;
		Url = url;
		Name = name;
	}
	public LinkType()
	{

	}
	[YamlDotNet.Serialization.YamlMember(Alias = "type")]
	public string Type { get; set; }
	[YamlDotNet.Serialization.YamlMember(Alias = "name")]
	public string Name { get; set; }
	[YamlDotNet.Serialization.YamlMember(Alias = "url")]
	public string Url { get; set; }
}
