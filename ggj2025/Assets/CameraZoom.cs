using UnityEngine;
using Unity.Cinemachine;

public class CameraZoom : MonoBehaviour
{
	public CinemachineCamera virtualCamera; // Reference to the Cinemachine virtual camera
	public Transform player; // Reference to the player object
	public float maxZoomOut = 10f; // Maximum zoom out value (camera size)
	public float minZoomIn = 5f; // Minimum zoom in value (camera size)
	public float zoomSpeed = 1f; // How quickly the camera zooms in/out
	public float maxSpeed = 10f; // Speed threshold for max zoom out

	private Rigidbody2D playerRigidbody; // Rigidbody of the player to track velocity

	void Start()
	{
		if (player == null)
		{
			Debug.LogError("Player transform is not assigned.");
			return;
		}

		if (virtualCamera == null)
		{
			Debug.LogError("CinemachineVirtualCamera is not assigned.");
			return;
		}

		// Get player's Rigidbody2D to track velocity
		playerRigidbody = player.GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (playerRigidbody == null) return;

		// Get the player's velocity
		float playerSpeed = playerRigidbody.linearVelocity.magnitude;

		// Calculate the desired orthographic size based on the player speed
		float targetSize = Mathf.Lerp(minZoomIn, maxZoomOut, playerSpeed / maxSpeed);

		// Smoothly adjust the camera's orthographic size
		virtualCamera.Lens.OrthographicSize = Mathf.Lerp(virtualCamera.Lens.OrthographicSize, targetSize, Time.deltaTime * zoomSpeed);
	}
}
