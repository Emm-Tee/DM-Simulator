using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    public float dmRelation;//

    public int maxTraitScore;//

    public int tier;//
    public int socailChem;//
    public int oosc;//

    public int dmRelationMaxPointsPerSession;
    public float tierModContributionToMaxPoints;
    public float socialChemModContributionToMaxPoints;
    public float ooscModContributionToMaxPoints;
    public float sessionEnjoymentModMin;

    public int enthusiasmPointsThisSession;

    public float tierPoints;
    public float socialChemPoints;
    public float ooscPoints;
    public float enthusiasmMod;

    // Start is called before the first frame update
    void Start()
    {
        dmRelation = 0;

        maxTraitScore = 10;

        tier = 1;
        socailChem = 0;
        oosc = 0;

        dmRelationMaxPointsPerSession = 10;
        tierModContributionToMaxPoints = .6f;
        socialChemModContributionToMaxPoints = .2f;
        ooscModContributionToMaxPoints = .2f;
        sessionEnjoymentModMin = 0.5f;

        enthusiasmPointsThisSession = 0;


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            foip();    
        }
    }

    public void foip()
    {
         tierPoints = (dmRelationMaxPointsPerSession * tierModContributionToMaxPoints) - ((tier - 1) * ((dmRelationMaxPointsPerSession * tierModContributionToMaxPoints) / 3));
         socialChemPoints = socailChem / (maxTraitScore / (dmRelationMaxPointsPerSession * socialChemModContributionToMaxPoints));
         ooscPoints = oosc / (maxTraitScore / (dmRelationMaxPointsPerSession * ooscModContributionToMaxPoints));

         enthusiasmMod = (Mathf.Abs(enthusiasmPointsThisSession)) / 3.0f * (1.0f-sessionEnjoymentModMin) + sessionEnjoymentModMin;

        dmRelation += ((tierPoints + socialChemPoints + ooscPoints)*enthusiasmMod);
    }
}
