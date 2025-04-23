using UnityEngine;

public class SimulationElement : MonoBehaviour
{
    public virtual (string, string)[] GetElementInfo()
    {
        return new (string, string)[] { };
    }
}