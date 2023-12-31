// Modified by asus4 based on Googles Geospatial Sample

// <copyright file="GeospatialController.cs" company="Google LLC">
//
// Copyright 2022 Google LLC
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace WorldEnsemble
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;
    using Unity.XR.CoreUtils;
    using Google.XR.ARCoreExtensions;
    using UnityEngine.Assertions;

#if UNITY_ANDROID
    using UnityEngine.Android;
#endif

    /// <summary>
    /// Simple version of Geospatial mesh
    /// </summary>
    public sealed class StreetscapeGeometryController : MonoBehaviour
    {
        public sealed class AREarthManagerEventArgs
        {
            public AREarthManager earthManager;
            public XROrigin origin;
        }

        [System.Serializable]
        public sealed class AREarthManagerEvent : UnityEvent<AREarthManagerEventArgs> { }

        [SerializeField]
        private MeshFilter streetscapeGeometryPrefab;

        [SerializeField]
        private bool calculateNormal = false;
        [SerializeField]
        private bool calculateTangent = false;

        [SerializeField]
        [Range(0.01f, 0.5f)]
        private float accuracySDThreshold = 0.2f;


        public AREarthManagerEvent onEarthInitialized;

        private readonly Dictionary<TrackableId, GameObject> _streetScapeGeometries = new();
        private XROrigin _origin;
        private AREarthManager _earthManager;
        private ARStreetscapeGeometryManager _streetScapeGeometryManager;



        private void Awake()
        {
            _origin = FindObjectOfType<XROrigin>();
            Assert.IsNotNull(_origin, "No XROrigin found in the scene.");
            _earthManager = _origin.GetComponent<AREarthManager>();
            _streetScapeGeometryManager = _origin.GetComponent<ARStreetscapeGeometryManager>();
        }

        public void OnEnable()
        {
            _streetScapeGeometryManager.StreetscapeGeometriesChanged += GetStreetscapeGeometry;
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Debug.Log("Stop location services.");
            Input.location.Stop();
            _streetScapeGeometryManager.StreetscapeGeometriesChanged -= GetStreetscapeGeometry;
        }

        private IEnumerator Start()
        {
            yield return StartLocationService();
            Debug.Log("Location services started.");

            yield return new WaitUntil(() =>
                ARSession.state != ARSessionState.SessionInitializing);

            Debug.Log("ARSession state: " + ARSession.state);

            var featureSupport = _earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
            if (featureSupport != FeatureSupported.Supported)
            {
                Debug.LogWarning("Geospatial mode is not supported.");
            }

            yield return new WaitUntil(() =>
                _earthManager.EarthState != EarthState.ErrorEarthNotReady);

            var earthState = _earthManager.EarthState;
            if (earthState != EarthState.Enabled)
            {
                Debug.LogWarning($"Geospatial sample encountered an EarthState error: {earthState}");
                yield break;
            }

            yield return WaitUntilAccuracyConverge(120, accuracySDThreshold);

            var args = new AREarthManagerEventArgs
            {
                earthManager = _earthManager,
                origin = _origin,
            };
            onEarthInitialized?.Invoke(args);
        }


        private IEnumerator WaitUntilAccuracyConverge(int frameCount, double threshold)
        {
            Queue<double> accuracyQueue = new();

            // Wait until queue is full
            yield return new WaitUntil(() =>
            {
                var pose = _earthManager.CameraGeospatialPose;
                accuracyQueue.Enqueue(pose.OrientationYawAccuracy);
                return accuracyQueue.Count > frameCount;
            });

            // Wait until accuracy converge
            yield return new WaitUntil(() =>
            {
                var pose = _earthManager.CameraGeospatialPose;
                accuracyQueue.Dequeue();
                accuracyQueue.Enqueue(pose.OrientationYawAccuracy);
                double stdDev = accuracyQueue.StandardDeviation();
                Debug.Log($"yar accuracy: {pose.OrientationYawAccuracy}, StandardDeviation: {stdDev}");
                return stdDev < threshold;
            });
        }

        private void GetStreetscapeGeometry(ARStreetscapeGeometriesChangedEventArgs eventArgs)
        {
            foreach (var added in eventArgs.Added)
            {
                AddOrUpdateRenderObject(added);
            }
            foreach (var updated in eventArgs.Updated)
            {
                AddOrUpdateRenderObject(updated);
            }
            foreach (var removed in eventArgs.Removed)
            {
                DestroyRenderObject(removed);
            }
        }

        private void AddOrUpdateRenderObject(ARStreetscapeGeometry geometry)
        {
            if (geometry.mesh == null)
            {
                return;
            }

            Pose pose = geometry.pose;

            // Check if a render object already exists for this streetscape geometry and
            // create one if not.
            if (_streetScapeGeometries.TryGetValue(geometry.trackableId, out GameObject go))
            {
                // Just update the pose.
                go.transform.SetPositionAndRotation(pose.position, pose.rotation);
                return;
            }

            var mesh = geometry.mesh;
            if (calculateNormal)
            {
                mesh.RecalculateNormals();
            }
            if (calculateTangent)
            {
                mesh.RecalculateTangents();
            }

            var meshFilter = Instantiate(streetscapeGeometryPrefab);
            go = meshFilter.gameObject;

            meshFilter.sharedMesh = mesh;
            if (go.TryGetComponent(out MeshCollider collider))
            {
                collider.sharedMesh = mesh;
            }

            go.transform.SetParent(transform, false);
            go.transform.SetPositionAndRotation(pose.position, pose.rotation);
            _streetScapeGeometries.Add(geometry.trackableId, go);
        }

        private void DestroyRenderObject(ARStreetscapeGeometry geometry)
        {
            if (_streetScapeGeometries.TryGetValue(geometry.trackableId, out GameObject go))
            {
                Destroy(go);
                _streetScapeGeometries.Remove(geometry.trackableId);
            }
        }

        private IEnumerator StartLocationService()
        {
#if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                Debug.Log("Requesting the fine location permission.");
                Permission.RequestUserPermission(Permission.FineLocation);
                yield return new WaitForSeconds(3.0f);
            }
#endif

            if (!Input.location.isEnabledByUser)
            {
                Debug.Log("Location service is disabled by the user.");
                yield break;
            }

            Debug.Log("Starting location service.");
            Input.location.Start();

            while (Input.location.status == LocationServiceStatus.Initializing)
            {
                yield return null;
            }

            if (Input.location.status != LocationServiceStatus.Running)
            {
                Debug.LogWarningFormat(
                    "Location service ended with {0} status.", Input.location.status);
                Input.location.Stop();
            }
        }
    }
}
