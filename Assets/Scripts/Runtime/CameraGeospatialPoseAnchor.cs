namespace WorldEnsemble
{
    using UnityEngine;
    using UnityEngine.XR.ARSubsystems;
    using Unity.Mathematics;
    using CesiumForUnity;
    using Google.XR.ARCoreExtensions;

    [RequireComponent(typeof(CesiumGlobeAnchor))]
    public sealed class CameraGeospatialPoseAnchor : MonoBehaviour
    {
        [SerializeField]
        private AREarthManager _earthManager;

        [SerializeField]
        private Vector3 _angleOffsetIOS;

        private CesiumGlobeAnchor _globeAnchor;
        private Quaternion _rotationOffset = Quaternion.identity;

        private void Start()
        {
            _globeAnchor = GetComponent<CesiumGlobeAnchor>();

#if UNITY_IOS
            // FIXME: EunRotation is rotated 90 degrees on iOS platform.
            // Is this an issue in ARCore Extensions?
            _rotationOffset = quaternion.EulerXYZ(_angleOffsetIOS * Mathf.Deg2Rad);
#endif // UNITY_IOS
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
#if UNITY_IOS
            _rotationOffset = quaternion.EulerXYZ(_angleOffsetIOS * Mathf.Deg2Rad);
#endif // UNITY_IOS
        }
#endif // UNITY_EDITOR

        private void Update()
        {
            if (_earthManager.EarthTrackingState != TrackingState.Tracking)
            {
                return;
            }
            var geoPose = _earthManager.CameraGeospatialPose;

            _globeAnchor.longitudeLatitudeHeight = new(geoPose.Longitude, geoPose.Latitude, geoPose.Altitude);
            _globeAnchor.rotationEastUpNorth = geoPose.EunRotation * _rotationOffset;
        }
    }
}
