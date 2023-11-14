namespace AugmentedInstrument
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "AppSettings", menuName = "ScriptableObjects/AppSettings")]
    public class AppSettings : ScriptableObject
    {
        public int BPM = 90;
    }
}
