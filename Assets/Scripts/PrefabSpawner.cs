using UnityEngine;

public class PrefabSpawner : SimulationElement
{
    [SerializeField] GameObject customerPrefab; // Prefab for the customer object
    [SerializeField] CustomerQueue customerQueue; // Reference to the customer queue
    [SerializeField] uint nextATClock = 0,atClock = 0; // Time to spawn the customer

    void Start(){
        Master.EnqueueTime(nextATClock); // Enqueue the initial spawn time
    }
    public void SpawnCustomer()
    {
        if(Master.MasterClock < nextATClock) // Check if the current time is greater than or equal to the spawn clock
        {
            return;
        }

        Master.Updated();

        nextATClock = Master.MasterClock + atClock; // Update the spawn clock for the next customer
        Master.EnqueueTime(nextATClock); // Enqueue the next spawn time

        GameObject customer = Instantiate(customerPrefab,transform.position,transform.rotation); // Instantiate a new customer object
        customerQueue.EnqueueCustomer(customer); // Enqueue the customer into the queue
        customer.SetActive(true); // Activate the customer object
    }

    public override (string, string)[] GetElementInfo()
    {
        return new (string, string)[] { ("AT", nextATClock.ToString()) };
    }
}
