

namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// An instrument that is instantiated in the AR world.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public sealed class ARInstrument : MonoBehaviour
    {
        private AudioSource _audioSource;
        private readonly RhythmMachine _rhythmMachine = RhythmMachine.Instance;

        void Start()
        {
            Debug.Log("Hello, ARInstrument!");
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlaySound()
        {
            Debug.Log("Playing sound!");
            _audioSource.PlayScheduled(0.2);
        }
    }
}
