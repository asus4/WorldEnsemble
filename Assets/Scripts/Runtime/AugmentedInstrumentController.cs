
namespace AugmentedInstrument
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Entry point for the Augmented Instrument Project!
    /// </summary>
    public sealed class AugmentedInstrumentController : MonoBehaviour
    {
        [SerializeField]
        private float _dspTime;

        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

        private void Awake()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            _dspTime = Time.realtimeSinceStartup;
            Shader.SetGlobalFloat(_DspTimeID, _dspTime);
        }
    }
}
