using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesMgr : MonoBehaviour
{

    static Dictionary<int, Notes> notesDict = new Dictionary<int, Notes>();
    static Dictionary<int, FootNotes> footNotesDict = new Dictionary<int, FootNotes>();

    void Start()
    {

    }

    void Update()
    {
        
    }

    public static void clear(){
        foreach(KeyValuePair<int,Notes> kvp in notesDict){
            kvp.Value.destroy();
        }
        foreach(KeyValuePair<int,FootNotes> kvp in footNotesDict){
            kvp.Value.destroy();
        }
        notesDict.Clear();
        footNotesDict.Clear();
    }

    public static void Schedule(string type,Json json){
        float notesHeight = 0.0f, notesWidth = 0.0f;
        float lane = json.lane;
        long time = json.time;
        float speed = PlayMgr.defaultSpeed * json.speed;
        if(type == "Circle"){
            notesWidth = PlayMgr.circleWidth;
            notesHeight = PlayMgr.circleHeight;
        }else if(type == "Rectangle"){
            notesWidth = PlayMgr.rectangleWidth;
            notesHeight = PlayMgr.rectangleHeight;
        }

        float yi = PlayMgr.max.y + 0.5f*notesHeight;
        float yj = yi - PlayMgr.judgeY;
        float yf = 2.0f*notesHeight + PlayMgr.height;
        float xi = (lane - 1.5f)*PlayMgr.laneWidth;
        long createTime = time - (long)(yj / speed * 1000.0f);
        Debug.Log(createTime);
        NotesPlan plan = new NotesPlan(notesType: type, x: xi, y: yi, length: json.length, speed: speed, time: createTime, id: json.id);

        QueueBundle.Push(plan);
    }


    public static void AddFoot(NotesPlan plan){
        PlayMgr.context.Post(__ => {
            FootNotes notes = FootNotes.Add(plan.x,plan.y,plan.notesType);
            footNotesDict.Add(plan.id,notes);
            notes.setLength(plan.length);
            notes.setId(plan.id);
            notes.setSpeed(plan.speed);
        },null);
    }
    public static void AddCircle(NotesPlan plan){
        PlayMgr.context.Post(__ => {
            Notes notes = Notes.Add(plan.x,plan.y,plan.notesType);
            notesDict.Add(plan.id,notes);
            notes.setId(plan.id);
            notes.setSpeed(plan.speed);
        },null);
    }

    public static void Remove(Json json){
        if(notesDict.ContainsKey(json.id)){
            notesDict[json.id].destroy();
            notesDict.Remove(json.id);
        }else if(footNotesDict.ContainsKey(json.id)){
            footNotesDict[json.id].destroy();
            footNotesDict.Remove(json.id);
        }
        
    }

    public static (float,float) GetPoint(int id){
        float x=0,y=0;
        if(notesDict.ContainsKey(id)){
            x = notesDict[id].X;
            y = notesDict[id].Y;
        }else if(footNotesDict.ContainsKey(id)){
            x = footNotesDict[id].X;
            y = footNotesDict[id].Y;
        }
        return (x,y);
    }
}
