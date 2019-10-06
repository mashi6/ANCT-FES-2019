using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Notes : Token
{
    static GameObject _prefab = null;
    Vector2 min;
    Vector2 max;
    public static Notes Add(float x,float y)
    {
        _prefab = GetPrefab(_prefab, "notes");
        return CreateInstance2<Notes>(_prefab, x, y);
    }

    void Start()
    {
        min = GetWorldMin();//画面左下座標取得
        max = GetWorldMax();//画面右上座標取得
        
        float dir = 270;//運動方向
        float spd = 3;//運動速度
        SetVelocity(dir, spd);//運動開始
    }

    
    void Update()
    {
        if(Y < min.y)//ノーツが下端に達したら
        {
            DestroyObj();//ノーツ破壊
            int index = (int)(X * 1024.0 / 128.0 / (max.x - min.x) + 2.5);//X座標からノーツのレーン計算
            Debug.Log(index);
            WS_Client.removeListAt(Math.Max(index-1,0));//noteslistの調整
            
        }
    }

    public void destroy()//無いとエラー吐く
    {
        DestroyObj();
    }

}
