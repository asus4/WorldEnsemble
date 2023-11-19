namespace WorldEnsemble
{
    using UnityEngine;

    /// <summary>
    /// Hide object in non-development builds.
    /// </summary>
    public sealed class DevelopmentOnlyObject : MonoBehaviour
    {
        private void Start()
        {
            bool isVisible = Debug.isDebugBuild;
            gameObject.SetActive(isVisible);
        }
    }
}
