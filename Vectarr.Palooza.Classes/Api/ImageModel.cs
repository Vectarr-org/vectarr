using System.Text.Json.Serialization;
using Vectarr.Palooza.Classes.BaseApi;

namespace Vectarr.Palooza.Classes.Api
{
    public class ImageModel : BaseApiModel
    {
        public string OriginalPath
        {
            get
            {
                return BaseStorageUrl + RelativeOriginalPath;
            }
        }

        public string OriginalThumbnailPath
        {
            get
            {
                if (!string.IsNullOrEmpty(RelativeThumbnailOriginalPath))
                {
                    return BaseStorageUrl + RelativeThumbnailOriginalPath;
                }
                else if (!string.IsNullOrEmpty(OriginalPath))
                {
                    return OriginalPath;
                }
                else
                {
                    return "";
                }
            }
        }

        [JsonIgnore]
        public string? RelativeThumbnailOriginalPath { get; set; }


        [JsonIgnore]
        public string? RelativeOriginalPath { get; set; }

        [JsonIgnore]
        public string? BaseStorageUrl { get; set; }

        public string? AltText { get; set; }

        [JsonIgnore]
        public int ApplicationId { get; set; }

        public object? Photographer { get; set; }
    }
}
