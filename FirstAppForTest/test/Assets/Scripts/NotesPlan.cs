using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesPlan
{
    public string notesType;
    public float x,y;
    public float length;
    public float speed;
    public long time;
    public int id;
    public NotesPlan(string notesType,float x,float y,float length,float speed,long time,int id){
        this.notesType = notesType;
        this.x = x;
        this.y = y;
        this.length = length;
        this.speed = speed;
        this.time = time;
        this.id = id;
    }
}
