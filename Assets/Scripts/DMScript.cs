using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DMScript : MonoBehaviour
{

    /*
     *
     * Session Variables
     * 
     */

    public int styleCombatRP; //Balance between combat and RP (scale, mid means equal, low combat, high RP)
    public int drama; //Level of drama  - low means relaxed, high = drama;
    public int mystery; //Level of mystery in the story (scale, low = questions answered, mid is equal, high = questions posed)

    public AudioSource diceSoundSource;
  

    /* ----- Player Related ----- */

    public GameObject playerObject;
    public GameObject[] players;
    public int numPlayers;
    int maxPlayers;
    private PlayerScript playerScript;


    /* ----- Session vars ----- */

    private int sessionNo;


    /* ----- UI ----- */

    public Button button0;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button playButton;
    public Button quitButton;
    public Button quitYesB;
    public Button quitNoB;

    public Slider combatSlider;
    public Slider dramaSlider;
    public Slider mysterySlider;

    public Image black;

    public GameObject sessionCounterText;
    public GameObject quitConfirmationContainer;

    /* ----- Sounds ----- */
    public AudioSource startPress;
    public AudioSource trashPressPaperHover;
    public AudioSource trashHover;

    public AudioSource[] diceSounds = new AudioSource[3];
    public AudioSource[] shockedSounds = new AudioSource[5];

    // Start is called before the first frame update
    void Start()
    {
        maxPlayers = 4;
        numPlayers = 0;
        sessionNo = 0;


        button0.onClick.AddListener(() => delPlayer(0));
        button1.onClick.AddListener(() => delPlayer(1));
        button2.onClick.AddListener(() => delPlayer(2));
        button3.onClick.AddListener(() => delPlayer(3));


        playButton.onClick.AddListener(() => StartCoroutine(launchSession()));
        quitButton.onClick.AddListener(() => showQuitConfirmation());
        quitYesB.onClick.AddListener(() => QuitYes());
        quitNoB.onClick.AddListener(() => HideQuitConfirmation());

        players = new GameObject[maxPlayers];
        for(int i = 0; i < players.Length; i++)
        {
            createPlayer(i);
        }

        quitConfirmationContainer.SetActive(false);
        black.gameObject.SetActive(true);
        StartCoroutine(FadeFromBlack());


    }



    void createPlayer(int i)
    {
        players[i] = Instantiate(playerObject);
        players[i].transform.name = "Player" + numPlayers;
        players[i].GetComponent<PlayerScript>().playerSeat = i+1;
        numPlayers++;
    }

    void delPlayer(int i)
    {
        players[i].GetComponent<PlayerScript>().speechBubble.SetActive(true);
        players[i].GetComponent<PlayerScript>().reactionSessionObject.GetComponent<SpriteRenderer>().sprite = null;
        Destroy(players[i]);
        createPlayer(i);
    }

    IEnumerator launchSession()
    {
        
        
        StartPress(); //Play button sound

        //Fade black over
        StartCoroutine(FadeToBlack());
        yield return new WaitForSeconds(0.5f);


        //Do session things
        sessionNo++;
        sessionCounterText.GetComponent<Text>().text = sessionNo.ToString();

        styleCombatRP = Mathf.RoundToInt(combatSlider.value);
        drama = Mathf.RoundToInt(dramaSlider.value);
        mystery = Mathf.RoundToInt(mysterySlider.value);

        for (int i = 0; i < maxPlayers; i++)
        {

            playerScript = players[i].GetComponent<PlayerScript>();
            playerScript.playSession(i, styleCombatRP, drama, mystery);
        }

        //Sounds
        diceSounds[Random.Range(0, diceSounds.Length)].Play();

        int dramaLevel = Random.Range(0, 11);
        if (dramaLevel < drama)
        {
            shockedSounds[Random.Range(0, shockedSounds.Length)].Play();
        }

        yield return new WaitForSeconds(Random.Range(0.2f, 1.0f));
        StartCoroutine(FadeFromBlack());
    
    }
   

    IEnumerator FadeFromBlack() //Black vanishes
    {
        for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime / 0.5f)
        {
            black.color = new Color(0, 0, 0, t);
            yield return null;
        }
        black.gameObject.SetActive(false);

    }

    IEnumerator FadeToBlack()
    {
        black.gameObject.SetActive(true);
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.2f)
        {
            black.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }

    void showQuitConfirmation()
    {
        quitConfirmationContainer.SetActive(true);
    }

    void QuitYes()
    {
        StartCoroutine(FadeToBlack());  
        SceneManager.LoadScene("Scenes/Menu", LoadSceneMode.Single);
    }
    void HideQuitConfirmation()
    {
        quitConfirmationContainer.SetActive(false);
    }

    /* ----- Play Sounds ----- */
    public void StartPress()
    {
        startPress.Play();
    }
    public void TrashPressPaperHover()
    {
        trashPressPaperHover.Play();
    }
    public void TrashHover()
    {
        trashHover.Play();
    }
}
