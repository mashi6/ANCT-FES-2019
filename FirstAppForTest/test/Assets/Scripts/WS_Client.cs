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
    public Combo combo;
    public Score score;
    public GameObject FootLeft;
    private Foot footLeft;
    public GameObject FootRight;
    private Foot footRight;
    
    static List<List<Notes>> noteslist;//各レーンごとのノーツ情報を格納する二次元配列
    static List<List<FootNotes>> footnoteslist;
    void Start(){
        footLeft = FootLeft.GetComponentInChildren<Foot>();
        footRight = FootRight.GetComponentInChildren<Foot>();

        // GameObject comboObj = GameObject.Find("Combo");
        // combo = comboObj.GetComponents<Combo>()[0];
        
        // GameObject _parent = comboObj.transform.root.gameObject;
        // Debug.Log ("Parent:" + _parent.name);
        // canvasMgr.GetComponents<CanvasMgr>()[0].addScore(0,0);
        
        // combo.changeText("comboo");

        var context = SynchronizationContext.Current;//メインスレッド呼ぶ時に使うやつ
        noteslist = new List<List<Notes>>();//初期化
        noteslist.Add(new List<Notes>());
        noteslist.Add(new List<Notes>());
        noteslist.Add(new List<Notes>());
        noteslist.Add(new List<Notes>());

        footnoteslist = new List<List<FootNotes>>();
        footnoteslist.Add(new List<FootNotes>());
        footnoteslist.Add(new List<FootNotes>());
        footnoteslist.Add(new List<FootNotes>());
        footnoteslist.Add(new List<FootNotes>());
        

        Vector2 min = GetWorldMin();//画面左下座標取得
        Vector2 max = GetWorldMax();//画面右上座標取得

        ws = new WebSocket("ws://127.0.0.1:3000/");//初期化
        ws.OnOpen += (sender, e) => {//接続確立した時に呼び出される
        
            Debug.Log("WebSocket Open");
        };
        ws.OnMessage += (sender, e) => {//データ受け取った時に呼び出される
            
            Debug.Log("WebSocket Receive Message: " + e.Data);

            string[] arr = e.Data.Split(':');//受け取ったデータの整理
            string code = arr[0];
            // string message = arr[1];
            // string submessage = arr[2];//良くなさそう
            // string subsubmessage = arr[3];//良くない

            if(code == "generate"){ //ノーツ生成
                int lane = int.Parse(arr[1]); //ノーツのレーン
                string notesType = arr[2];
                if(notesType == "Circle"){
                    context.Post(__ => {//ここからメインスレッド---
                        noteslist[lane-1].Add(Notes.Add((float)(128 * (lane - 2 - 0.5) * (max.x - min.x) / 1024.0), (float)((max.y - min.y) / 2.0))); //座標計算してノーツ生成、配列に格納
                    }, null); //ここまでメインスレッド------------
                }else if(notesType == "Rectangle"){
                    float length = float.Parse(arr[3]);
                    context.Post(__ => {
                        FootNotes footNotes = FootNotes.Add((float)(128 * (lane - 2 - 0.5) * (max.x - min.x) / 1024.0), (float)((max.y - min.y) / 2.0));
                        footnoteslist[lane-1].Add(footNotes);
                        footNotes.setLength(length);
                    }, null);
                }

            }else if(code == "remove"){ //ノーツ削除
                int lane = int.Parse(arr[1]) - 1; //ノーツのレーン
                string judgeText = arr[2];
                string comboText = arr[3];
                string scoreText = arr[4];
                context.Post(__ => {//ここからメインスレッド---
                    if(noteslist[lane].Count > 0){
                        float y = noteslist[lane][0].Y;
                        noteslist[lane][0].destroy(); //ノーツオブジェクトの破壊
                        noteslist[lane].RemoveAt(0); //配列内のノーツ削除
                        Judge judge = Judge.Add(0.026f*(-75 + lane * 50),y);
                        judge.setText(judgeText);
                        combo.setText(comboText);
                        score.setText(scoreText);
                    }
                    
                },null); //ここまでメインスレッド------------
                combo.textAnimation();
            }else if(code == "reset"){//ノーツ配列リセット
            
                noteslist[0].Clear();
                noteslist[1].Clear();
                noteslist[2].Clear();
                noteslist[3].Clear();
                footnoteslist[0].Clear();
                footnoteslist[0].Clear();
                footnoteslist[0].Clear();
                footnoteslist[0].Clear();

            }else if(code == "move"){
                // Debug.Log(footLeft.name);
                float left = float.Parse(arr[1]) - 5.0f;//0~2
                float right = float.Parse(arr[2]) - 5.0f;//3~5
                context.Post(__ => {
                    
                    footLeft.setX(128.0f * (left  - 2.0f - 0.5f) * (max.x - min.x) / 1024.0f);
                    footRight.setX(128.0f * (right  - 2.0f - 0.5f) * (max.x - min.x) / 1024.0f);
                },null);
            }else if(code == "judgeFoot"){
                int lane = int.Parse(arr[1]) - 1;
                string judgeText = arr[2];
                string comboText = arr[3];
                string scoreText = arr[4];
                
                // float y = -0.026f * 260.0f;

                context.Post(__ => {
                    // float y = footnoteslist[lane][0].getY();
                    
                    float y = footLeft.Y;
                    Judge judge = Judge.Add(0.026f*(-75 + lane * 50),y);
                    judge.setText(judgeText);
                    combo.setText(comboText);
                    score.setText(scoreText);
                },null);
                
            }
        };
        ws.OnError += (sender, e) => {//エラー発生で呼び出される
        
            Debug.Log("WebSocket Error Message: " + e.Message);
        };
        ws.OnClose += (sender, e) => {//コネクション切断で呼び出される
        
            Debug.Log("WebSocket Close");
        };

        ws.Connect(); //WS開始
    }

    // Update is called once per frame
    void Update(){

    }
    
    public static void removeListAt(int index){ //ノーツ配列の index番目のレーンから１つ削除
        noteslist[index].RemoveAt(0);
    }
    public static void removeFootListAt(int index){
        footnoteslist[index].RemoveAt(0);
    }
   
    void OnDestroy(){
        ws.Close();
        ws = null;
    }
}
