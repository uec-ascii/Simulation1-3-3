using UnityEngine;
using System.Collections.Generic;

public class CustomerPool : MonoBehaviour{
    [SerializeField] GameObject customerPrefab; // Prefab for the customer object
    Queue<Customer> customers; // Array to hold the customer objects

    void Awake(){
        customers = new Queue<GameObject>(); // Initialize the queue
    }

    public GameObject GetCustomer(){
        if(customers.Count > 0){ // If there are available customers in the pool
            GameObject customer = customers.Dequeue(); // Dequeue a customer from the pool
            customer.SetActive(true); // Activate the customer object
            return customer; // Return the customer object
        }else{ // If no customers are available in the pool
            GameObject customer = Instantiate(customerPrefab); // Instantiate a new customer object
            return customer; // Return the new customer object
        }
    }
}