using System.Collections.Generic;
using UnityEngine;

public class CustomerQueue : SimulationElement
{
    List<GameObject> customers = new List<GameObject>();
    [SerializeField] int maxQueueSize = -1; // -1は無限
    [SerializeField] float startXPos= 0; // X座標の初期値

    public bool EnqueueCustomer(GameObject customer)
    {
        if (maxQueueSize != -1 && customers.Count >= maxQueueSize) return false;
        customers.Add(customer);
        UpdatePositions();
        return true;
    }

    public bool Enqueueable(){
        return maxQueueSize == -1 || customers.Count < maxQueueSize;
    }

    public GameObject DequeueCustomer()
    {
        if (customers.Count == 0) return null;
        GameObject customer = customers[0];
        customers.RemoveAt(0);
        UpdatePositions();
        return customer;
    }

    float GetXPos(int index)
    {
        return startXPos - index + transform.position.x;
    }

    void UpdatePositions()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            customers[i].GetComponent<Customer>().targetPosition = new Vector3(GetXPos(i), transform.position.y, transform.position.z);
            // Debug.Log($"Customer {customers[i].name} position updated to {customers[i].GetComponent<Customer>().targetPosition}");
        }
    }

    public int GetQueueSize()
    {
        return customers.Count;
    }

    public override (string, string)[] GetElementInfo()
    {
        return new (string, string)[] { ("#Cust", customers.Count.ToString()) };
    }
}
