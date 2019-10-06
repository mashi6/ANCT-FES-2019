using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour
{
    void OnGUI()
    {
        
        Util.SetFontSize(128);
        Util.SetFontAlignment(TextAnchor.MiddleCenter);
        float w = 256;
        float h = 64;
        float px = Screen.width / 2 - w / 2;
        float py = Screen.height / 2 - h / 2;
        py += 120;
        if(GUI.Button(new Rect(px,py,w,h), "START"))
        {
            SceneManager.LoadScene("Select");
        }

        
    }
}
