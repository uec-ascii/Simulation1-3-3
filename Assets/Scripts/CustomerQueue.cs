using System.Collections.Generic;
using UnityEngine;

public class CustomerQueue : SimulationElement
{
    List<GameObject> customers = new List<GameObject>();
    [SerializeField] int maxQueueSize = -1; // -1は無限

    public bool EnqueueCustomer(GameObject customer)
    {
        if (maxQueueSize != -1 && customers.Count >= maxQueueSize) return false;
        customers.Add(customer);
        return true;
    }

    public GameObject DequeueCustomer()
    {
        if (customers.Count == 0) return null;
        GameObject customer = customers[0];
        customers.RemoveAt(0);
        return customer;
    }
}
