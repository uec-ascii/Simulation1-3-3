using UnityEngine;

public class PrefabSpawner : SimulationElement
{
    [SerializeField] GameObject customerPrefab; // Prefab for the customer object
    [SerializeField] CustomerQueue customerQueue; // Reference to the customer queue
    [SerializeField] float nextATClock = 0,atClock = 0; // Time to spawn the customer

    void Start()
    {
        if (nextATClock > 0)
        {
            nextATClock = RandomDist.Exponential(nextATClock); // Initialize the spawn clock
            Master.RegisterSimulationEventTime(nextATClock); // Register the initial spawn time
        }
        else
        {
            nextATClock = 0; // If no initial time is set, default to 0
        }
    }

    public void SpawnCustomer()
    {
        if (Master.MasterClock < nextATClock) // Check if the current time is greater than or equal to the spawn clock
        {
            return;
        }

        Master.Updated();

        nextATClock = Master.MasterClock + RandomDist.Exponential(atClock); // Update the spawn clock for the next customer
        Master.RegisterSimulationEventTime(nextATClock); // Register the next spawn time
        GameObject customer = Instantiate(customerPrefab, transform.position, transform.rotation); // Instantiate a new customer object
        customerQueue.EnqueueCustomer(customer); // Enqueue the customer into the queue
        customer.SetActive(true); // Activate the customer object
    }

    public override (string, string)[] GetElementInfo()
    {
        return new (string, string)[] { ("AT", nextATClock.ToString("F2")) };
    }
}
