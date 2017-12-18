using System;

namespace WebRanging.Sites
{
    public class FileInfo
    {
        public string OwnerHost { get; set; } 
        public string Filename { get; set; } 
        public Lazy<string> Content { get; set; } 
    }
}