using UnityEngine;

public class GunIdleSway : MonoBehaviour
{
    public PlayerMovement playerMovement; // Reference to your movement script

    [Header("Idle Movement Settings")]
    public float swayFrequency = 1.2f;
    public float swayAmplitudeX = 0.015f;
    public float swayAmplitudeY = 0.008f;
    public float swaySmooth = 6f;

    private Vector3 initialLocalPos;
    private float timer;
    private Vector3 targetLocalPos;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        targetLocalPos = initialLocalPos;
    }

    void FixedUpdate()
    {
        Vector3 velocity = playerMovement.controller.velocity;
        bool isIdle = velocity.magnitude <= 0.1f && playerMovement.isGrounded;

        if (isIdle)
        {
            timer += Time.fixedDeltaTime * swayFrequency;

            float swayX = Mathf.Sin(timer) * swayAmplitudeX;
            float swayY = Mathf.Cos(timer * 0.5f) * swayAmplitudeY;

            targetLocalPos = initialLocalPos + new Vector3(swayX, swayY, 0f);
        }
        else
        {
            timer = 0;
            targetLocalPos = initialLocalPos;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetLocalPos, Time.fixedDeltaTime * swaySmooth);
    }
}
