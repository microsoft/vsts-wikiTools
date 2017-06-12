using System.Collections.Generic;
using System.IO;

namespace MigrateToVSTSWiki
{
    public static class FileHelper
    {
        public static void MigrateFiles(string source, string destination)
        {
            var oldAttachmentsPath = Path.Combine(source, ExtensionWikiConstants.AttachmentsFolder);
            var newAttachmentsPath = Path.Combine(destination, VSTSWikiConstants.AttachmentsFolder);

            if (Directory.Exists(oldAttachmentsPath))
            {
                // Creating the attachments folder in the destination
                Directory.CreateDirectory(newAttachmentsPath);

                // Migrating attachment files
                foreach(string file in Directory.EnumerateFiles(oldAttachmentsPath, "*.*", SearchOption.TopDirectoryOnly))
                {
                    var attachmentName = PathHelper.NormalizePath(Path.GetFileName(file));   // Normalizing attachment name for standardizing the names and links
                    var targetAttachmentFile = Path.Combine(newAttachmentsPath, attachmentName);

                    File.Copy(file, targetAttachmentFile);

                    Logger.Log(string.Format("  Action: Attachment file migration complete    Source'{0}'    Destination: '{1}'", file, targetAttachmentFile));
                }
            }

            foreach(string file in Directory.EnumerateFiles(source, "*.md", SearchOption.AllDirectories))
            {
                var relativePath = file.Replace(source + "\\", "");
                var targetFullPath = Path.Combine(destination, relativePath);   // Normalizing not required since it is done by the extension

                // Create the required directories in the page's path
                Directory.CreateDirectory(Path.GetDirectoryName(targetFullPath));

                // 1. Migrate the page from the source to destination
                File.Copy(file, targetFullPath);
                File.SetAttributes(targetFullPath, FileAttributes.Normal);

                // 2. Add the page to the order file
                AddToOrderFile(targetFullPath);

                // 3. Update the links in the page
                LinkHelper.UpdateLinks(targetFullPath);

                Logger.Log(string.Format("  Action: File migration complete    Source: '{0}'    Destination: '{1}'", file, targetFullPath));
            }
        }

        private static void AddToOrderFile(string filePath)
        {
            var parentPath = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var orderFilePath = Path.Combine(parentPath, VSTSWikiConstants.OrderFile);

            using (StreamWriter writer = File.AppendText(orderFilePath))
            {
                writer.WriteLine(fileName);
            }
        }
    }
}
