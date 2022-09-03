using System;

namespace NickName73.Site.DataTypes;

[Serializable]
public sealed class LinksFile
{
	public string[] Links { get; set; } = Array.Empty<String>();
}
