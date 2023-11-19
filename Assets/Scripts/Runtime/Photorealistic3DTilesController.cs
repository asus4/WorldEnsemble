namespace WorldInstrument
{
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.Events;
    using CesiumForUnity;
    using Google.XR.ARCoreExtensions.Internal;

    /// <summary>
    /// Controller for Google Photorealistic 3D Tiles
    /// </summary>
    public sealed class Photorealistic3DTilesController : MonoBehaviour
    {

        [SerializeField]
        private RuntimeConfig _runtimeConfig;

        [SerializeField]
        private StreetscapeGeometryController _streetscapeGeometryController;

        [SerializeField]
        private CesiumGeoreference _georeference;

        public UnityEvent onTilesetLoaded;

        private const string BASE_USL = "https://tile.googleapis.com/v1/3dtiles/root.json?key=";

        private Cesium3DTileset _tileset;
        private bool _hasTilesetLoaded = false;

        private void Start()
        {
            _tileset = _georeference.GetComponentInChildren<Cesium3DTileset>();
            Assert.IsNotNull(_tileset, "No Cesium3DTileset found in children of CesiumGeoreference.");

            _streetscapeGeometryController.onEarthInitialized.AddListener(OnEarthManagerInitialized);
        }

        private void OnDestroy()
        {
            _streetscapeGeometryController.onEarthInitialized.RemoveListener(OnEarthManagerInitialized);
        }

        private void Update()
        {
            if (_hasTilesetLoaded || _tileset == null)
            {
                return;
            }

            float progress = _tileset.ComputeLoadProgress();
            if (progress >= 100)
            {
                _hasTilesetLoaded = true;
                onTilesetLoaded?.Invoke();
            }
        }

        private void OnEarthManagerInitialized(StreetscapeGeometryController.AREarthManagerEventArgs args)
        {
            var cameraTransform = args.origin.Camera.transform;
            var geoPose = args.earthManager.CameraGeospatialPose;

            // Sync CesiumGeoreference with ARCore pose
            cameraTransform.GetPositionAndRotation(out Vector3 position, out Quaternion rotation);

            // Not sure why 180 degree rotation?
            rotation *= Quaternion.Euler(0, 180, 0);
            _georeference.transform.SetPositionAndRotation(position, rotation);

            _georeference.SetOriginLongitudeLatitudeHeight(
                geoPose.Longitude,
                geoPose.Latitude,
                geoPose.Altitude);

            // Start updating tileset
            _tileset.url = BASE_USL + GetApiKey();
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

                string url = _runtimeConfig.IOSCloudServicesApiKey;
                if (string.IsNullOrEmpty(url))
                {
                    // TODO: Use Android API key
                    Debug.LogError("No API key found. Please try to build iOS first. Then build Android.");
                }
                return url;
            }
            Debug.LogError("No API key found for platform: " + platform);
            return string.Empty;
        }
    }
}
