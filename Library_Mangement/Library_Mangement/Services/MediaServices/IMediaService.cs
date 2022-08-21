using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Library_Mangement.Services.MediaServices
{
    public interface IMediaService
    {
        event EventHandler<MediaEventArgs> OnMediaAssetLoaded;
        bool IsLoading { get; }
        Task<IList<MediaAsset>> RetrieveMediaAssetsAsync(CancellationToken? token = null);
        Task<IList<MediaAsset>> LoadMediaAsync();
    }
}
