using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    
    public static bool roamIsPaused = false;
    
    public GameObject PauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            if(roamIsPaused)
            {

                Resume();

            }

            else
            {
                
                Pause();

            }

        }

    }

    public void Resume()
    {

        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        roamIsPaused = false;
    }

    void Pause()
    {

        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        roamIsPaused = true;

    }
}
