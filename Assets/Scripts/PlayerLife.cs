using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLife : MonoBehaviour
{
    [SerializeField] private AudioSource audioDeath;
    [SerializeField] private DataPersistenceManagaer saveManager;
    private Rigidbody2D rbPlayer;
    private Animator anim;
    private GameObject spawnPoint;
    private void Start()
    {
        anim = GetComponent<Animator>();
        rbPlayer = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }

        if (collision.gameObject.CompareTag("Death"))
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Room"))
        {
            //Obtengo el spawnpoint que esta en el Room con el que estamos colisionando
            spawnPoint = collision.gameObject.GetComponent<RoomCamera>().spawnPoint;

            //Al entrar en contacto el personaje se mueve inmediatamente a dicho spawnpoint, para imitar lo que hace celeste al entrar a cada room y evitar colisiones indeseadas
            transform.position = Vector3.MoveTowards(transform.position, spawnPoint.transform.position, gameObject.GetComponent<PlayerMovemente>().maxSpeed);
        }
    }

    private void Die()
    {
        audioDeath.Play();
        //Guardamos el juego antes de perder control del personaje en caso de que agarremos una cereza en el ultimo frame
        saveManager.SaveGame();
        //Checkpoint
        transform.position = new Vector3(spawnPoint.transform.position.x, spawnPoint.transform.position.y, transform.position.z);
        //rbPlayer.bodyType = RigidbodyType2D.Static;
        //anim.SetTrigger("death");
    }

    //Se llama al morir desde la animacion, no me parece una buena forma de manejarlo. Podria utilizar coroutines para timearlo y que sea mas facilmente accesible
    //desde el codigo y no desde el editor
    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
