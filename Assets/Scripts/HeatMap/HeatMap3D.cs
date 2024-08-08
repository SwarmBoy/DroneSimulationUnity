using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class HeatMap3D : MonoBehaviour
{   
    public float _Spacing = 0.5f;
    public float spacing
{
    get { return Mathf.Max(0.2f, _Spacing); }
    set { _Spacing = Mathf.Max(0.2f, value); } // Ensure a minimum value of 0.1f
}
    public GameObject voxelPrefab; // Assign a cube or sphere prefab

    public Transform heatmapHolder;

    public float fixedHeight = 5;
    public float radius = 15;
    public float refreshRate = 0.2f;

    public Vector3 center = new Vector3(0, 0, 0);

    
    void Start()
    {
        InvokeRepeating("UpdateHeatmap", 0.0f, refreshRate);
    }

    void UpdateHeatmap()
    {
        destroyAllChildren(heatmapHolder);
        StartCoroutine(GenerateHeatmap());
    }


    IEnumerator GenerateHeatmap()
    {
        center = this.GetComponent<CameraMovement>().center;
        //discretize within a circle
        for (float x = -radius; x <= radius; x += spacing)
        {
            for (float z = -radius; z <= radius; z += spacing)
            {
                if(x*x + z*z > radius*radius)
                {
                    continue;
                }
                Vector2 position = new Vector2(center.x + x, center.z + z);
                
                float value = CalculateValue(new Vector3(position.x, center.y, position.y));
                CreateVoxel(position.x, fixedHeight, position.y, value);
            }
        }
        yield return null;

    }

    float CalculateValue(Vector3 pos)
    {
        // Example function, replace with your own
        float value = this.GetComponent<ReynoldModel>().computeObstacleAndSeparation(pos);
        return value;
    }

    void CreateVoxel(float x, float y, float z, float value)
    {
        GameObject voxel = Instantiate(voxelPrefab, new Vector3(x, y , z ), Quaternion.identity);
        voxel.transform.parent = heatmapHolder;
        voxel.transform.localScale = new Vector3(spacing, spacing, spacing);
        voxel.GetComponent<Renderer>().material.color = Color.Lerp(Color.blue, Color.red, value); // Normalize value to [0, 1]
        voxel.GetComponent<Renderer>().material.SetFloat("_Transparency", 0.8f); // Adjust transparency
    }

    void destroyAllChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
