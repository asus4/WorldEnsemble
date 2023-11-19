namespace WorldEnsemble
{
    using UnityEngine;

    public sealed class RaycastCursor : MonoBehaviour
    {
        [SerializeField]
        private Transform _tip;

        [SerializeField]
        private AudioSource _raycastHitAudio;

        [SerializeField]
        private AudioSource _raycastNoneAudio;

        private void Awake()
        {
            SetRaycastHitNone();
        }

        public void SetRaycastHit(in Ray ray, in RaycastHit hit)
        {
            _tip.gameObject.SetActive(true);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.back, hit.normal);
            _tip.SetPositionAndRotation(hit.point, rotation);

            if (!_raycastHitAudio.isPlaying)
            {
                _raycastHitAudio.Play();
            }
        }

        public void SetRaycastHitNone()
        {
            _raycastHitAudio.Stop();
            _tip.gameObject.SetActive(false);
        }
    }
}
