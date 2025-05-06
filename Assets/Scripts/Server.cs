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

    // 顧客の処理が終わったら呼び出される
    public void ServiceCompletion()
    {
        // idle, down中は処理できない
        if(status == StatusType.idle || status == StatusType.down) return;
        if(nextQueue != null){
            // blocked継続処理
            if(!nextQueue.Enqueueable() && status == StatusType.blocked) return;
        }
        if (prevQueue.GetQueueSize() == 0) return;
        if(Master.MasterClock < nextDTClock || (nextDTClock == 0 && status != StatusType.blocked)) return;
        Master.Updated();
        // 次のキューが空の場合はオブジェクトを破棄
        if(nextQueue == null){
            GameObject customerObj = prevQueue.DequeueCustomer();
            customerObj.GetComponent<Customer>().OnDestroy();
            Destroy(customerObj);
            if (prevQueue.GetQueueSize() > 0)
            {
                nextDTClock = Master.MasterClock + dtClock;
                SetStatus(StatusType.busy);
            }
            else
            {
                // デバッグ表示で無駄な数字を出さないため、0にセット（出力時に空白になる）
                nextDTClock = 0;
                SetStatus(StatusType.idle);
            }
            return;
        }
        else if(nextQueue.Enqueueable())
        {
            // 次のキューが空でない場合は、次のDTをセットする
            nextQueue.EnqueueCustomer(prevQueue.DequeueCustomer());
            if (prevQueue.GetQueueSize() > 0)
            {
                nextDTClock = Master.MasterClock + dtClock;
                SetStatus(StatusType.busy);
            }
            else
            {
                SetStatus(StatusType.idle);
            }
        }else{
            // 次のキューが満杯の場合は、次のDTを次のサーバーに応じてセットする
            if(nextServer!=null) {
                nextDTClock = 0;
            }
            SetStatus(StatusType.blocked);
        }
    }

    // サーバーのダウン処理
    public void ServerBreak()
    {
        if (Master.MasterClock < nextBRClock || nextBRClock == 0) return;
        Master.Updated();
        if(nextDTClock > nextBRClock) {
            nextDTClock += opClock;
        }
        nextBRClock = 0;
        nextOPClock = Master.MasterClock + opClock;
        SetStatus(StatusType.down);
    }

    // サーバー復旧処理
    public void ServerBecomesOperational()
    {
        if (Master.MasterClock < nextOPClock || nextOPClock == 0) return;
        Master.Updated();
        nextOPClock = 0;
        nextBRClock = Master.MasterClock + brClock;
        if(nextDTClock > Master.MasterClock){
            SetStatus(StatusType.busy);
        }else{
            if (prevQueue.GetQueueSize() > 0)
            {
                nextDTClock = Master.MasterClock + dtClock;
                SetStatus(StatusType.busy);
            }
            else
            {
                SetStatus(StatusType.idle);
            }
        }
    }

    // サーバーの状態を色とともにセットする
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

    // 新規顧客が来ているか確認し、もしいてかつ空いている場合は次のDTをセットする
    public void CheckNewCustomer(){
        if(status == StatusType.down) return;
        if (prevQueue.GetQueueSize() > 0 && status == StatusType.idle)
        {
            nextDTClock = Master.MasterClock + dtClock;
            Master.Updated();
            SetStatus(StatusType.busy);
        }
    }

    // プリントアウトする際、0の場合は空白を出力する。ただし文字数は固定し、左側に空白を出力する
    string PrintClock(uint clock, uint length = 3)
    {
        string str = clock.ToString();
        if (clock == 0) str = "";

        for (int i = str.Length; i < length; i++)
        {
            str = " " + str;
        }

        return str;
    }

    public override (string, string)[] GetElementInfo()
    {
        return new (string, string)[] { ("DT", PrintClock(nextDTClock)), ("BR", PrintClock(nextBRClock)), ("OP", PrintClock(nextOPClock)), ("Status", status.ToString()) };
    }
}
