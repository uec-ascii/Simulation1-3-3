using UnityEngine;
using TMPro;

public class StatusText : MonoBehaviour
{
    [SerializeField] Server server;
    TextMeshProUGUI text;
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        Master.Instance.CustomerMoveFuncs += ShowText;
    }

    public void ShowText()
    {
        text.text = server.Status.ToString();
    }
}
