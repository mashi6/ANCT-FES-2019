using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Judge : Token
{
    public Text text;
    public GameObject canvas;
    static GameObject _prefab = null;
    private Color color;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        canvas = GameObject.Find("Canvas");
        this.transform.SetParent(canvas.transform);
        
        ScaleX = 1;
        ScaleY = 1;
        text.fontStyle = FontStyle.Bold;

        float dir = 90;
        float spd = 0.5f;//運動速度
        SetVelocity(dir, spd);//運動開始

        while (text.color.a > 0.01f){
            // Debug.Log(text.color);
            yield return new WaitForSeconds(0.01f);
            text.color = new Color ( this.color.r, this.color.g, this.color.b, text.color.a*0.92f);
        }
        // 消滅
        DestroyObj();

    }

    public static Judge Add(float x,float y){
        _prefab = GetPrefab(_prefab, "Judge");
        return CreateInstance2<Judge>(_prefab, x, y);
    }

    public void setText(string text){
        this.text.text = text;
        switch(text){
            case "excellent":
                this.text.color = Color.magenta;
                this.color = Color.magenta;
                break;
            case "great":
                this.text.color = Color.red;
                this.color = Color.red;
                break;
            case "good":
                this.text.color = new Color(0.5f,0,0,1);
                this.color = new Color(0.5f,0,0,1);
                break;
            case "bad":
                this.text.color = Color.black;
                this.color = Color.black;
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
