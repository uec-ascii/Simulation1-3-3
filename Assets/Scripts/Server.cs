using UnityEngine;

public class Server : SimulationElement
{
    public enum StatusType { idle, busy, down, blocked }
    [SerializeField] CustomerQueue prevQueue, nextQueue;
    [SerializeField] Server nextServer;
    public uint nextDTClock = 0, nextBRClock = 0, nextOPClock = 0;
    public uint dtClock = 0, brClock = 0, opClock = 0;
    StatusType status = StatusType.idle;
    [SerializeField] Renderer render;
    [SerializeField] Color activatedColor, downColor, blockedColor;
    public StatusType Status
    {
        get { return status; }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Master.EnqueueTime(nextDTClock);
        Master.EnqueueTime(nextBRClock);
        Master.EnqueueTime(nextOPClock);
    }

    public void ServiceCompletion()
    {
        // idle処理
        if(status == StatusType.idle) return;
        if(nextQueue != null){
            // blocked継続処理
            if(!nextQueue.Enqueueable() && status == StatusType.blocked) return;
        }
        if (prevQueue.GetQueueSize() == 0) return;
        if(Master.MasterClock < nextDTClock) return;
        Master.Updated();
        // 次のキューが空の場合はオブジェクトを破棄
        if(nextQueue == null){
            GameObject customerObj = prevQueue.DequeueCustomer();
            customerObj.GetComponent<Customer>().OnDestroy();
            Destroy(customerObj);
            if (prevQueue.GetQueueSize() > 0)
            {
                nextDTClock = Master.MasterClock + dtClock;
                Master.EnqueueTime(nextDTClock);
                SetStatus(StatusType.busy);
            }
            else
            {
                SetStatus(StatusType.idle);
            }
            return;
        }
        else if(nextQueue.Enqueueable())
        {
            nextQueue.EnqueueCustomer(prevQueue.DequeueCustomer());
            if (prevQueue.GetQueueSize() > 0)
            {
                nextDTClock = Master.MasterClock + dtClock;
                Master.EnqueueTime(nextDTClock);
                SetStatus(StatusType.busy);
            }
            else
            {
                SetStatus(StatusType.idle);
            }
        }else{
            if(nextServer!=null) {
                nextDTClock = nextServer.nextDTClock;
                Master.EnqueueTime(nextDTClock);
            }
            SetStatus(StatusType.blocked);
        }

        
    }

    public void ServerBreak()
    {
        if (Master.MasterClock != nextBRClock) return;
        Master.Updated();
        if(nextDTClock > nextBRClock) {
            nextDTClock += opClock;
            Master.EnqueueTime(nextDTClock);
        }
        nextBRClock = Master.MasterClock + opClock + brClock;
        Master.EnqueueTime(nextBRClock);
        nextOPClock = Master.MasterClock + opClock;
        Master.EnqueueTime(nextOPClock);
        SetStatus(StatusType.down);
    }

    public void ServerBecomesOperational()
    {
        if (prevQueue.GetQueueSize()<=0) return;
        if (Master.MasterClock != nextOPClock) return;
        Master.Updated();
        if(nextDTClock > Master.MasterClock){
            SetStatus(StatusType.busy);
        }else{
            if (prevQueue.GetQueueSize() > 0)
            {
                nextDTClock = Master.MasterClock + dtClock;
                Master.EnqueueTime(nextDTClock);
                SetStatus(StatusType.busy);
            }
            else
            {
                SetStatus(StatusType.idle);
            }
        }
    }

    void SetStatus(StatusType newStatus)
    {
        status = newStatus;
        switch (status)
        {
            case StatusType.idle:
                render.material.color = activatedColor;
                break;
            case StatusType.busy:
                render.material.color = activatedColor;
                break;
            case StatusType.down:
                render.material.color = downColor;
                break;
            case StatusType.blocked:
                render.material.color = blockedColor;
                break;
        }
    }

    public void CheckNewCustomer(){
        if(status == StatusType.down) return;
        if (prevQueue.GetQueueSize() > 0 && status == StatusType.idle)
        {
            nextDTClock = Master.MasterClock + dtClock;
            Master.EnqueueTime(nextDTClock);
            Master.Updated();
            SetStatus(StatusType.busy);
        }
    }

    public override (string, string)[] GetElementInfo()
    {
        return new (string, string)[] { ("DT", nextDTClock.ToString()), ("BR", nextBRClock.ToString()), ("OP", nextOPClock.ToString()), ("Status", status.ToString()) };
    }
}
