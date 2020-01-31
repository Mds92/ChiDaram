using ChiDaram.Common.Classes;
using ZetaLongPaths;

namespace ChiDaram.Common.Helper
{
    public static class FileHelper
    {
        public static string NormalizeFileSize(long fileSize)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            double size = fileSize;
            var unit = 0;
            while (size >= 1024)
            {
                size /= 1024;
                ++unit;
            }
            return $"{size:0.#} {units[unit]}";
        }

        public static string GetTempFolderPath(SoftwareConfig softwareConfig)
        {
            return ZlpPathHelper.Combine(softwareConfig.WebRootPath, Constants.UploadFolderNameTemp);
        }
        public static string GetRelativeTempFileUrl(string fileName)
        {
            return ZlpPathHelper.Combine(Constants.UploadFolderNameTemp, fileName);
        }

        public static bool IsHtml(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) return false;
            extension = extension.ToLower().Replace(".", "");
            return extension == "html" || extension == "htm";
        }
        public static bool IsCompress(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) return false;
            extension = extension.ToLower().Replace(".", "");
            return
                extension == "tar" || extension == "bz2" || extension == "lzma" ||
                extension == "7z" || extension == "s7z" || extension == "sfx" || extension == "gz" ||
                extension == "tgz" || extension == "zip" || extension == "rar" ||
                extension == "bzip2" || extension == "xz" || extension == "wim" ||
                extension == "rzip" || extension == "cab" || extension == "gzip";
        }
        public static bool IsExe(string extension)
        {
            if (string.IsNullOrWhiteSpace(extension)) return false;
            extension = extension.ToLower().Replace(".", "");
            return extension == "exe";
        }
    }
}
