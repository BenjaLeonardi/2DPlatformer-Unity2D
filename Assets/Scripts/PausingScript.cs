using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausingScript : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject hud;
    [SerializeField] private GameObject player;
    private bool paused;
    private PlayerMovemente playermov;

    void Start()
    {
        playermov = player.GetComponent<PlayerMovemente>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseGame();
        }
    }

    private void pauseGame()
    {
        if (!paused)
        {
            paused = true;
            pauseMenu.SetActive(true);
            hud.SetActive(false);
            Time.timeScale = 0f;
            playermov.enabled = false;
        }
        else if (paused)
        {
            paused = false;
            pauseMenu.SetActive(false);
            hud.SetActive(true);
            Time.timeScale = 1f;
            playermov.enabled = true;
        }
    }
}
