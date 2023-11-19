namespace WorldEnsemble
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    /// <summary>
    /// Gets normalized loudness from AudioListener.
    /// </summary>
    public static class LoudnessMeter
    {
        private static readonly float[] _outputSamples = new float[512];
        // TODO: should replace with circular buffer?
        private const int HISTORY_COUNT = 128;
        private static readonly Queue<float> _history = new(HISTORY_COUNT); // roughly 2 seconds

        private const float FALL_DOWN_RATE = 0.5f;
        private static float _loudness = 0.0f;

        /// <summary>
        /// Get Loudness.
        /// </summary>
        /// <returns>Raw Loudness</returns>
        public static float GetLoudness()
        {
            AudioListener.GetOutputData(_outputSamples, 0);
            float loudness = _outputSamples.Sum(x => Mathf.Abs(x)) / _outputSamples.Length;

            // slow down the change
            _loudness = loudness < _loudness
                ? Mathf.Lerp(_loudness, loudness, Time.deltaTime / FALL_DOWN_RATE)
                : loudness;
            return _loudness;
        }

        /// <summary>
        /// Get Normalized Loudness.
        /// </summary>
        /// <returns>The Loudness normalized in range of 0 to 1</returns>
        public static float GetLoudnessNormalized()
        {
            float loudness = GetLoudness();

            _history.Enqueue(loudness);
            if (_history.Count > HISTORY_COUNT)
            {
                _history.Dequeue();
            }

            // Normalize into 0.0 - 1.0
            float min = float.MaxValue;
            float max = float.MinValue;
            foreach (float value in _history)
            {
                min = Mathf.Min(min, value);
                max = Mathf.Max(max, value);
            }

            float normalized = Mathf.InverseLerp(min, max, loudness);
            return normalized;
        }
    }
}
