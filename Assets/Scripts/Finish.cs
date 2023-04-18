using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    //No hace falta serializefield porque es un solo audio el que puede emitir
    private AudioSource checkpointSource;
    private bool finished = false;
    private void Start()
    {
        checkpointSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "Player" && !finished)
        {
            finished = true;
            checkpointSource.Play();
            Invoke("CompleteLevel", 2f);
        }
    }

    private void CompleteLevel()
    {
        //Esto cuando uso Using UnityEditor.SceneManagement
        //EditorSceneManager.LoadScene(EditorSceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
