using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Master : SingletonMonoBehaviour<Master>
{
    [SerializeField] UnityEvent[] stepActions;
    [SerializeField] SimulationElement[] simulationElements; // ログ出力用
    uint masterClock = 0;
    public static uint MasterClock{
        get { return Instance.masterClock; }
    }

    bool updated = false;
    int lastidx = 0;

    public UnityAction CustomerMoveFuncs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] bool verbose = false; // Set to true to enable verbose logging
    protected override void Awake()
    {
        base.Awake();
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

        while(!updated){
            if(lastidx >= stepActions.Length) {
                lastidx = 0;
                masterClock += 10;
            }
            if (stepActions[lastidx] == null) continue;
            stepActions[lastidx++].Invoke();
            if(updated){
                updated = false;
                Log();
                MoveCustomers();
                return;
            }
            if(verbose){
                Debug.Log($"Executing action on object: {stepActions[lastidx].GetPersistentTarget(0)}, method: {stepActions[lastidx].GetPersistentMethodName(0)}");
                Log();
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
