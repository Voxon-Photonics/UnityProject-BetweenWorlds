using System;
using UnityEngine;
using Voxon;
using System.Collections;
using System.Collections.Generic;
using DataStructures.ViliWonka.KDTree;

public class MeshReader : MonoBehaviour
{
    public float delayBeforeActivation = 3.0f; // Seconds
    public float radius = 400;
    public new GameObject camera;
    private bool added = false;
    
    private Vector3[] pointCloud;
    private MeshFilter[] meshFilters;
    
    int maxPointsPerLeafNode = 32;
    KDTree tree;
    
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Initialise", delayBeforeActivation);
    }

    void Initialise() // Requires all statics to be active at this point
    {
        Debug.Log("Building Point Cloud");
        added = true;

        meshFilters = FindObjectsOfType<MeshFilter>();
        pointCloud = new Vector3[meshFilters.Length];
            
        for(int i = 0; i < pointCloud.Length; i++)
            pointCloud[i] = meshFilters[i].gameObject.transform.position;
                
        tree = new KDTree(pointCloud, maxPointsPerLeafNode);
    }

    private void Update()
    {
        if (added)
        {
            KDQuery query = new KDQuery();
            List<int> results = new List<int>();

            Vector3 GroundSphere = camera.transform.position;
            // GroundSphere.y = 0; (force highlighting to specific plane)

            // spherical query
            query.Radius(tree, GroundSphere, radius, results);

            GameObject activeObject;
            VXComponent activeObjectComponent;
            // Debug.Log("Objects Found: " + results.Count);
            for (int i = 0; i < results.Count; ++i)
            {
                activeObject = meshFilters[results[i]].gameObject;
                activeObjectComponent = activeObject.GetComponent<VXComponent>(); 
                if (activeObjectComponent == null)
                {
                    activeObjectComponent = activeObject.AddComponent<VXComponent>();
                    activeObjectComponent.CanExpire = true;
                }
                else
                {
                    activeObjectComponent.Refresh();
                }
            }
        }
    }
}
