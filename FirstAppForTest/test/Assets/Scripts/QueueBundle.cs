using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueBundle : Token
{
    static private List<List<NotesPlan>> bundle = new List<List<NotesPlan>>();
    static private int bundleWidth = 4;
    public static bool isRun = true;
    public static float updateCycle = 0.01f;

    IEnumerator Start()
    {
        for(int i=0;i<bundleWidth;i++){
            bundle.Add(new List<NotesPlan>());
        }
        while(isRun){
            yield return new WaitForSeconds(updateCycle);
            long nowTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            long elapsedTime = nowTime - PlayMgr.startTime;

            List<NotesPlan> ready = new List<NotesPlan>();
            for(int i=0;i<bundleWidth;i++){
                if(bundle[i].Count > 0){
       
                   if(Math.Abs(bundle[i][0].time - elapsedTime) < updateCycle * 1000 * 3){
                        ready.Add(bundle[i][0]);
                        bundle[i].RemoveAt(0);
                    }
                }
            }
            
            foreach(NotesPlan plan in ready){
                Debug.Log(plan.notesType);
                if(plan.notesType == "Circle"){
                    NotesMgr.AddCircle(plan);
                }else if(plan.notesType == "Rectangle"){
                    NotesMgr.AddFoot(plan);
                }
            }
        }
    }

    public static void Push(NotesPlan plan){

        int index = -1;
        long delay = 0;
        int depth = 0;
        while(true){
            for(int i=0;i<bundleWidth;i++){

                if(bundle[i].Count <= depth){
                    bundle[i].Insert(depth,plan);
                    return;
                }
                if(bundle[i][depth].time > plan.time){
                    if(delay < bundle[i][depth].time - plan.time){
                        index = i;
                        delay = bundle[i][depth].time - plan.time;
                    }
                }
            }
            if(index != -1){
                bundle[index].Insert(depth,plan);
                return;
            }
            index = -1;
            delay = 0;
            depth++;
        }
    }

    public static void clear(){
        bundle = new List<List<NotesPlan>>();
    }
}
