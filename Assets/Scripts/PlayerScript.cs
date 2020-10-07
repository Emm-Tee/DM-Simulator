using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{


    /* 
     *
     *
     * Player Attributes 
     * 
     * 
     */

    //Components
    
    public string playerName;
    public int playerSeat = 0;
    public int tier;                            //How many points the player has to go towards attributes 
    private float agreeModPercent;
    private float socialChemNeutral;

    // Player traits

    private int numAttributeTraits;
    private int maxTraitScore = 10;
    private float maxAttPoints;

    //public int availability;                  //How available they are for sessions --- removed for now
    public int agreeability;                    //What variance from their preferance they are okay with before unhappiness;
    public int socialChemistry;                 //How well they get along with other players in the group
    private int maxEnthusiasm;                   //How much they like DnD
    private int oosChats;                        //How much they participate in out of session chats (Increases enjoyment)
    private int stressResilliance;               //How much stress they can handle before they don't enjoy/ break

    //DnD preferences     
    public int combatRP;                        //What balance between combat and RP they enjoy (Scale with mid being equal, low combat, high RP)
    public int drama;                           //How much they enjoy drama
    public int mystery;                         //How much they enjoy mysteries / unanswered questions

    //Campaign related
    public int timeSinceJoined;                 //How long they've been playing
    public int dmRelations;                     //How well the DM knows them 
    public float enthusiasm;                      //How much they're enjoying the campaign 
    public float sessionEnjoyment;                //How much they enjoyed the previous session.


    //Session related
    float sessionDrama;
    float sessionCombat;
    float sessionMystery;

    public int sessionPointsTS;
    public int sessionPointsTotal;

    public int dmRelationsIncrease;

    //UI  
    public SpriteRenderer bodySprite;
    public SpriteRenderer headSprite;

    public GameObject reactionSessionObject;
    public GameObject reactionRelationCover;

    public GameObject reactionCombatObject;
    public GameObject reactionDramaObject;
    public GameObject reactionMysteryObject;

    int sessionReactionSpriteNo;
    int sessionEnthusiasmSpriteNo;

    int combatSpriteNo;
    int dramaSpriteNo;
    int mysterySpriteNo;

    public GameObject speechBubble;

    public Color[] enthusiasmColours = new Color[6];


    /*
     * Debug 
     */

    private int tier1 = 0;
    private int tier2 = 0;
    private int tier3 = 0;
    private int tier4 = 0;
    private int tierOther = 0;

    public float attributePoints;

    //public int[] attList;
    private int[] distributedAttPoints;


    //public int goTo;                          //The attribute to assign point to
    private int[] attList;                      //The list to generate goTo from
    private int[] availAttList;                 //The list to generate attList from
    private int availAttNum;                    //The number of attributes left not full (how many attList indices are valid)
    private int placedAttListNums;              //Index to place non-full attribute numbers into attList
    


    // Start is called before the first frame update
    void Start()
    {
        
        numAttributeTraits = 5;
        availAttNum = numAttributeTraits;
        maxAttPoints = numAttributeTraits * maxTraitScore;
        agreeModPercent = 0.4f;
        socialChemNeutral = 4;

        playerName = getAName();
        tier = getTier();
        distributePoints(tier);
        generatePrefences();

        dmRelations = Random.Range(0, 40);
        dmRelationsIncrease = 2;



        //Manage UI
        speechBubble = GameObject.Find(string.Concat("SB", playerSeat));
        //choose frame
        speechBubble.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/Speech/", playerSeat, "/", Random.Range(1,16)));            
        speechBubble.SetActive(false);

        int spriteNo = Random.Range(1, 5);

        bodySprite = GetComponent<SpriteRenderer>();
        bodySprite.sprite = Resources.Load<Sprite>(string.Concat("Art/Players/PC", playerSeat, "/", playerSeat, ".", Random.Range(1, 5)));

        if(playerSeat == 1 || playerSeat == 4)
        {
            bodySprite.sortingLayerName = "BodiesBehindTable";
        }

        headSprite = transform.Find("Head").GetComponent<SpriteRenderer>();
        headSprite.sprite = Resources.Load<Sprite>(string.Concat("Art/Players/PC", playerSeat, "/", playerSeat, ".a", Random.Range(1, 5)));

        transform.position = GameObject.Find(string.Concat("Pos_Player_", playerSeat)).transform.position;
        transform.localScale = GameObject.Find(string.Concat("Pos_Player_", playerSeat)).transform.localScale;

        GameObject.Find(string.Concat("Name_P", playerSeat)).GetComponent<Text>().text = playerName;

        reactionSessionObject = GameObject.Find(string.Concat("Seat", playerSeat, "Reaction"));
        reactionRelationCover = GameObject.Find(string.Concat("Seat", playerSeat, "DMRelations"));
        reactionRelationCover.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/UI/DMRelations/", playerSeat, ".", dmRelations /10));

        reactionCombatObject = GameObject.Find(string.Concat("Seat", playerSeat,"Combat"));
        reactionDramaObject = GameObject.Find(string.Concat("Seat", playerSeat, "Drama"));
        reactionMysteryObject = GameObject.Find(string.Concat("Seat", playerSeat, "Mystery"));


        //Starting Enthusiasm

        if (maxEnthusiasm < 5)
            enthusiasm = 4;
        else
            enthusiasm = 5;

        sessionEnthusiasmSpriteNo = Mathf.RoundToInt(5 - ((enthusiasm / 2) + 0.1f));

        bodySprite.color = enthusiasmColours[sessionEnthusiasmSpriteNo];
        headSprite.color = enthusiasmColours[sessionEnthusiasmSpriteNo];


    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKeyDown("space"))
        {

            availAttNum = numAttributeTraits;

            attList = new int[numAttributeTraits];                      //List of Attributes for attribute points to be distributed into
            availAttList = new int[numAttributeTraits];
            distributedAttPoints = new int[numAttributeTraits];

            for (int i = 0; i < numAttributeTraits; i++)
            {
                attList[i] = i;                                         //Generate indices 
                availAttList[i] = i;
            }

            tier = getTier();
            distributePoints(tier);
        }
        */

    }
    
    int getTier()
    {
        float tierPerc = Random.Range(0, 100);

        int tier;

        if (tierPerc > 100 * .9)
            tier = 1;
        else if (tierPerc > 100 * .75)
            tier = 2;
        else if (tierPerc > 100 * .4)
            tier = 3;
        else
            tier = 4;

        return tier;
    }

    //Distribute the points across the attribute traits 
    void distributePoints(int tier)
    {
        float attMin;
        float attMax;

        float min1 = .9f;
        float min2 = .8f;
        float min3 = .6f;
        float min4 = .3f;

        if (tier == 1)
        {
            attMin = min1;
            attMax = 1f;
        }
        else if (tier == 2)
        {
            attMin = min2;
            attMax = min1;
        }
        else if (tier == 3)
        {
            attMin = min3;
            attMax = min2;
        }
        else
        {
            attMin = min4;
            attMax = min3;
        }

        attributePoints = Mathf.RoundToInt(Random.Range(maxAttPoints * attMin, maxAttPoints * attMax)); //Randomise how many points gotten within tier's max and min.


        attList = new int[numAttributeTraits];                          //List of Attributes for attribute points to be distributed into
        availAttList = new int[numAttributeTraits];                     //List of A 
        distributedAttPoints = new int[numAttributeTraits];             //Array of points to be distributed into
        //removedAttNums = new int[numAttributeTraits];


        for (int i = 0; i < numAttributeTraits; i++)
        {
            attList[i] = i;                                         //Generate indices 
            availAttList[i] = i;
        }


        for (int i = 0; i < attributePoints; i++)
        {
            int goTo = attList[Random.Range(0, availAttNum)];           //Attribute to assign the point to. 
            distributedAttPoints[goTo]++;                               //Assign point to attribute

            if (distributedAttPoints[goTo] >= maxTraitScore)            //If  attribute reaches it's amx score, don't let that attribute be randomly selected any more
            {
                availAttList[goTo] = -1;                                //Remove that attribute from the list you can select from
                availAttNum--;                                          //Reduce the know number of attributes to select from - (length of att list)
                placedAttListNums = 0;                                  //Inde marker of zero to scroll through attList

                for (int j = 0; j < numAttributeTraits; j++)            //For each attribute
                {
                    if (availAttList[j] != -1)                           //Check to see if that attribute is full (it's value changed to -1 means that it's full)
                    {
                        attList[placedAttListNums] = j;                 //If not full, put it onto the attList (placeAttListNums index makes sure they're all squished together)
                        placedAttListNums++;
                    }
                }
            }
        }

        agreeability = distributedAttPoints[0];
        socialChemistry = distributedAttPoints[1];
        maxEnthusiasm = distributedAttPoints[2];
        oosChats = distributedAttPoints[3];
        stressResilliance = distributedAttPoints[4];
    }
    
    void generatePrefences()
    {
        drama = Random.Range(0, maxTraitScore + 1);           //How much they enjoy drama 
        combatRP = Random.Range(0, maxTraitScore + 1);      //What balance between combat and RP they enjoy (Scale with mid being equal, low combat, high RP)
        mystery = Random.Range(0, maxTraitScore + 1);
    }
    
    void agreeMod()
    {
        /*
        sessionDrama =      ((1 + (drama * agreeModPercent)) * .1f);
        sessionCombat =     ((1 + (combatRP * agreeModPercent)) * .1f);
        sessionMystery =    ((1 + (mystery * agreeModPercent)) * .1f);
        */
    }

    public void playSession(int playerNumber, int sCombat, int sDrama, int sMystery)
    {
        //Debug.Log("Player " + playerNumber + " played the session!");


        //Equation (Absolute value of session score - preference score (higher value = greater difference = more unhappy))
        /* Equation
         * Mathf.Abs(preference score - session score)      == difference between preference and session --- higher means more unhappy
         * * 1 -                                            == modifying the unhappy value by a percentage of it - higher percentage = more unhappy, so need to invert the agreeability modifier so that high agreeability gets a low percentage
         * (agreeability * agreeModPercent * 0.1f)          == multiply agreeability score by the mod percent to affect how great an influence agreeability has on the unhappy value - modPercent is set to 0.4, so agreeability ranges from 0% t0 40% (inverted to 60% to 100% of unhappy) - 10 agreeability will change unhappy score to 60% of it's value
         */

        /* ----- Speech Bubble ----- */
        speechBubble.SetActive(true);


        timeSinceJoined++;

        if(dmRelations < 100)
        {
            dmRelations += dmRelationsIncrease;
            dmRelations += oosChats/3;
            
        }

        reactionRelationCover.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/UI/DMRelations/", playerSeat, ".", dmRelations /10));


        sessionCombat = Mathf.Abs(combatRP - sCombat) * (10 - (agreeability * agreeModPercent));
        sessionDrama = Mathf.Abs(drama - sDrama) * (10 - (agreeability * agreeModPercent));
        sessionMystery = Mathf.Abs(mystery - sMystery) * (10 - (agreeability * agreeModPercent));

        float socialChemMod = ((socialChemNeutral * agreeModPercent) + (10 - socialChemistry * agreeModPercent)) * .1f;

        sessionEnjoyment = sessionDrama + sessionCombat + sessionMystery;


        /*  Reaction UI  */
        if (sessionEnjoyment == 0)
        {
            sessionReactionSpriteNo = 0;
            sessionPointsTS = 5;
        }
        else if (sessionEnjoyment <= 60)
        {
            sessionReactionSpriteNo = 1;
            sessionPointsTS = 4;
        }
        else if (sessionEnjoyment <= 120)
        {
            sessionReactionSpriteNo = 2;
            sessionPointsTS = 3;
        }
        else if (sessionEnjoyment <= 180)
        {
            sessionReactionSpriteNo = 3;
            sessionPointsTS = 2;
        }
        else if (sessionEnjoyment <= 240)
        {
            sessionReactionSpriteNo = 4;
            sessionPointsTS = 1;
        }
        else
        {
            sessionReactionSpriteNo = 5;
            sessionPointsTS = 0;
        }

        sessionPointsTS -= 2;
  
        //Increase Max enthusiasm 
        
        if(enthusiasm == maxEnthusiasm)
        {
            if (sessionEnjoyment <= 20)
            {
                int rand = Random.Range(0, 40 - (agreeability - socialChemistry - oosChats));
                if (rand <= 4)
                    maxEnthusiasm++;
            }

        }


        sessionPointsTotal += sessionPointsTS;

        if(sessionPointsTotal >= 3)
        {
            if(enthusiasm < maxEnthusiasm)
                enthusiasm++;
            sessionPointsTotal -= 3;
        }
        else if( sessionPointsTotal <= -3)
        {
            enthusiasm--;
            sessionPointsTotal += 3;
        }
        if (enthusiasm > maxEnthusiasm)
        {
            enthusiasm--;
        }
        if (enthusiasm < 0)
            enthusiasm = 0;


        if (sessionCombat == 0)
            combatSpriteNo = 0;
        else if (sessionCombat <= 20)
            combatSpriteNo = 1;
        else if (sessionCombat <= 40)
            combatSpriteNo = 2;
        else if (sessionCombat <= 60)
            combatSpriteNo = 3;
        else if (sessionCombat <= 80)
            combatSpriteNo = 4;
        else
            combatSpriteNo = 5;

        if (sessionDrama == 0)
            dramaSpriteNo = 0;
        else if (sessionDrama <= 20)
            dramaSpriteNo = 1;
        else if (sessionDrama <= 40)
            dramaSpriteNo = 2;
        else if (sessionDrama <= 60)
            dramaSpriteNo = 3;
        else if (sessionDrama <= 80)
            dramaSpriteNo = 4;
        else
            dramaSpriteNo = 5;

        if (sessionMystery == 0)
            mysterySpriteNo = 0;
        else if (sessionMystery <= 20)
            mysterySpriteNo = 1;
        else if (sessionMystery <= 40)
            mysterySpriteNo = 2;
        else if (sessionMystery <= 60)
            mysterySpriteNo = 3;
        else if (sessionMystery <= 80)
            mysterySpriteNo = 4;
        else
            mysterySpriteNo = 5;

        //GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/Players/PC", playerSeat, "/", playerSeat, ".", spriteNo));


        sessionEnthusiasmSpriteNo = Mathf.RoundToInt(5- ((enthusiasm / 2) + 0.1f));

        //Enthusiasm colour

        bodySprite.color = enthusiasmColours[sessionEnthusiasmSpriteNo];
        headSprite.color = enthusiasmColours[sessionEnthusiasmSpriteNo];



        reactionSessionObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/UI/Faces/Face", sessionReactionSpriteNo)); //reaction session object is now actually enthusiasm display object but I'm too lazy to change the names
        

        reactionCombatObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/UI/Faces/Face", combatSpriteNo));
        reactionDramaObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/UI/Faces/Face", dramaSpriteNo));
        reactionMysteryObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(string.Concat("Art/UI/Faces/Face", mysterySpriteNo));
    }

    /*
     * Debug 
     */


    string getAName()
    {
        string[] namesList = {  "Aaron",    "Anne",     "Adam",     "Andy",     "Amy",      "Annie",    "Arthur",   "Alonso",   "Anette",   "Ava",
                                "Ben",      "Betty",    "Bert",     "Bill",     "Babe",     "Becky",    "Bethany",  "Bonnie",   "Blaise",   "Barney",
                                "Claire",   "Clarice",  "Carl",     "Connie",   "Charlie",  "Chance",   "Carlyle",  "Clarence", "Chris",    "Cooper",
                                "Danny",    "Deni",     "Daemon",   "Darlene",  "Desmond",  "Dorian",   "Dylan",    "Dwayne",   "Daria",    "Delila",
                                "Edward",   "Edna",     "Ellie",    "Edbert",   "Ellen",    "Esther",   "Ewan",     "Ebony",    "Emma",     "Ezibar",
                                "Freddy",   "Florence", "Frida",    "Fuschia",  "Frank",    "Fran",     "Finnan",   "Fergus",   "Ferris",   "Felicity",
                                "Greta",    "Geoff",    "George",   "Gretal",   "Gill",     "Greyson",  "Gary",     "Glen",     "Gladys",   "Gloria",
                                "Haylee",   "Hayden",   "Harper",   "Hansel",   "Henry",    "Harriet",  "Holly",    "Hilda",    "Holden",   "Hershal",
                                "Indiana",  "Isabell",  "Irene",    "Isla",     "Ian",      "Iona",     "Ivan",     "Igor",     "Isaac",    "Ifan",
                                "Jill",     "John",     "Jack",     "Janice",   "Jolene",   "Jerald",   "Johnny",   "Jezabell", "Jaime",    "Jan",
                                "Karl",     "Kyle",     "Kole",     "Kylie",    "Kadence",  "Kerry",    "Killian",  "Karen",    "Kieran",   "Kenn",
                                "Liam",     "Lindy",    "Linda",    "Lillian",  "Loris",    "Lana",     "Liona",    "Len",      "Leroy",    "Liz",
                                "Maurice",  "Mary",     "Manny",    "Milson",   "Milroy",   "Madison",  "Mark",     "Merry",    "Milly",    "Mike",
                                "Nerida",   "Niles",    "Naomi",    "Noel",     "Nina",     "Neri",     "Norbert",  "Nimeria",  "Ned",      "Nick",
                                "Oliver",   "Olivia",   "Oscar",    "Olaf",     "Opal",     "Owen",     "Olive",    "Otto",     "Otis",     "Osara",
                                "Penelo",   "Piper",    "Pippen",   "Philamena","Phineas",  "Phteven",  "Paris",    "Peter",    "Patrick",  "Peony",
                                "Quebert",  "Queen",    "Quanda",   "Quince",   "Quinbie",  "Quinn",    "Quade",    "Quill",    "Quintyn",  "Quasim",
                                "Rose",     "Rowan",    "Ryan",     "Rhylar",   "Rachel",   "Randy",    "Rupert",   "Rain",     "Roland",   "Reece",
                                "Sally",    "Steven",   "Suzie",    "Shane",    "Sophie",   "Sean",     "Sandra",   "Stewart",  "Sonia",    "Smark",
                                "Tully",    "Tony",     "Tyson",    "Tania",    "Thomas",   "Tilly",    "Terry",     "Tiffany", "Tobias",   "Tilda",
                                "Ursula",   "Uma",      "Uriel",    "Ulises",   "Ulysses",  "Uri",      "Usman",    "Urban",    "Ulmer",    "Unique",
                                "Verity",   "Vinny",    "Victor",   "Val",      "Valory",   "Vicky",    "Vince",    "Violet",   "Vassily",  "Vallory",
                                "Wilson",   "Wendy",    "Wanda",    "Wynona",   "William",  "Wynn",     "Wallice",  "Wilamena", "Wren",     "Winston",  "Wick",
                                "Xena",     "Xim",      "Xavier",   "Xander",   "Ximena",   "Xyla",     "Xia",      "Xandria",  "Xu",       "Xan",
                                "Yuni",     "Yani",     "Yvonne",   "Yvette",   "Yolanda",  "Yasmin",   "Yosef",    "Yukio",    "Ysabella", "Yuri",
                                "Zoe",      "Zed",      "Zaine",    "Zero",     "Zuni",     "Zander",   "Zach",     "Zerkis",   "Zeke",     "Zelda",};


        return namesList[Random.Range(0, namesList.Length)];
    }

    void TierTest()
    {
        tier1 = 0;
        tier2 = 0;
        tier3 = 0;
        tier4 = 0;
        tierOther = 0;

        float numPlayersTested = 20;

        for (int i = 0; i < numPlayersTested; i++)
        {
            int theTier = getTier();
            if (theTier == 1)
                tier1++;
            else if (theTier == 2)
                tier2++;
            else if (theTier == 3)
                tier3++;
            else if (theTier == 4)
                tier4++;
            else
                tierOther++;
        }

        tierOther = 0;
        tierOther = tier1 + tier2;
    }

}

//Random range's max value isn't inclusive so 0,2 will print 0s and 1s, 5,6 will print 5s 
