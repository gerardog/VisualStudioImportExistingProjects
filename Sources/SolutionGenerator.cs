using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionGenerator
{
    class FolderContent
    {
        static SequentialGuid FolderSequentialGuid = new SequentialGuid(Guid.Empty);

        public List<string> Projects = new List<string>();
        public List<FolderContent> SubDirectories = new List<FolderContent>();
        public bool IsEmpty() => !Projects.Any() && SubDirectories.All(sd=>sd.IsEmpty());
        public Lazy<string> FolderId = new Lazy<string>(() => (FolderSequentialGuid++).CurrentGuid.ToString().ToUpper());
        public string Path;
        public bool IsRoot;
    }

    class SolutionGenerator
    {
        string Template =
@"Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 14
VisualStudioVersion = 14.0.25420.1
MinimumVisualStudioVersion = 10.0.40219.1
{0}Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
{2}	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
{1}	EndGlobalSection
EndGlobal
";


        private readonly Options _args;
        private HashSet<string> _excludedFolders;
        public SolutionGenerator(Options args)
        {
            _args = args;
            _excludedFolders = new HashSet<string>(_args.ExcludedFolders.Split(','));
        }

        public void Render()
        {
            var content = GetContent(_args.Folder);
            content.IsRoot = true; // Root folder is a special case

            StringBuilder projectSection = new StringBuilder();
            StringBuilder folderSection = new StringBuilder();
            StringBuilder buildConfigSection = new StringBuilder();

            WriteContent(content, projectSection, folderSection, buildConfigSection);

            var outputFile = Path.Combine(_args.Folder, _args.SolutionFileName);
            File.WriteAllText(
                outputFile,
                string.Format(Template, projectSection.ToString(), folderSection.ToString(), buildConfigSection.ToString())
                );

            Console.WriteLine($"Output written to {outputFile}.");
        }

        private void WriteContent(FolderContent content, StringBuilder projectSection, StringBuilder folderSection, StringBuilder buildConfigSection)
        {
            foreach (var dir in content.SubDirectories)
            {
                projectSection.AppendLine($"Project(\"{{2150E333-8FDC-42A3-9474-1A3956D46DE8}}\") = \"{ Path.GetFileName(dir.Path) }\", \"{ Path.GetFileName(dir.Path) }\", \"{{{dir.FolderId.Value}}}\"");
                projectSection.AppendLine("EndProject");

                if (!content.IsRoot)
                {
                    folderSection.AppendLine($"		{{{dir.FolderId.Value}}} = {{{content.FolderId.Value}}}");
                }
                WriteContent(dir, projectSection, folderSection, buildConfigSection);
            }

            foreach (var project in content.Projects)
            {
                var projectId = File.ReadAllText(Path.Combine(content.Path, project))  ;
                projectId = projectId.Substring(projectId.IndexOf("<ProjectGuid>") + 14, 36);

                projectSection.AppendLine($"Project(\"{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}\") = \"{ Path.GetFileNameWithoutExtension(project) }\", \"{ MakeRelative(project) }\", \"{{{projectId}}}\"");
                projectSection.AppendLine("EndProject");

                if (!content.IsRoot)
                    folderSection.AppendLine($"		{{{projectId}}} = {{{content.FolderId.Value}}}");

                buildConfigSection.AppendLine($"		{{{projectId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                buildConfigSection.AppendLine($"		{{{projectId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                buildConfigSection.AppendLine($"		{{{projectId}}}.Release|Any CPU.ActiveCfg = Debug|Any CPU");
                buildConfigSection.AppendLine($"		{{{projectId}}}.Release|Any CPU.Build.0 = Debug|Any CPU");
            }
        }

        private string MakeRelative(string path)
        {
            return path.Substring(_args.Folder.Length+1);
        }

        FolderContent GetContent(string folder)
        {
            if(_args.Verbose)
                Console.WriteLine(folder);
            var content = new FolderContent();
            content.Path = folder;
            try
            {
                foreach (var subFolder in Directory.GetDirectories(folder))
                {
                    if (_excludedFolders.Contains(Path.GetFileName(subFolder))) continue;

                    var subContent = GetContent(subFolder);
                    if (subContent.IsEmpty())
                        continue;
                    else if (subContent.SubDirectories.Count() == 0 && subContent.Projects.Count()==1) 
                        content.Projects.AddRange(subContent.Projects); // Flatten folders with only one project.
                    else
                        content.SubDirectories.Add(subContent);
                }
            }
            catch (PathTooLongException)
            { Console.Error.WriteLine($"PathTooLongException: {folder}"); }

            try
            {
                foreach (var subProj in Directory.GetFiles(folder, "*.CSPROJ"))
                {
                    content.Projects.Add(subProj);
                }
            }
            catch (PathTooLongException)
            { Console.Error.WriteLine($"PathTooLongException: {folder}"); }
            return content;
        }
    }
}
