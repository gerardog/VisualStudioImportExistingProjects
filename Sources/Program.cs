using CommandLine;
using System;
using System.IO;

[assembly: CommandLine.AssemblyUsageAttribute("Finds all Visual Studio projects in a folder tree and adds them to a new solution file.")]

namespace SolutionGenerator
{
    public class Options
    {
        [Option('f', "folder", HelpText = "Folder to traverse and where the solution will be generated.", Required = true)]
        public string Folder { get; set; }
        [Option('o', "output", HelpText = "FileName of the solution to create.", DefaultValue = "AllProjects.sln")]
        public string SolutionFileName { get; set; }
        [Option('e', "exclude", HelpText = "Comma separated list of folders to exclude.", DefaultValue = ".git,bin,obj,packages,node_modules")]
        public string ExcludedFolders { get; set; }
        [Option('v', "verbose", HelpText = "Verbose")]
        public bool Verbose { get; set; }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var options = new Options();
            var isValid = Parser.Default.ParseArgumentsStrict(args, options);
            options.Folder = Path.GetFullPath(options.Folder);
            if (isValid)
                new SolutionGenerator(options).Render();
        }

    }
}
