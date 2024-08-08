using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineChart : MonoBehaviour
{
    public UILineRenderer lineRenderer;
    public List<float> values = new List<float>();
    public Transform target;
    public float updateRate = 0.1f;

    void Start()
    {
        if (lineRenderer == null)
            lineRenderer = GetComponent<UILineRenderer>();

        UpdateLineChart();

        InvokeRepeating("refreshGraph", 0.0f, updateRate);

    }

    void refreshGraph()
    {
        values = this.GetComponent<graphManager>().getValues();

        UpdateLineChart();
    }

    public void AddValue(float value)
    {
        values.Add(value);
        lineRenderer.GetComponent<UILineRenderer>().AddValue(value);
        lineRenderer.SetVerticesDirty();
    }

    private void UpdateLineChart()
    {
        lineRenderer.values = values;
        lineRenderer.SetVerticesDirty();
    }
}
