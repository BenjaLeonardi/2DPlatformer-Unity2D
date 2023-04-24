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

    private void Die()
    {
        audioDeath.Play();
        //Guardamos el juego antes de perder control, en caso de que 
        saveManager.SaveGame();
        rbPlayer.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
