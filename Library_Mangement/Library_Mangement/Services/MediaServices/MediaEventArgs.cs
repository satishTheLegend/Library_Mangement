using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Services.MediaServices
{
    public class MediaEventArgs : EventArgs
    {
        public MediaAsset Media { get; }
        public MediaEventArgs(MediaAsset media)
        {
            Media = media;
        }
    }
}
