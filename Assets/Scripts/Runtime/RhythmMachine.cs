using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AugmentedInstrument
{
    public sealed class RhythmMachine
    {
        public static RhythmMachine Instance { get; } = new RhythmMachine();

        public double DspTime => AudioSettings.dspTime - _startDspTime;

        private double _startDspTime;
        private readonly List<ARInstrument> _instruments = new();

        // DSP time for shader
        private static readonly int _DspTimeID = Shader.PropertyToID("_DspTime");

        public void Start()
        {
            _startDspTime = AudioSettings.dspTime;
        }

        public void Tick()
        {
            float dspTime = (float)(AudioSettings.dspTime - _startDspTime);
            Shader.SetGlobalFloat(_DspTimeID, dspTime);
        }

        public void RegisterInstrument(ARInstrument instrument)
        {
            _instruments.Add(instrument);
        }

        public void PlaySound()
        {
            foreach (ARInstrument instrument in _instruments)
            {
                instrument.PlaySound();
            }
        }

    }
}
