using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // The ball
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    private bool isFollowing = false;

    public void StartFollowing()
    {
        isFollowing = true;
    }

    void LateUpdate()
    {
        if (isFollowing && target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}