using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayMgr : Token
{
    public static Vector2 min,max;
    public static SynchronizationContext context;

    public static float width;
    public static float height;
    public static float laneWidth;
    public static float judgeY;
    
    public static float circleWidth;
    public static float circleHeight;
    
    public static float rectangleWidth;
    public static float rectangleHeight;
    public static float defaultSpeed;
    public static long startTime;

    void Start()
    {
        min = GetWorldMin();//画面左下座標取得
        max = GetWorldMax();//画面右上座標取得
        width = max.x - min.x;
        height = max.y - min.y;
        laneWidth = width/8.0f;
        judgeY = -height/4.0f;
        circleWidth = laneWidth;
        circleHeight = circleWidth;
        rectangleWidth = laneWidth;
        rectangleHeight = 4*rectangleWidth;
        defaultSpeed = 3.0f;

        context = SynchronizationContext.Current;
        GameObject footLeftObj = GameObject.Find("FootLeft");
        GameObject footRightObj = GameObject.Find("FootRight");
        FootMgr.footLeft = footLeftObj.GetComponentInChildren<Foot>();
        FootMgr.footRight = footRightObj.GetComponentInChildren<Foot>();

        GameObject comboObj = GameObject.Find("Combo");
        GameObject scoreObj = GameObject.Find("Score");
        TextMgr.combo = comboObj.GetComponentInChildren<Combo>();
        TextMgr.score = scoreObj.GetComponentInChildren<Score>();
    }

    public static void gameStart(){
        startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
    public static void setStart(long time){
        startTime = time;
    }

    void Update()
    {
        
    }

    public static void clear(){
        NotesMgr.clear();
        TextMgr.clear();
        QueueBundle.clear();
    }
}
