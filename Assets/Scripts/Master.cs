using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Master : SingletonMonoBehaviour<Master>
{
    [SerializeField] UnityAction[] stepActions;
    [SerializeField] SimulationElement[] simulationElements; // ログ出力用
    List<uint> stepTimes;
    uint masterClock = 0;
    public static uint MasterClock{
        get { return Instance.masterClock; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stepTimes = new List<uint>();
    }

    public static void EnqueueTime(uint time)
    {
        Instance.stepTimes.Add(time);
    }

    void SortTimes()
    {
        stepTimes.Sort((a, b) => a.CompareTo(b));
    }

    // Update is called once per frame
    public static void NextStep()
    {
        Instance._NextStep();
    }

    void _NextStep()
    {
        if (stepActions.Length == 0) return;
        if (stepTimes.Count == 0) return;
        if (stepActions.Length <= stepTimes.Count) return;

        SortTimes();

        masterClock = stepTimes[0];

        stepActions[stepTimes.Count].Invoke();
        stepTimes.RemoveAt(0);
    }

    public static void Log(){
        string buf = "";
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
}
