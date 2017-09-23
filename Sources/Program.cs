using Fclp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionGenerator
{
    public class Arguments
    {
        public string Folder { get; set; }
        public string SolutionFileName { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var parser = new FluentCommandLineParser<Arguments>();

            parser.Setup(a => a.Folder)
                .As('f', "folder")
                .SetDefault(Directory.GetCurrentDirectory());

            parser.Setup(a => a.SolutionFileName)
                .As('d', "dest")
                .Required();

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.WriteLine( result.ErrorText);
                Console.Read();
                return;
            }

            parser.Object.Folder = Path.GetFullPath(parser.Object.Folder);

            new SolutionGenerator(parser.Object).Render();
        }
    }
}
