namespace ChiDaram.Common.Classes
{
    public class SoftwareConfig
    {
        public string Version { get; set; }
        public string WebRootPath { get; set; }
        public string BackupDirectoryPath { get; set; }
        public string UploadFolderName { get; set; }
        public string UploadDirectoryPath { get; set; }
        public string FileServerBaseAddress { get; set; }
        public string FileServerUsername { get; set; }
        public string FileServerPassword { get; set; }
        public string Aria2ExePath { get; set; }
        public string FFmpegExePath { get; set; }
        public string ViewSwaggerPassword { get; set; }
    }
}
