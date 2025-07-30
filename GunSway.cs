using UnityEngine;

public class GunSway : MonoBehaviour
{
    public PlayerMovement playerMovement;

    [Header("Position Sway")]
    public float amount = 0.02f;
    public float maxAmount = 0.06f;
    public float smoothAmount = 6f;

    [Header("Rotation Sway")]
    public float rotationAmount = 4f;
    public float maxRotationAmount = 5f;
    public float smoothRotation = 12f;

    [Space]
    public bool rotationX = true, rotationY = true, rotationZ = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float inputX, inputY;

    private void Start()
    {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    void LateUpdate()
    {
        CalculateSway();
        MoveSway();
        TiltSway();
    }

    private void CalculateSway()
    {
        if (!playerMovement.isSprinting)
        {
            inputX = Input.GetAxis("Mouse X");
            inputY = Input.GetAxis("Mouse Y");
        }
        else
        {
            // Reset input when sprinting to avoid lingering sway
            inputX = 0f;
            inputY = 0f;
        }
    }

    private void MoveSway()
    {
        if (!playerMovement.isSprinting)
        {
            float moveX = Mathf.Clamp(inputX * amount, -maxAmount, maxAmount);
            float moveY = Mathf.Clamp(inputY * amount, -maxAmount, maxAmount);

            Vector3 finalPosition = new Vector3(moveX, moveY, 0);

            transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, smoothAmount * Time.deltaTime);
        }
    }

    private void TiltSway()
    {
        if (!playerMovement.isSprinting)
        {
            float tiltY = Mathf.Clamp(inputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
            float tiltX = Mathf.Clamp(inputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

            // Roll (Z) is half the tiltY for a subtler effect
            float tiltZ = rotationZ ? tiltY * 0.5f : 0f;

            Quaternion finalRotation = Quaternion.Euler(
                rotationX ? -tiltX : 0f,
                rotationY ? tiltY : 0f,
                tiltZ);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, smoothRotation * Time.deltaTime);
        }
    }
}
