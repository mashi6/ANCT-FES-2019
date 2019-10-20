using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Notes : Token{
    static GameObject _prefab = null;
    private NotesType type;
    private float length;
    private int id;

    public static Notes Add(float x,float y,string type){
        _prefab = GetPrefab(_prefab, type);
        return CreateInstance2<Notes>(_prefab, x, y);
    }

    public void setSpeed(float speed){
        SetVelocity(270,speed);
    }
    public void setId(int id){
        this.id = id;
    }
    public void setType(NotesType type){
        this.type = type;
    }
    public void setLength(float length){
        this.length = length;
        ScaleY = 0.5528f * length / 1300.0f;

    }

    void Start(){

    }
    
    void Update(){

    }

    public void destroy(){//無いとエラー吐く
        DestroyObj();
    }

}
