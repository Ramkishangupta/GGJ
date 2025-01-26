using UnityEngine;

public class EndlessBackground : MonoBehaviour
{
	public float speed = 2f; // Speed at which the background moves
	public float width = 20f; // Width of the movement along x-axis
	public float height = 5f; // Height of the movement along y-axis

	private Vector3 startPosition;
	private float time;

	void Start()
	{
		startPosition = transform.position;
		time = 0f;
	}

	void Update()
	{
		// Smooth sine wave motion in both x and y axes
		time += speed * Time.deltaTime;
		float xOffset = Mathf.Sin(time) * width;
		float yOffset = Mathf.Cos(time) * height;
		transform.position = startPosition + new Vector3(xOffset, yOffset, 0);
	}
}