namespace AugmentedInstrument
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Scripting;

    /// <summary>
    /// Switches between canvas groups with a fade animation.
    /// </summary>
    public sealed class CanvasGroupSwitcher : MonoBehaviour
    {
        [SerializeField]
        private CanvasGroup[] _groups;

        [SerializeField]
        private int _currentGroupIndex = 0;

        [SerializeField]
        private AnimationCurve _fadeCurve;

        private void Start()
        {
            // Initial setup
            for (int i = 0; i < _groups.Length; i++)
            {
                var group = _groups[i];

                bool isCurrent = i == _currentGroupIndex;
                group.gameObject.SetActive(isCurrent);
                group.interactable = isCurrent;
                group.alpha = isCurrent ? 1 : 0;
            }
        }

        // Called from UI events
        [Preserve]
        public void Switch(int index)
        {
            Debug.Log($"Switching to {index}");

            CanvasGroup prevGroup = _groups[_currentGroupIndex];
            CanvasGroup newGroup = _groups[index];
            StartCoroutine(Animate(prevGroup, newGroup));

            _currentGroupIndex = index;
        }

        private IEnumerator Animate(CanvasGroup prevGroup, CanvasGroup newGroup)
        {
            // Initial setup
            prevGroup.gameObject.SetActive(true);
            prevGroup.interactable = false;
            prevGroup.alpha = 1;
            newGroup.gameObject.SetActive(true);
            newGroup.interactable = false;
            newGroup.alpha = 0;

            // Tween
            float fadeDuration = _fadeCurve[_fadeCurve.length - 1].time;
            float t = 0;
            while (true)
            {
                t += Time.deltaTime;
                if (t > fadeDuration)
                {
                    break;
                }
                float alpha = _fadeCurve.Evaluate(t);
                prevGroup.alpha = 1 - alpha;
                newGroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }

            // Final setup
            prevGroup.gameObject.SetActive(false);
            prevGroup.interactable = false;
            prevGroup.alpha = 0;
            newGroup.gameObject.SetActive(true);
            newGroup.interactable = true;
            newGroup.alpha = 1;
        }
    }
}
