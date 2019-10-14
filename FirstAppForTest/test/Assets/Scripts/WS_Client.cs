using System;
using System.Threading;
using System.Threading.Tasks;
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
    public GameObject footLeftObj;
    private Foot footLeft;
    public GameObject footRightObj;
    private Foot footRight;
    
    static List<List<Notes>> notesList;//各レーンごとのノーツ情報を格納する二次元配列
    static List<List<FootNotes>> footNotesList;

    private SynchronizationContext context;

    private Vector2 min,max;
    void Start(){
        footLeft = footLeftObj.GetComponentInChildren<Foot>();
        footRight = footRightObj.GetComponentInChildren<Foot>();

        context = SynchronizationContext.Current;//メインスレッド呼ぶ時に使うやつ
        
        notesList = new List<List<Notes>>();//初期化
        notesList.Add(new List<Notes>());
        notesList.Add(new List<Notes>());
        notesList.Add(new List<Notes>());
        notesList.Add(new List<Notes>());

        footNotesList = new List<List<FootNotes>>();
        footNotesList.Add(new List<FootNotes>());
        footNotesList.Add(new List<FootNotes>());
        footNotesList.Add(new List<FootNotes>());
        footNotesList.Add(new List<FootNotes>());
        
        min = GetWorldMin();//画面左下座標取得
        max = GetWorldMax();//画面右上座標取得

        ws = new WebSocket("ws://127.0.0.1:3000/");//初期化

        ws.OnOpen += (sender, e) => {//接続確立した時に呼び出される        
            Debug.Log("WebSocket Open");
        };
        ws.OnMessage += (sender, e) => {//データ受け取った時に呼び出される
            Debug.Log("WebSocket Receive Message: " + e.Data);

            string[] arr = e.Data.Split(':');//受け取ったデータの整理
            string code = arr[0];
            switch(code){
                case "generate":
                generate(arr);
                    break;
                case "remove":
                    remove(arr);
                    break;
                case "removeFoot":
                    removeFoot(arr);
                    break;
                case "reset":
                    reset();
                    break;
                case "move":
                    move(arr);
                    break;
                case "judgeFoot":
                    judgeFoot(arr);
                    break;
            }
        };
        ws.OnError += (sender, e) => {//エラー発生で呼び出される
            Debug.Log("WebSocket Error Message: " + e.Message);
        };
        ws.OnClose += (sender, e) => {//コネクション切断で呼び出される
            Debug.Log("WebSocket Close");
            reset();
            reConnect();
        };
        ws.Connect(); //WS開始
    }
    async void reConnect(){
        await Task.Delay(3000);
        ws.Connect();
    }

    public void generate(string[] arr){
        int lane = int.Parse(arr[1]);
        string notesType = arr[2];
        if(notesType == "Circle"){ //丸ノーツ
            context.Post(__ => {
                notesList[lane-1].Add(Notes.Add((float)(128 * (lane - 2 - 0.5) * (max.x - min.x) / 1024.0), (float)((max.y - min.y) / 2.0))); //座標計算してノーツ生成、配列に格納
            }, null);
        }else if(notesType == "Rectangle"){ //足ノーツ
            float length = float.Parse(arr[3]);
            context.Post(__ => {
                FootNotes footNotes = FootNotes.Add((float)(128 * (lane - 2 - 0.5) * (max.x - min.x) / 1024.0), (float)((max.y - min.y) / 2.0));
                footNotesList[lane-1].Add(footNotes);
                footNotes.setLength(length);
            }, null);
        }
    }
    public void remove(string[] arr){
        int lane = int.Parse(arr[1]) - 1; //ノーツのレーン
        string judgeText = arr[2];
        string comboText = arr[3];
        string scoreText = arr[4];
        context.Post(__ => {//ここからメインスレッド---
            if(notesList[lane].Count > 0){
                float y = notesList[lane][0].Y;
                notesList[lane][0].destroy(); //ノーツオブジェクトの破壊
                notesList[lane].RemoveAt(0); //配列内のノーツ削除
                Judge judge = Judge.Add(0.026f*(-75 + lane * 50),y);
                judge.setText(judgeText);
                combo.setText(comboText);
                score.setText(scoreText);
            }
        },null); //ここまでメインスレッド------------
        combo.textAnimation();
    }
    public void removeFoot(string[] arr){
        int lane = int.Parse(arr[1]) - 1; //ノーツのレーン
        string judgeText = arr[2];
        string comboText = arr[3];
        string scoreText = arr[4];
        context.Post(__ => {//ここからメインスレッド---
            if(footNotesList[lane].Count > 0){
                float y = footNotesList[lane][0].Y;
                footNotesList[lane][0].destroy(); //ノーツオブジェクトの破壊
                footNotesList[lane].RemoveAt(0); //配列内のノーツ削除
                Judge judge = Judge.Add(0.026f*(-75 + lane * 50),y);
                judge.setText(judgeText);
                combo.setText(comboText);
                score.setText(scoreText);
            }
            
        },null); //ここまでメインスレッド------------
        combo.textAnimation();
    }
    public void move(string[] arr){
        float left = float.Parse(arr[1]) - 5.0f;//0~2
        float right = float.Parse(arr[2]) - 5.0f;//3~5
        context.Post(__ => {
            footLeft.setX(128.0f * (left  - 2.0f - 0.5f) * (max.x - min.x) / 1024.0f);
            footRight.setX(128.0f * (right  - 2.0f - 0.5f) * (max.x - min.x) / 1024.0f);
        },null);
    }
    public void judgeFoot(string[] arr){
        int lane = int.Parse(arr[1]) - 1;
        string judgeText = arr[2];
        string comboText = arr[3];
        string scoreText = arr[4];
        context.Post(__ => {
            float y = footLeft.Y;
            Judge judge = Judge.Add(0.026f*(-75 + lane * 50),y);
            judge.setText(judgeText);
            combo.setText(comboText);
            score.setText(scoreText);
        },null);
    }
    public void reset(){
        context.Post(__ => {
            foreach (List<Notes> notes in notesList){
                foreach(Notes note in notes){
                    note.destroy();
                }
            }
            foreach (List<FootNotes> notes in footNotesList){
                foreach(FootNotes note in notes){
                    note.destroy();
                }
            }
            notesList[0].Clear();
            notesList[1].Clear();
            notesList[2].Clear();
            notesList[3].Clear();
            footNotesList[0].Clear();
            footNotesList[1].Clear();
            footNotesList[2].Clear();
            footNotesList[3].Clear();
        
            combo.setText("0");
            score.setText("0");
        },null);
    }

    // Update is called once per frame
    void Update(){

    }
    
    public static void removeListAt(int index){ //ノーツ配列の index番目のレーンから１つ削除
        notesList[index].RemoveAt(0);
    }
    public static void removeFootListAt(int index){
        footNotesList[index].RemoveAt(0);
    }
   
    void OnDestroy(){
        ws.Close();
        ws = null;
    }
}
