namespace WorldEnsemble
{
    using UnityEngine;
    using UnityEngine.Audio;
    using UnityEngine.Rendering;
    using UnityEngine.Scripting;

    /// <summary>
    /// Switch both VISUAL and AUDIO effect
    /// </summary>
    public sealed class WorldEffectSwitcher : MonoBehaviour
    {
        [System.Serializable]
        public sealed class EffectSet
        {
            public VolumeProfile volumeProfile;
            public AudioMixerSnapshot mixerSnapshot;
        }

        [SerializeField]
        private Volume _postEffectVolume;

        [SerializeField]
        [Range(0.0f, 8.0f)]
        private float _transitionDuration = 2f;

        [SerializeField]
        private EffectSet[] _effects;


        private int _currentEffectIndex = 0;

        // Called from event
        [Preserve]
        public void NextEffect()
        {
            // Next effect
            _currentEffectIndex = (_currentEffectIndex + 1) % _effects.Length;
            SwitchEffect(_effects[_currentEffectIndex]);

            Debug.Log($"Switch to effect: {_currentEffectIndex}");
        }

        public void SwitchEffect(EffectSet effect)
        {
            _postEffectVolume.profile = effect.volumeProfile;
            effect.mixerSnapshot.TransitionTo(_transitionDuration);
        }
    }
}
