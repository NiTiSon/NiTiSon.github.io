using Serilog;
using System.Collections.Generic;

namespace NickName73.Site.DataTypes;

public class ProjectsFile : IFileType
{
	public static string DefaultFileName => "projects.yml";

	[YamlDotNet.Serialization.YamlMember(Alias = "projects")]
	public string[] Projects { get; set; }

	public IEnumerable<(ProjectType?, string)> Iterate(SiteBuilder builder)
	{
		foreach (string project in Projects)
		{
			ProjectType aproject = builder.GetPreset<ProjectType>(project, false);
			Log.Debug(">> {0}", aproject?.ProjectName ?? "null:" + project);

			yield return (aproject, project);
		}
	}
}
