namespace AugmentedInstrument
{
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.InputSystem;

    /// <summary>
    /// Rotating panel based on gyroscope (mobile), mouse position (PC).
    /// </summary>
    public class GyroParallaxPanel : UIBehaviour
    {
        [SerializeField]
        private float horizontalAngle = 30f;

        [SerializeField]
        private float verticalAngle = 30f;

        private void Update()
        {
            if (Application.isMobilePlatform)
            {
                Vector3 rotation = Input.gyro.attitude.eulerAngles;
                transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -rotation.z);
            }
            else
            {
                Vector2 screenSize = new(Screen.width, Screen.height);
                Vector2 position = Mouse.current.position.ReadValue() / screenSize;
                Vector3 angles = new(
                    Mathf.Lerp(-verticalAngle, verticalAngle, position.y),
                    Mathf.Lerp(-horizontalAngle, horizontalAngle, 1f - position.x),
                    0);
                transform.localRotation = Quaternion.Euler(angles);
            }
        }
    }
}
