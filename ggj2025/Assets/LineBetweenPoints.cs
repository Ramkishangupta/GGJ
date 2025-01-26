using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineBetweenPoints : MonoBehaviour
{
	public Transform pointA; // First Transform
	public Transform pointB; // Second Transform

	private LineRenderer lineRenderer;

	void Start()
	{
		// Get or add the LineRenderer component
		lineRenderer = GetComponent<LineRenderer>();

		// Set the number of line points
		lineRenderer.positionCount = 2;

		// Optional: Configure LineRenderer settings
		lineRenderer.startWidth = 0.3f; // Width at the start of the line
		lineRenderer.endWidth = 0.3f;   // Width at the end of the line
		lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Default material
		lineRenderer.startColor = Color.white;
	}

	void Update()
	{
		if (pointA != null && pointB != null)
		{
			// Update the positions of the line endpoints
			lineRenderer.SetPosition(0, pointA.position); // Start of the line
			lineRenderer.SetPosition(1, pointB.position); // End of the line
		}
	}
}
