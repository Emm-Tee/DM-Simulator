using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{

    public Button quitButton;
    public Button startButton;

    public Image black;

    public AudioSource paperHover;
    public AudioSource paperPress;
    public AudioSource paperHover2;

    // Start is called before the first frame update
    void Start()
    {
        quitButton.onClick.AddListener(() => StartCoroutine(quitGame()));
        startButton.onClick.AddListener(() => StartCoroutine(startGame()));
    }


    IEnumerator startGame()
    {
        black.gameObject.SetActive(true);
        
        for (float t = 0.0f; t<1.0f; t+= Time.deltaTime/ 0.5f)
        {
            black.color = new Color(0,0,0, t);
            yield return null;
        }

        SceneManager.LoadScene("Scenes/Main", LoadSceneMode.Single);

    }
    
    IEnumerator quitGame()
    {
        black.gameObject.SetActive(true);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
        {
            black.color = new Color(0, 0, 0, t);
            yield return null;
        }
        Application.Quit();
    }

    public void playPaperHover()
    {
        paperHover.Play();
    }

    public void playPaperPress()
    {
        paperPress.Play();
    }

    public void playPaperHover2()
    {
        paperHover2.Play();
    }

}
