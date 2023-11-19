namespace WorldEnsemble
{
    using UnityEngine;

    /// <summary>
    /// Global settings for the app.
    /// </summary>
    [CreateAssetMenu(fileName = "AppSettings", menuName = "ScriptableObjects/AppSettings")]
    public class AppSettings : ScriptableObject
    {
        /// <summary>
        /// 1/4 beat per minute
        /// </summary>
        public int BPM = 90;

        /// <summary>
        /// Offset 1/16 beat each X meter
        /// </summary>
        public double offset16BeatByDistanceMeter = 2.0;

        /// <summary>
        /// Speed of the sound in air: 340 m/s at 20C temperature 
        /// </summary>
        public double soundSpeedInAir = 340.0;
    }
}
