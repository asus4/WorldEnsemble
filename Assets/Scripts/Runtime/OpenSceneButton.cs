namespace AugmentedInstrument
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public sealed class OpenSceneButton : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        private void OnEnable()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnButtonTap);
        }

        private void OnDisable()
        {
            var button = GetComponent<Button>();
            button.onClick.RemoveListener(OnButtonTap);
        }

        private void OnButtonTap()
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
