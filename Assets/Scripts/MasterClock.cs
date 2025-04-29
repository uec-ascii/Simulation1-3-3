using UnityEngine;
using TMPro;

public class MasterClock : MonoBehaviour
{
    TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Master.Instance.CustomerMoveFuncs += ShowText;
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    public void ShowText()
    {
        text.text = "MasterClock: " + Master.MasterClock.ToString();
    }
}
