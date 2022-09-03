using System;

namespace NickName73.Site.DataTypes;

[Serializable]
public sealed class ProjectFile
{
	public string ProjectName { get; set; } = String.Empty;
	public bool Depricated { get; set; } = false;
	public LinkType[] Links { get; set; } = Array.Empty<LinkType>();
}
