using UnityEngine;

public class Server : SimulationElement
{
    public enum StatusType { idle, busy, down, blocked } // Service types: DT (Drive-Thru), BR (Beverage), OP (Order Processing)
    [SerializeField] CustomerQueue prevQueue, nextQueue;
    [SerializeField] Server nextServer;
    GameObject customer;
    public uint nextDTClock = 0, nextBRClock = 0, nextOPClock = 0;
    public uint dtClock = 0, brClock = 0, opClock = 0;
    StatusType status = StatusType.idle;
    public StatusType Status
    {
        get { return status; }
    }
    [SerializeField] float customerYPos = 2f; // Y座標の初期値

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Master.EnqueueTime(nextDTClock);
        Master.EnqueueTime(nextBRClock);
        Master.EnqueueTime(nextOPClock);
    }

    public void ServiceCompletion()
    {
        if (customer == null) return;
        if(Master.MasterClock != nextDTClock) return;
        Master.Updated();
        if(nextQueue == null){
            Destroy(customer);
        }
        if(nextQueue.EnqueueCustomer(customer) == false)
        {
            status = StatusType.blocked;
            if(nextServer!=null) {
                nextDTClock = nextServer.nextDTClock;
                Master.EnqueueTime(nextDTClock);
            }
        }
        else
        {
            customer = prevQueue.DequeueCustomer();
            if (customer != null)
            {
                status = StatusType.busy;
                customer.GetComponent<Customer>().targetPosition = new Vector3(transform.position.x, transform.position.y + customerYPos, transform.position.z);
                customer.SetActive(true);
            }
            else
            {
                status = StatusType.idle;
            }
        }
    }

    public void ServerBreak()
    {
        if (customer == null) return;
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
        status = StatusType.down;
    }

    public void ServerBecomesOperational()
    {
        if (customer == null) return;
        if (Master.MasterClock != nextOPClock) return;
        Master.Updated();
        if(customer != null) {
            status = StatusType.busy;
        }else{
            customer = prevQueue.DequeueCustomer();
            if (customer != null)
            {
                customer.GetComponent<Customer>().targetPosition = new Vector3(transform.position.x, transform.position.y + customerYPos, transform.position.z);
                customer.SetActive(true);
                nextDTClock = Master.MasterClock + dtClock;
                Master.EnqueueTime(nextDTClock);
                status = StatusType.busy;
            }
            else
            {
                status = StatusType.idle;
            }
        }
    }
}
