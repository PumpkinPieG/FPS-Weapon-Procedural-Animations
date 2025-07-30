using UnityEngine;

public class GunBob : MonoBehaviour
{
    // Reference to the PlayerMovement script to access player velocity and sprint state
    public PlayerMovement playerMovement;

    // Frequency of the bobbing motion (how fast the gun bobs)
    public float bobFrequency = 8f;

    // Vertical bob amplitude (how much the gun moves up and down)
    public float bobAmplitudeY = 0.025f;

    // Horizontal bob amplitude (side-to-side movement)
    public float bobAmplitudeX = 0.015f;

    // Additional sideways bob amplitude when sprinting
    public float sprintSideBobAmplitude = 0.05f;

    // Smoothness factor for interpolation between positions and rotations
    public float bobSmooth = 10f;

    // How much the gun dips downward vertically when sprinting
    public float sprintDipAmount = 0.1f;

    // Multiplier to reduce bob amplitude when sprinting
    public float sprintBobMultiplier = 0.5f;

    // Rotation applied to the gun when sprinting (Euler angles)
    public Vector3 sprintRotationEuler = new Vector3(21f, -81.5f, 0f);

    // Amount to shift the gun left when sprinting (local X axis)
    public float sprintLeftShiftAmount = 0.15f;

    // The original local position of the gun (used as the base position)
    private Vector3 initialPosition;

    // The original local rotation of the gun (used as the base rotation)
    private Quaternion initialRotation;

    // Timer to drive the bobbing oscillation
    private float timer;

    // Target position the gun will smoothly move toward
    private Vector3 targetPosition;

    // Target rotation the gun will smoothly rotate toward
    private Quaternion targetRotation;

    void Start()
    {
        // Store the initial local position and rotation of the gun at start
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        targetPosition = initialPosition;
        targetRotation = initialRotation;
    }

    void FixedUpdate()
    {
        // Get the current velocity of the player from the CharacterController
        Vector3 velocity = playerMovement.controller.velocity;

        // Determine if the player is moving and grounded (threshold velocity magnitude > 0.1)
        bool isMoving = velocity.magnitude > 0.1f && playerMovement.isGrounded;

        // Check if the player is sprinting from the PlayerMovement script
        bool isSprinting = playerMovement.isSprinting;

        if (isMoving)
        {
            // Increment timer based on fixed delta time and bob frequency to animate the bob
            timer += Time.fixedDeltaTime * bobFrequency;

            // Start with base bob amplitudes
            float amplitudeX = bobAmplitudeX;
            float amplitudeY = bobAmplitudeY;

            // Vertical dip amount (used to lower gun while sprinting)
            float verticalDip = 0f;

            // Horizontal shift amount (used to move gun left while sprinting)
            float horizontalShift = 0f;

            // Additional side bob offset (used to simulate sideways motion fixed relative to world)
            Vector3 sideBobOffset = Vector3.zero;

            if (isSprinting)
            {
                // Reduce bob amplitude to make bobbing less floaty while sprinting
                amplitudeX *= sprintBobMultiplier;
                amplitudeY *= sprintBobMultiplier;

                // Apply vertical dip to lower the gun a bit while sprinting
                verticalDip = -sprintDipAmount;

                // Shift the gun left along local X axis while sprinting
                horizontalShift = -sprintLeftShiftAmount;

                // Calculate sideways bob fixed relative to world right direction:

                // World right direction (always fixed on the X axis)
                Vector3 worldRight = Vector3.right;

                // Camera's local right vector (gun is child of camera)
                Vector3 cameraRight = transform.parent.right;

                // Dot product gives how much the camera right axis aligns with world right
                float alignment = Vector3.Dot(worldRight.normalized, cameraRight.normalized);

                // Calculate a sideways bob oscillation using sine wave, multiplied by sprint side bob amplitude
                float sideBobAmount = Mathf.Sin(timer * 2f) * sprintSideBobAmplitude;

                // Sideways bob amount adjusted by alignment factor so it stays fixed relative to world right
                float sideBobLocalX = sideBobAmount * alignment;

                // Side bob offset on local X axis only
                sideBobOffset = new Vector3(sideBobLocalX, 0, 0);
            }

            // Calculate horizontal bob position with cosine wave plus sprint left shift offset
            float bobX = Mathf.Cos(timer) * amplitudeX + horizontalShift;

            // Calculate vertical bob position with absolute sine wave plus vertical dip offset
            float bobY = Mathf.Abs(Mathf.Sin(timer)) * amplitudeY + verticalDip;

            // Calculate final target position combining initial position + bobbing + sprint side bob offset
            targetPosition = initialPosition + new Vector3(bobX, bobY, 0) + sideBobOffset;

            // Set target rotation depending on sprint state
            targetRotation = isSprinting
                ? Quaternion.Euler(sprintRotationEuler) // Use sprint rotation while sprinting
                : initialRotation;                      // Use initial rotation when not sprinting
        }
        else
        {
            // If player is not moving, reset timer and target position/rotation to initial values
            timer = 0;
            targetPosition = initialPosition;
            targetRotation = initialRotation;
        }

        // Smoothly interpolate the gun's local position towards target position
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, Time.fixedDeltaTime * bobSmooth);

        // Smoothly interpolate the gun's local rotation towards target rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.fixedDeltaTime * bobSmooth);
    }
}
