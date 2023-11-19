namespace WorldEnsemble
{
    using UnityEngine;
    using UnityEngine.Assertions;
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
        private Vector3 _angleOffset;

        private CesiumGlobeAnchor _globeAnchor;
        private Quaternion _rotationOffset;

        private void Start()
        {
            _globeAnchor = GetComponent<CesiumGlobeAnchor>();
            _rotationOffset = quaternion.EulerXYZ(_angleOffset * Mathf.Deg2Rad);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _rotationOffset = quaternion.EulerXYZ(_angleOffset * Mathf.Deg2Rad);
        }
#endif

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
