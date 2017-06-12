using System.Text.RegularExpressions;

namespace MigrateToVSTSWiki
{
    public static class PathHelper
    {
        /// <summary>
        /// This is only to support links if in case they were not mentioned correctly in the extension.
        /// For the links to have worked in the extension, it should have already obeyed these rules.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string NormalizePath(string path)
        {
            string normalizedPath = null;

            normalizedPath = Regex.Replace(path, @"[^/^\w\.]|[_]", "-");
            normalizedPath = Regex.Replace(normalizedPath, @"-+", "-");
            normalizedPath = Regex.Replace(normalizedPath, @"-$", "");

            return normalizedPath;
        }
    }
}
