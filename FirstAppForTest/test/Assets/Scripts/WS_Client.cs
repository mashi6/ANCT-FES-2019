using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;


public class WS_Client : Token
{
    
    WebSocket ws;
    static List<List<Notes>> noteslist;//各レーンごとのノーツ情報を格納する二次元配列
    
    void Start()
    {
        var context = SynchronizationContext.Current;//メインスレッド呼ぶ時に使うやつ
        noteslist = new List<List<Notes>>();//初期化
        noteslist.Add(new List<Notes>());
        noteslist.Add(new List<Notes>());
        noteslist.Add(new List<Notes>());
        noteslist.Add(new List<Notes>());
        

        Vector2 min = GetWorldMin();//画面左下座標取得
        Vector2 max = GetWorldMax();//画面右上座標取得

        ws = new WebSocket("ws://127.0.0.1:3000/");//初期化
        ws.OnOpen += (sender, e) => //接続確立した時に呼び出される
        {
            Debug.Log("WebSocket Open");
        };
        ws.OnMessage += (sender, e) => //データ受け取った時に呼び出される
        {
            
            Debug.Log("WebSocket Receive Message: " + e.Data);

            string[] arr = e.Data.Split(':');//受け取ったデータの整理
            string code = arr[0];
            string message = arr[1];

            if(code == "generate"){ //ノーツ生成
                int lane = int.Parse(message); //ノーツのレーン
                context.Post(__ => //ここからメインスレッド---
                {
                    noteslist[lane-1].Add(Notes.Add((float)(128 * (lane - 2 - 0.5) * (max.x - min.x) / 1024.0), (float)((max.y - min.y) / 2.0))); //座標計算してノーツ生成、配列に格納
                }, null); //ここまでメインスレッド------------

            }else if(code == "remove"){ //ノーツ削除
                int lane = int.Parse(message) - 1; //ノーツのレーン
                context.Post(__ => //ここからメインスレッド---
                {
                    noteslist[lane][0].destroy(); //ノーツオブジェクトの破壊
                    noteslist[lane].RemoveAt(0); //配列内のノーツ削除
                },null); //ここまでメインスレッド------------

            }else if(code == "reset") //ノーツ配列リセット
            {
                noteslist[0].Clear();
                noteslist[1].Clear();
                noteslist[2].Clear();
                noteslist[3].Clear();
            }
        };
        ws.OnError += (sender, e) => //エラー発生で呼び出される
        {
            Debug.Log("WebSocket Error Message: " + e.Message);
        };
        ws.OnClose += (sender, e) => //コネクション切断で呼び出される
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect(); //WS開始
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public static void removeListAt(int index) //ノーツ配列の index番目のレーンから１つ削除
    {
        noteslist[index].RemoveAt(0);
    }
   
    void OnDestroy()
    {
        ws.Close();
        ws = null;
    }
}
