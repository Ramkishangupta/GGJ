using UnityEngine;
using UnityEngine.SceneManagement;

public class won : MonoBehaviour
{
    public GameObject panel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Restart()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.tag == "Player")
        {
            panel.SetActive(true);
        }
	}
}
