namespace WorldInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Google.XR.ARCoreExtensions.Internal;
    using CesiumForUnity;

    [RequireComponent(typeof(CesiumGeoreference))]
    public sealed class GeoTilesetController : MonoBehaviour
    {
        [SerializeField]
        private RuntimeConfig _runtimeConfig;

        [SerializeField]
        private CesiumGeoreference _georeference;

        [SerializeField]
        private Cesium3DTileset _tileset;

        private const string BASE_USL = "https://tile.googleapis.com/v1/3dtiles/root.json?key=";

        private void Start()
        {
            string apiKey = GetApiKey();
            _tileset.url = BASE_USL + apiKey;
        }

        private string GetApiKey()
        {
            var platform = Application.platform;
            if (Application.isEditor || platform == RuntimePlatform.IPhonePlayer)
            {
                return _runtimeConfig.IOSCloudServicesApiKey;
            }
            if (platform == RuntimePlatform.Android)
            {
                // TODO: Use Android API key
                return _runtimeConfig.IOSCloudServicesApiKey;
            }
            Debug.LogError("No API key found for platform: " + platform);
            return string.Empty;
        }
    }
}
