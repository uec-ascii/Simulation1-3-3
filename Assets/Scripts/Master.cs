using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Master : SingletonMonoBehaviour<Master>
{
    [SerializeField] UnityEvent[] stepActions;
    [SerializeField] SimulationElement[] simulationElements; // ログ出力用
    List<uint> stepTimes;
    uint masterClock = 0;
    public static uint MasterClock{
        get { return Instance.masterClock; }
    }

    bool updated = false;
    int lastidx = 0;
    uint lastClock = 0;

    public UnityAction CustomerMoveFuncs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] bool verbose = false; // Set to true to enable verbose logging
    protected override void Awake()
    {
        base.Awake();
        stepTimes = new List<uint>();
    }

    public static void EnqueueTime(uint time)
    {
        Instance.stepTimes.Add(time);
    }

    void SortTimes()
    {
        stepTimes.Sort((a, b) => a.CompareTo(b));
        if(verbose){
            Debug.Log($"Sorting step times: {string.Join(", ", stepTimes)}");
        }
    }

    // Update is called once per frame
    public static void NextStep()
    {
        Instance._NextStep();
    }

    [ContextMenu("Next Step")]
    public void _NextStep()
    {
        if (stepActions.Length == 0) return;
        SortTimes();

        while(stepTimes.Count > 0){
            if(lastClock == stepTimes[0] && lastidx == 0){
                stepTimes.RemoveAt(0);
            }else{
                break;
            }
        }

        bool stepWithConstant = false;
        if(stepTimes.Count == 0) {
            stepWithConstant = true;
            masterClock += 10;
        }else{
            masterClock = stepTimes[0];
        }
        // クロックの処理が途中で終わってたら最後までやりきる
        if(lastClock != masterClock && lastidx > 0){
            masterClock = lastClock;
            stepWithConstant = false;
            stepTimes.Insert(0, masterClock);
        }
        while(!updated){
            lastClock = masterClock;
            for (int i = lastidx; i < stepActions.Length; i++)
            {
                if (stepActions[i] == null) continue;
                stepActions[i].Invoke();
                if(updated){
                    lastidx = i+1;
                    updated = false;
                    stepTimes.RemoveAt(0);
                    Log();
                    MoveCustomers();
                    return;
                }
                if(verbose){
                    Debug.Log($"Executing action on object: {stepActions[i].GetPersistentTarget(0)}, method: {stepActions[i].GetPersistentMethodName(0)}");
                    Log();
                }
            }
            lastidx = 0;

            if(!stepWithConstant) {
                stepTimes.RemoveAt(0);
            }
            if(stepTimes.Count == 0) {
                stepWithConstant = true;
                masterClock += 10;
            }else{
                stepWithConstant = false;
                masterClock = stepTimes[0];
            }
        }
    }

    public static void Log(){
        string buf = $"MC:{MasterClock}  ";
        foreach (var element in Instance.simulationElements)
        {
            var info = element.GetElementInfo();
            foreach (var item in info)
            {
                buf += item.Item1 + ": " + item.Item2 + " ";
            }
            buf += "  ";
        }
        Debug.Log(buf);
    }

    public static void Updated(){
        Instance.updated = true;
    }

    void MoveCustomers(){
        if (CustomerMoveFuncs == null) return;
        CustomerMoveFuncs.Invoke();
    }
}
