namespace AugmentedInstrument
{
    using UnityEngine;

    public sealed class RaycastCursor : MonoBehaviour
    {
        [SerializeField]
        private Transform _tip;

        private void Awake()
        {
            SetRaycastHitNone();
        }


        public void SetRaycastHit(in Ray ray, in RaycastHit hit)
        {
            _tip.gameObject.SetActive(true);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.back, hit.normal);
            _tip.SetPositionAndRotation(hit.point, rotation);
        }

        public void SetRaycastHitNone()
        {
            _tip.gameObject.SetActive(false);
        }
    }
}
