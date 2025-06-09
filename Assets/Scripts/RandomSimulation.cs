using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RandomSimulation : MonoBehaviour
{
    [SerializeField] const int MAX_LOG_LENGTH = 100;
    List<float> variables;
    [SerializeField] TextMeshProUGUI logText, populationText, sampleText; // UIに表示するためのTextMeshProUGUIコンポーネント
    float mu;
    [SerializeField] TMP_InputField muInputField, countInputField;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        variables = new List<float>();
    }

    void UpdatePopulationText()
    {
        string populationTextStr = "Population\n";
        populationTextStr += $"E: {mu:F2}\n";
        populationTextStr += $"V: {mu * mu:F2}";
        populationText.text = populationTextStr;
    }
    void UpdateSampleText()
    {
        float mean, variance;
        string sampleTextStr = "Sample\n";
        if (variables.Count > 0)
        {
            mean = 0;
            variance = 0;
            foreach (var value in variables)
            {
                mean += value;
            }
            mean /= variables.Count;

            foreach (var value in variables)
            {
                variance += (value - mean) * (value - mean);
            }
            variance /= variables.Count;
            sampleTextStr += $"Mean: {mean:F2}\nVar: {variance:F2}\nCount: {variables.Count}";
        }

        sampleText.text = sampleTextStr;
    }

    void UpdateLogText()
    {
        // 後ろから出力していき、最大長を超えたら表示を更新
        string logStr = "";
        for (int i = variables.Count - 1; i >= 0 && logStr.Length < MAX_LOG_LENGTH; i--)
        {
            logStr = $"{variables[i]:F2}," + logStr;
        }
        if (logStr.Length > 0)
        {
            logStr = logStr.TrimEnd(','); // 最後のカンマを削除
        }
        logText.text = logStr;
    }

    public void Initialize()
    {
        variables.Clear();
        mu = 0;
        if (muInputField != null)
        {
            float.TryParse(muInputField.text, out mu);
        }
        UpdatePopulationText();
        UpdateSampleText();
        UpdateLogText();
    }
    
    public void AddSample()
    {
        int count = 0;
        if (countInputField != null)
        {
            int.TryParse(countInputField.text, out count);
        }
        for (int i = 0; i < count; i++)
        {
            float sample = RandomDist.Exponential(mu);
            variables.Add(sample);
        }
        UpdateSampleText();
        UpdateLogText();
    }
}
