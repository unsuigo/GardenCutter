using UnityEngine;
using System.Collections.Generic;

public class TreeGenerator : MonoBehaviour
{
    public enum ShapeType { Sphere, Cloud, TreeCrown }
    public ShapeType shapeType = ShapeType.Sphere;
    public int resolution = 10;
    public int cloudSphereCount = 10; // Number of spheres to create the cloud
    public float cloudSize = 5.0f; // Maximum size of the cloud
    public float maxSphereDistance = 1.0f; // Maximum distance between sphere centers

    void Start()
    {
        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard"));

        switch (shapeType)
        {
            case ShapeType.Sphere:
                meshFilter.mesh = GenerateLowPolySphere(resolution);
                break;
            case ShapeType.Cloud:
                meshFilter.mesh = GenerateLowPolyCloud(resolution, cloudSphereCount, cloudSize, maxSphereDistance);
                break;
            case ShapeType.TreeCrown:
                meshFilter.mesh = GenerateLowPolyTreeCrown(resolution);
                break;
        }
    }

    Mesh GenerateLowPolySphere(int resolution)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] triangles = new int[resolution * resolution * 6];

        float step = Mathf.PI * 2 / resolution;
        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float theta = x * step;
                float phi = y * Mathf.PI / resolution;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                vertices[y * (resolution + 1) + x] = new Vector3(sinPhi * cosTheta, cosPhi, sinPhi * sinTheta);
            }
        }

        int triIndex = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i0 = y * (resolution + 1) + x;
                int i1 = i0 + 1;
                int i2 = i0 + (resolution + 1);
                int i3 = i2 + 1;

                triangles[triIndex++] = i0;
                triangles[triIndex++] = i1;
                triangles[triIndex++] = i2;

                triangles[triIndex++] = i1;
                triangles[triIndex++] = i3;
                triangles[triIndex++] = i2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    Mesh GenerateLowPolyCloud(int resolution, int sphereCount, float size, float maxDistance)
    {
        List<Vector3> verticesList = new List<Vector3>();
        List<int> trianglesList = new List<int>();
        int vertexOffset = 0;

        Vector3[] centers = new Vector3[sphereCount];
        for (int i = 0; i < sphereCount; i++)
        {
            if (i == 0)
            {
                centers[i] = Vector3.zero;
            }
            else
            {
                Vector3 newCenter;
                bool isValid;
                do
                {
                    newCenter = new Vector3(
                        Random.Range(-size / 2, size / 2),
                        Random.Range(-size / 2, size / 2),
                        Random.Range(-size / 2, size / 2)
                    );
                    isValid = true;
                    for (int j = 0; j < i; j++)
                    {
                        if (Vector3.Distance(newCenter, centers[j]) > maxDistance)
                        {
                            isValid = false;
                            break;
                        }
                    }
                } while (!isValid);
                centers[i] = newCenter;
            }
        }

        foreach (Vector3 center in centers)
        {
            float radius = Random.Range(0.2f, 0.7f);
            Mesh sphereMesh = GenerateLowPolySphereWithRadius(resolution, center, radius);

            for (int i = 0; i < sphereMesh.vertexCount; i++)
            {
                verticesList.Add(sphereMesh.vertices[i]);
            }

            for (int i = 0; i < sphereMesh.triangles.Length; i++)
            {
                trianglesList.Add(sphereMesh.triangles[i] + vertexOffset);
            }

            vertexOffset += sphereMesh.vertexCount;
        }

        // Remove internal vertices
        RemoveInternalVertices(ref verticesList, ref trianglesList, maxDistance / 2);

        Mesh mesh = new Mesh();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    void RemoveInternalVertices(ref List<Vector3> vertices, ref List<int> triangles, float maxDistance)
    {
        HashSet<int> toRemove = new HashSet<int>();
        int vertexCount = vertices.Count;

        // Найти вершины, которые находятся внутри других сфер
        for (int i = 0; i < vertexCount; i++)
        {
            Vector3 vertex = vertices[i];
            bool isInternal = false;

            for (int j = 0; j < vertexCount; j++)
            {
                if (i != j && Vector3.Distance(vertex, vertices[j]) < maxDistance)
                {
                    isInternal = true;
                    break;
                }
            }

            if (isInternal)
            {
                toRemove.Add(i);
            }
        }

        // Создать новый список вершин и индексов треугольников
        List<Vector3> newVertices = new List<Vector3>();
        List<int> newTriangles = new List<int>();
        Dictionary<int, int> indexMap = new Dictionary<int, int>();
        int newIndex = 0;

        for (int i = 0; i < vertices.Count; i++)
        {
            if (!toRemove.Contains(i))
            {
                newVertices.Add(vertices[i]);
                indexMap[i] = newIndex++;
            }
        }

        for (int i = 0; i < triangles.Count; i += 3)
        {
            int i0 = triangles[i];
            int i1 = triangles[i + 1];
            int i2 = triangles[i + 2];

            if (!toRemove.Contains(i0) && !toRemove.Contains(i1) && !toRemove.Contains(i2))
            {
                newTriangles.Add(indexMap[i0]);
                newTriangles.Add(indexMap[i1]);
                newTriangles.Add(indexMap[i2]);
            }
        }

        vertices = newVertices;
        triangles = newTriangles;
    }

    Mesh GenerateLowPolySphereWithRadius(int resolution, Vector3 center, float radius)
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        int[] triangles = new int[resolution * resolution * 6];
        Vector3[] normals = new Vector3[(resolution + 1) * (resolution + 1)];

        float step = Mathf.PI * 2 / resolution;
        for (int y = 0; y <= resolution; y++)
        {
            for (int x = 0; x <= resolution; x++)
            {
                float theta = x * step;
                float phi = y * Mathf.PI / resolution;
                float sinPhi = Mathf.Sin(phi);
                float cosPhi = Mathf.Cos(phi);
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                Vector3 vertex = new Vector3(sinPhi * cosTheta, cosPhi, sinPhi * sinTheta) * radius;
                vertices[y * (resolution + 1) + x] = center + vertex;
                normals[y * (resolution + 1) + x] = vertex.normalized;
            }
        }

        int triIndex = 0;
        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                int i0 = y * (resolution + 1) + x;
                int i1 = i0 + 1;
                int i2 = i0 + (resolution + 1);
                int i3 = i2 + 1;

                triangles[triIndex++] = i0;
                triangles[triIndex++] = i1;
                triangles[triIndex++] = i2;

                triangles[triIndex++] = i1;
                triangles[triIndex++] = i3;
                triangles[triIndex++] = i2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        return mesh;
    }

    Mesh GenerateLowPolyTreeCrown(int resolution)
    {
        Mesh mesh = new Mesh();
        // Placeholder logic for generating a low poly tree crown
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1, 0, -1),
            new Vector3(1, 0, -1),
            new Vector3(1, 0, 1),
            new Vector3(-1, 0, 1),
            new Vector3(0, 2, 0)
        };

        int[] triangles = new int[]
        {
            0, 4, 1,
            1, 4, 2,
            2, 4, 3,
            3, 4, 0,
            0, 1, 2,
            2, 3, 0
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
