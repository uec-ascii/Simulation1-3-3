using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks; // Import the Cysharp.Threading.Tasks namespace
using System.Threading; // Import the System.Threading namespace

public class Customer : MonoBehaviour
{
    public float defaultDuration = 0.2f; // Duration of the movement
    public Vector3 targetPosition; // Target position to move towards
    CancellationTokenSource cts; // Cancellation token source for task cancellation
    [SerializeField] Renderer _render;
    [SerializeField] float s,v;

    public void Move(float duration = -1)
    {
        if(cts!=null)
        {
            cts.Cancel(); // Cancel the previous task if it exists
        }
        cts = new CancellationTokenSource(); // Create a new cancellation token source
        CancellationToken token = cts.Token; // Get the cancellation token

        if (duration < 0) // If duration is not provided, use the default duration
        {
            duration = defaultDuration;
        }
        if(duration <= 0) // If duration is zero or negative, return without doing anything
        {
            transform.position = targetPosition; // Set the position directly to the target
            return;
        }

        _Move(duration, token).Forget(); // Start the movement task and forget about it
    }

    async UniTask _Move(float duration, CancellationToken token){
        Vector3 startPosition = transform.position; // Store the starting position
        float elapsedTime = 0f; // Initialize elapsed time

        while (elapsedTime < duration)
        {
            if (token.IsCancellationRequested) // Check if the task has been cancelled
            {
                break; // Exit the loop if cancelled
            }

            elapsedTime += Time.deltaTime; // Update elapsed time
            float t = Mathf.Clamp01(elapsedTime / duration); // Calculate interpolation factor
            transform.position = Vector3.Lerp(startPosition, targetPosition, t); // Interpolate position

            await UniTask.Yield(); // Yield control to allow other tasks to run
        }
        transform.position = targetPosition; // Ensure final position is set to target
    }

    void Initialize()
    {
        // Rendererのマテリアルの色をランダムに設定する、ただしHSVからで、sとvは固定
        Color color = Random.ColorHSV(0, 1, s, s, v, v); // Generate a random color with fixed saturation and value
        _render.material.color = color; // Set the material color
        Master.Instance.CustomerMoveFuncs += Move; // Subscribe to the customer move function
    }

    void Awake()
    {
        Initialize(); // Call the initialization method
    }

    public void OnDestroy()
    {
        if(Master.Instance != null) Master.Instance.CustomerMoveFuncs -= Move; // Unsubscribe from the customer move function
    }
}
