using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeformableMesh : MonoBehaviour
{
    // mover o ponto (auto lógicakkkk): 
    // point += qtd * normal

    public List<Vector3> originalVertices, deformedVertices, controlPoints, normals;
    public int numControlPoints = 6;
    public float influenceRadius = 0.5f;
    public float smoothingFactor = 3f;
    public float customForce = 3f;

    Mesh mesh;

    private void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        originalVertices = new List<Vector3>(mesh.vertices);
        deformedVertices = new List<Vector3>(mesh.vertices);


        normals = new List<Vector3>(mesh.normals);

        // seleciona pontos aleatórios na mesh
        controlPoints = new List<Vector3>();

        for (int i = 0; i < numControlPoints; i++)
        {
            controlPoints.Add(originalVertices[Random.Range(0, originalVertices.Count)]);
        }
       
    }

    private void Update()
    {
        for (int i = 0; i < controlPoints.Count; i++)
        {
            int idxOriginalVertex = originalVertices.FindIndex(p => p == controlPoints[i]);
            ModifyPoint(controlPoints[i], customForce);
        }
    }

    // Warping
    public void ModifyPoint(Vector3 point, float force)
    {
        for (int i = 0; i < originalVertices.Count; i++)
        {

            float euclideanDistance = Vector3.Distance(deformedVertices[i], point);
            float attenuatedForceCosine = (1f + Mathf.Cos(Mathf.PI * (Vector3.Distance(point, deformedVertices[i]) / influenceRadius))) * force;

            // se tiver dentro do raio de influência
            if (euclideanDistance < influenceRadius)
            {
                // deforma o vértice
                deformedVertices[i] += (attenuatedForceCosine * normals[i].normalized) / smoothingFactor;
            } 
        }

        mesh.vertices = deformedVertices.ToArray();
        mesh.RecalculateNormals();
    }
}
