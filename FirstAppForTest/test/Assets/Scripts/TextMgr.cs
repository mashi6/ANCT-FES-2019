using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMgr : MonoBehaviour
{
    public static Combo combo;
    public static Score score;

    public static void Show(Json json){
        (float x,float y) point = NotesMgr.GetPoint(json.id);
        Judge judge = Judge.Add(point.x,point.y);
        judge.setText(json.judge);
        combo.setText(json.combo.ToString());
        score.setText(json.score.ToString());
    }
    void Start()
    {

    }

    void Update()
    {
        
    }

    public static void clear(){
        combo.setText("0");
        score.setText("0");
    }
}
