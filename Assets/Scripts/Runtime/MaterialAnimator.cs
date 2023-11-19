namespace WorldEnsemble
{
    using UnityEngine;

    /// <summary>
    /// Exposes material parameter for animation.
    /// </summary>
    public sealed class MaterialAnimator : MonoBehaviour
    {
        public enum ControlType
        {
            Float,
            Vector
        }

        [SerializeField]
        private Material _material;

        [SerializeField]
        private ControlType controlType;

        [SerializeField]
        private string parameterName;

        public float floatValue;
        public Vector4 vectorValue;

        private float _prevFloatValue = float.NegativeInfinity;
        private Vector4 _prevVectorValue = Vector4.negativeInfinity;

        private int _parameterId;

        private void Start()
        {
            _parameterId = Shader.PropertyToID(parameterName);
        }

        private void Update()
        {
            switch (controlType)
            {
                case ControlType.Float:
                    UpdateFloatValue();
                    break;
                case ControlType.Vector:
                    UpdateVectorValue();
                    break;
            }
        }

        private void UpdateFloatValue()
        {
            if (floatValue != _prevFloatValue)
            {
                _material.SetFloat(_parameterId, floatValue);
                _prevFloatValue = floatValue;
            }
        }

        private void UpdateVectorValue()
        {
            if (vectorValue != _prevVectorValue)
            {
                _material.SetVector(_parameterId, vectorValue);
                _prevVectorValue = vectorValue;
            }
        }
    }
}
