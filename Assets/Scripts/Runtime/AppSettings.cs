namespace WorldInstrument
{
    using UnityEngine;

    /// <summary>
    /// Global settings for the Augmented Instrument Project!
    /// </summary>
    [CreateAssetMenu(fileName = "AppSettings", menuName = "ScriptableObjects/AppSettings")]
    public class AppSettings : ScriptableObject
    {
        public int BPM = 90;
    }
}
