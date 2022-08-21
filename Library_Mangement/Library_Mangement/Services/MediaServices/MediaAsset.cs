using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Services.MediaServices
{
    public class MediaAsset
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TempPath { get; set; }
        public MediaAssetType Type { get; set; }
        public string PreviewPath { get; set; }
        public string Path { get; set; }
    }

    public enum MediaAssetType
    {
        Image, Video
    }
}
