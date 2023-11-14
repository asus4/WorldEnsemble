namespace AugmentedInstrument
{
    using UnityEngine;

    public sealed class DevelopmentOnlyObject : MonoBehaviour
    {
        private void Start()
        {
            bool isVisible = Debug.isDebugBuild;
            gameObject.SetActive(isVisible);
        }
    }
}
