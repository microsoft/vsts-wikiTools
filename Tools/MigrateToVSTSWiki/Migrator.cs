using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MigrateToVSTSWiki
{
    public class Migrator
    {
        private static string source = null;
        private static string destination = null;

        static void Main(string[] args)
        {
            PopulateArguments(args);

            if(source == null || destination == null)
            {
                // This will also take care of '/?'
                PrintHelpText();
                return;
            }

            if(!Directory.Exists(source) || !Directory.Exists(destination))
            {
                Logger.Log("The source and/or destination directory is not available. Please retry with an existing directory paths as input.", LogType.Error);
                return;
            }

            if (Directory.EnumerateFileSystemEntries(destination).Any())
            {
                Logger.Log("The destination directory is not empty. Please retry with an empty destination directory.", LogType.Error);
                return;
            }

            // Initiate migration
            FileHelper.MigrateFiles(source, destination);

            Console.WriteLine("\nPress Enter key to exit...");
            Console.ReadLine();
        }

        private static void PopulateArguments(string[] args)
        {
            if (args == null || args.Length != 2)
            {
                return;
            }

            var trimChars = new char[] { '\"', ' ', '\\' };
            foreach(var arg in args)
            {
                Match sourceMatch, destinationMatch = null;

                sourceMatch = Regex.Match(arg, "/source:(?<paramValue>.+)");
                destinationMatch = Regex.Match(arg, "/destination:(?<paramValue>.+)");

                if(sourceMatch.Success)
                {
                    source = sourceMatch.Groups["paramValue"].Value.Trim(trimChars);
                }

                if (destinationMatch.Success)
                {
                    destination = destinationMatch.Groups["paramValue"].Value.Trim(trimChars);
                }
            }
        }

        private static void PrintHelpText()
        {
            Console.Write(@"
            ooooo   ooooo           oooo             
            `888'   `888'           `888             
             888     888   .ooooo.   888  oo.ooooo.  
             888ooooo888  d88' `88b  888   888' `88b 
             888     888  888ooo888  888   888   888 
             888     888  888    .o  888   888   888 
            o888o   o888o `Y8bod8P' o888o  888bod8P' 
                                           888       
                                          o888o  

            Arguments:

                /source          -     Root folder of the wiki created using the wiki extension

                /destination     -     Destination path to create the migrated files

            Example: 
                MigrateToVSTSWiki /source:C:\OldWiki\_wiki /destination:C:\NewWiki  
                ");
        }
    }
}
