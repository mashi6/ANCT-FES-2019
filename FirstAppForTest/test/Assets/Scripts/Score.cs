using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : Token
{
    public Text text;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("textAnimation");
    }
    public void setText(string text){
        this.text.text = text;
        this.text.fontSize += 10;
        
    }
    public IEnumerator textAnimation(){
        // Debug.Log(ScaleX);
        while(true){
            yield return new WaitForSeconds(0.01f);
            while(text.fontSize > 50){
                yield return new WaitForSeconds(0.01f);
                text.fontSize -= 1;
            }
        }   
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
