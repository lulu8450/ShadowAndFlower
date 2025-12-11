using UnityEngine;
using System.Collections.Generic;

public class cameraSwitch : MonoBehaviour
{
    [System.Serializable]
    public class CameraZone
    {
        public Transform cameraTransform;
        public string triggerName; // Name of the animator trigger (e.g., "Camera1", "Camera2")
        public float detectionRadius = 10f;
    }

    [SerializeField] private List<CameraZone> cameraZones = new List<CameraZone>();
    [SerializeField] private Animator animator;
    [SerializeField] private float checkInterval = 0.5f;

    private float nextCheckTime = 0f;
    private CameraZone currentCamera = null;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (cameraZones.Count == 0)
        {
            Debug.LogWarning("cameraSwitch: No camera zones assigned!", this);
        }
    }

    private void Update()
    {
        // Check for nearest camera periodically
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            UpdateNearestCamera();
        }
    }

    private void UpdateNearestCamera()
    {
        CameraZone nearestCamera = null;
        float nearestDistance = float.MaxValue;

        // Find the closest camera zone
        foreach (CameraZone zone in cameraZones)
        {
            if (zone == null || zone.cameraTransform == null)
                continue;

            float distance = Vector3.Distance(transform.position, zone.cameraTransform.position);

            if (distance < zone.detectionRadius && distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCamera = zone;
            }
        }

        // Switch camera if a new one is nearest
        if (nearestCamera != null && nearestCamera != currentCamera)
        {
            SwitchCamera(nearestCamera);
        }
        // If no camera is in range and we had one, reset
        else if (nearestCamera == null && currentCamera != null)
        {
            currentCamera = null;
            // Optionally trigger a "default" or reset state
        }
    }

    private void SwitchCamera(CameraZone newCamera)
    {
        // Reset previous camera trigger if needed
        if (currentCamera != null && !string.IsNullOrEmpty(currentCamera.triggerName))
        {
            animator.ResetTrigger(currentCamera.triggerName);
        }

        // Switch to new camera
        currentCamera = newCamera;
        if (animator != null && !string.IsNullOrEmpty(newCamera.triggerName))
        {
            animator.SetTrigger(newCamera.triggerName);
            Debug.Log($"Camera switched to: {newCamera.triggerName}", this);
        }
    }

    // Manually set a camera zone (if you want to switch via code)
    public void SetCameraZone(int index)
    {
        if (index >= 0 && index < cameraZones.Count)
        {
            SwitchCamera(cameraZones[index]);
        }
    }

    // Get the current active camera
    public CameraZone GetCurrentCamera()
    {
        return currentCamera;
    }
}
