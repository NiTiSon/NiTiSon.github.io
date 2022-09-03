using System;

namespace NickName73.Site.DataTypes;

[Serializable]
public sealed class ProjectType : IFileType
{
	public static string DefaultFileName => String.Empty;
	public ProjectType(string projectName, bool depricated, LinkType[] links)
	{
		ProjectName = projectName;
		Depricated = depricated;
		Links = links;
	}
	public ProjectType()
	{

	}
	[YamlDotNet.Serialization.YamlMember(Alias = "project-name")]
	public string ProjectName { get; set; } = String.Empty;
	[YamlDotNet.Serialization.YamlMember(Alias = "depricated")]
	public bool Depricated { get; set; } = false;
	[YamlDotNet.Serialization.YamlMember(Alias = "links")]
	public LinkType[] Links { get; set; } = Array.Empty<LinkType>();
}
