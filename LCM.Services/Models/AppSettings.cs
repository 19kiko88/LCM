namespace LCM.Services.Models
{
    public class AppSettings
    {
        public const string _PathSettings = "PathSettings";

        public class PathSettings
        {
            public string UploadPath { get; set; }
            public string TemplateFilePath { get; set; }
        }
    }
}
