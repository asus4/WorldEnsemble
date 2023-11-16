namespace WorldInstrument
{
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.InputSystem;

    using Gyroscope = UnityEngine.InputSystem.Gyroscope;

    /// <summary>
    /// Rotating panel based on gyroscope (mobile), mouse position (PC).
    /// </summary>
    public class GyroParallaxPanel : MonoBehaviour
    {
        [SerializeField]
        private float _horizontalAngleScale = 30f;

        [SerializeField]
        private float _verticalAngleScale = 30f;

        [SerializeField]
        private float _clampMaxVelocityMagnitude = 2f;

        [SerializeField]
        [Range(0.01f, 3f)]
        private float _smoothTime = 0.5f;

        private Vector3 _velocity;

        private void Start()
        {
            if (Application.isMobilePlatform)
            {
                Assert.IsNotNull(Gyroscope.current, "Gyroscope is not supported on this device.");
                InputSystem.EnableDevice(Gyroscope.current);
            }
        }

        private void Update()
        {
            float delta = Time.deltaTime;
            Vector3 newVelocity = Vector3.ClampMagnitude(GetAngularVelocity(), _clampMaxVelocityMagnitude);
            _velocity = Vector3.Lerp(_velocity, newVelocity, delta / _smoothTime);

            Vector3 angles = new(
                _velocity.y * _verticalAngleScale,
                _velocity.x * _horizontalAngleScale,
                0);
            transform.localRotation = Quaternion.Euler(angles);
        }

        private Vector3 GetAngularVelocity()
        {
            if (Application.isMobilePlatform)
            {
                Vector3 v = Gyroscope.current.angularVelocity.ReadValue();
                return new Vector3(v.y, v.x, v.z);
            }
            else
            {
                Vector3 v = Mouse.current.delta.ReadValue();
                return new Vector3(-v.x, v.y, 0);
            }
        }
    }
}
