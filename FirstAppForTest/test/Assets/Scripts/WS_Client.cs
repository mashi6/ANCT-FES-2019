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
    WebSocket ws_notes;
    WebSocket ws_foot;
    void Start(){

        ws_notes = new WebSocket("ws://127.0.0.1:3000/notes");//初期化

        ws_notes.OnOpen += (sender, e) => {//接続確立した時に呼び出される        
            Debug.Log("ws_notes open");
        };
        ws_notes.OnMessage += (sender, e) => {//データ受け取った時に呼び出される
            Debug.Log("ws_notes receive message: " + e.Data);

            PlayMgr.context.Post(__ =>{
                Json json = JsonUtility.FromJson<Json>(e.Data);
                switch(json.command){
                    case "newFoot":
                        NotesMgr.Schedule("Rectangle",json);        
                        break;
                    case "new":
                        NotesMgr.Schedule("Circle",json);
                        break;
                    case "judge":
                        TextMgr.Show(json);
                        if(json.delete){
                            NotesMgr.Remove(json);
                        }
                        break;
                    case "start":
                        PlayMgr.setStart(json.time);
                        break;
                    case "end":
                        PlayMgr.clear();
                        break;
                    default:
                        break;
                }
            },null);
        };
        ws_notes.OnError += (sender, e) => {//エラー発生で呼び出される
            Debug.Log("ws_notes error message: " + e.Message);
        };
        ws_notes.OnClose += (sender, e) => {//コネクション切断で呼び出される
            Debug.Log("ws_notes close");
            // reset();
            reConnect_notes();
        };
        ws_notes.Connect(); //WS開始



        ws_foot = new WebSocket("ws://127.0.0.1:3000/foot");
        ws_foot.OnOpen += (sender, e) => {//接続確立した時に呼び出される        
            Debug.Log("ws_foot open");
        };
        ws_notes.OnMessage += (sender, e) => {//データ受け取った時に呼び出される
            Debug.Log("ws_foot receive message: " + e.Data);
            PlayMgr.context.Post(__ =>{
                Json json = JsonUtility.FromJson<Json>(e.Data);
                switch(json.command){
                    case "foot":
                        FootMgr.Move(json);
                        break;
                    case "end":
                        break;
                    default:
                        break;
                }
            },null);
        };
        ws_foot.OnError += (sender, e) => {//エラー発生で呼び出される
            Debug.Log("ws_foot error message: " + e.Message);
        };
        ws_foot.OnClose += (sender, e) => {//コネクション切断で呼び出される
            Debug.Log("ws_foot close");
            reConnect_foot();
        };
        ws_foot.Connect();
    }
    
    async void reConnect_notes(){
        await Task.Delay(3000);
        ws_notes.Connect();
    }
    async void reConnect_foot(){
        await Task.Delay(3000);
        ws_foot.Connect();
    }

    void Update(){

    }
   
    void OnDestroy(){
        ws_notes.Close();
        ws_foot.Close();
        ws_notes = null;
        ws_foot = null;
    }
}
