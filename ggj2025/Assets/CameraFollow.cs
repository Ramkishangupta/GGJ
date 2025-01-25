using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target and Offset")]
    public Transform target; // The player or object to follow
    public Vector3 offset; // Offset of the camera from the target

    [Header("Smoothness")]
    public float smoothSpeed = 5f; // Speed of the camera's smooth transition

    private void FixedUpdate(){
        transform.position = target.position + offset   ;
    }
}
