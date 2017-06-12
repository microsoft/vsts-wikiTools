using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace MigrateToVSTSWiki
{
    public static class LinkHelper
    {
        public static void UpdateLinks(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }

            string fileContent = File.ReadAllText(file);
            Dictionary<string, string> allLinks = new Dictionary<string, string>(StringComparer.Ordinal);

            // Processing the image links
            foreach(Match match in Regex.Matches(fileContent, @"[!]\[(?<name>.*)\]\((?<link>.+)\)"))
            {
                // The match is for pattern ![name](link)
                var name = match.Groups["name"].Value;
                var link = PathHelper.NormalizePath(match.Groups["link"].Value);   // Normalizing attachment name for standardizing the names and links
                Uri uri = null;
                
                if(!allLinks.ContainsKey(match.Value) && Uri.TryCreate(link, UriKind.Relative, out uri))
                {
                    // We process only the relative linked attachments
                    allLinks.Add(match.Value, string.Format(VSTSWikiConstants.ImageLink, name, string.Format(VSTSWikiConstants.AttachmentLink, link)));
                }
            }

            // Processing the page links
            foreach (Match match in Regex.Matches(fileContent, @"(?<nonImageMarker>[^!]{1})\[(?<name>.*)\]\((?<prefix>/.[^/]+)(?<link>/.+)\)"))
            {
                // The match is for pattern x[name](/prefix/link), where 'x' is not !
                var name = match.Groups["name"].Value;
                var link = PathHelper.NormalizePath(match.Groups["link"].Value);    // Normalizing link for standardizing the names and links
                var nonImageMarker = match.Groups["nonImageMarker"].Value;

                if (!allLinks.ContainsKey(match.Value))
                {
                    // The Regex is for relative links onlys, so we need no further check
                    allLinks.Add(match.Value, nonImageMarker + string.Format(VSTSWikiConstants.PageLink, name, link));

                    Logger.Log(string.Format("   Action: Converting link:{0}", link));
                }
            }

            // Replace the old links with the constructed links
            foreach(var key in allLinks.Keys)
            {
                fileContent = fileContent.Replace(key, allLinks[key]);
            }

            Logger.Log(string.Format("  Action: Link processing complete    File: {0}    Links processed: {1}", file, allLinks.Count));

            // Write content back to the file
            File.WriteAllText(file, fileContent);
        }
    }
}
