using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootMgr : MonoBehaviour
{

    // public static GameObject footLeftObj;
    public static Foot footLeft;
    // public static GameObject footRightObj;
    public static Foot footRight;
    // Start is called before the first frame update
    void Start()
    {
        // footLeft = footLeftObj.GetComponentInChildren<Foot>();
        // footRight = footRightObj.GetComponentInChildren<Foot>();
    }

    public static void Move(Json json){
        Debug.Log("FootMgr Move call");
        float left = json.positionLeft - 5.0f;
        float right = json.positionRight - 5.0f;
        float width = PlayMgr.width;
        footLeft.setX(128.0f * (left  - 2.0f - 0.5f) * width / 1024.0f);
        footRight.setX(128.0f * (right  - 2.0f - 0.5f) * width / 1024.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
