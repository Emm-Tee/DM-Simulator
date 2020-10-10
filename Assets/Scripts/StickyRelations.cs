using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyRelations : MonoBehaviour
{
    int playerNo;
     

    public enum sessionAspects
    {
        Combat, Drama, Mystery
    }

    public sessionAspects sessionAspect;
    public Sprite[] frames = new Sprite[3];

    // Start is called before the first frame update
    void Awake()
    {
        string gettingSeatNo = this.transform.parent.transform.parent.name;
                
        playerNo = int.Parse(gettingSeatNo.Substring(gettingSeatNo.Length - 1));
        this.gameObject.name = string.Concat("stickyP", playerNo.ToString(), sessionAspect);

    }

    
}
