namespace LCM.Website.Dtos.Request
{
    public class BsEighteen
    {
        public class Upload
        {
            public IFormFile postFile { get; set; }
            public string uploadType { get; set; }
        }

        public class InsertReport 
        { 
            public string filePath { get; set; }
        }
    }
}
