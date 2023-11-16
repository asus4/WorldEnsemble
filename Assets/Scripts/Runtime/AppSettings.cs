namespace WorldInstrument
{
    using UnityEngine;

    /// <summary>
    /// Global settings for the app.
    /// </summary>
    [CreateAssetMenu(fileName = "AppSettings", menuName = "ScriptableObjects/AppSettings")]
    public class AppSettings : ScriptableObject
    {
        public int BPM = 90;
        public double Offset16BeatByDistanceMeter = 2.0;
    }
}
