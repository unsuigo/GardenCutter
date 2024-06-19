using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GardenCutter
{
    class Slicer
    {
        public static GameObject[] Slice(Plane plane, GameObject objectToCut)
        {
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;

            if (mesh == null)
            {
                Debug.LogError("Mesh is null. Ensure the object has a MeshFilter component with a valid mesh.");
                return null;
            }

            if (!mesh.isReadable)
            {
                Debug.LogError("The mesh is not readable. Enable Read/Write in import settings.");
                return null;
            }

            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null)
            {
                throw new NotSupportedException("Cannot slice non sliceable object");
            }

            SlicesMetadata slicesMeta = new SlicesMetadata(plane, mesh, sliceable.IsSolid,
                sliceable.ReverseWireTriangles, sliceable.ShareVertices, sliceable.SmoothVertices);

            GameObject positiveObject = CreateMeshGameObject(objectToCut);
            positiveObject.name = string.Format("{0}_positive", objectToCut.name);

            GameObject negativeObject = CreateMeshGameObject(objectToCut);
            negativeObject.name = string.Format("{0}_negative", objectToCut.name);

            var positiveSideMeshData = slicesMeta.PositiveSideMesh;
            var negativeSideMeshData = slicesMeta.NegativeSideMesh;

            positiveObject.GetComponent<MeshFilter>().mesh = positiveSideMeshData;
            negativeObject.GetComponent<MeshFilter>().mesh = negativeSideMeshData;

            // Assign the same material as the original object to both new objects
            var originalMaterial = objectToCut.GetComponent<MeshRenderer>().material;
            positiveObject.GetComponent<MeshRenderer>().material = originalMaterial;
            negativeObject.GetComponent<MeshRenderer>().material = originalMaterial;

            // Add Sliceable component to new objects
            // positiveObject.AddComponent<Sliceable>().UseGravity=true;
            // negativeObject.AddComponent<Sliceable>().UseGravity=true;

            // Copy all attributes from the original object to the new objects
            SetupAttributes(objectToCut, positiveObject);
            SetupAttributes(objectToCut, negativeObject);

            // Calculate the number of triangles to determine which one is smaller
            int positiveTriangles = positiveSideMeshData.triangles.Length / 3;
            int negativeTriangles = negativeSideMeshData.triangles.Length / 3;
                SetupMeshCollider(ref negativeObject, negativeSideMeshData);
                SetupMeshCollider(ref positiveObject, positiveSideMeshData);

            if (positiveTriangles < negativeTriangles)
            {
                SetupSelfDestruct(ref positiveObject,  sliceable.UseGravity);
                negativeObject.transform.parent = objectToCut.transform.parent;
            }
            else
            {
                SetupSelfDestruct(ref negativeObject, sliceable.UseGravity);
                positiveObject.transform.parent = objectToCut.transform.parent;
            }

            return new GameObject[] { positiveObject, negativeObject };
        }

        private static void SetupSelfDestruct(ref GameObject obj, bool useGravity)
        {
            Rigidbody rb = obj.AddComponent<Rigidbody>();
            rb.mass = 5;
            rb.useGravity = useGravity;
            obj.AddComponent<SelfDestruct>();
            Sliceable sl = obj.GetComponent<Sliceable>();
            Object.Destroy(sl);

        }

        private static void SetupMeshCollider(ref GameObject obj, Mesh mesh)
        {
            MeshCollider collider = obj.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
            collider.convex = true;
        }

        private static void SetupAttributes(GameObject original, GameObject newObject)
        {
            newObject.transform.position = original.transform.position;
            newObject.transform.rotation = original.transform.rotation;
            newObject.transform.localScale = original.transform.localScale;

            newObject.AddComponent<Sliceable>().UseGravity=true;
            
            
            // Copy any other necessary components from the original to the new object
            // Example: Copying Rigidbody settings (if it exists on the original object)
            // Rigidbody originalRb = original.GetComponent<Rigidbody>();
            // if (originalRb != null)
            // {
            //     Rigidbody newRb = newObject.AddComponent<Rigidbody>();
            //     newRb.mass = originalRb.mass;
            //     newRb.drag = originalRb.drag;
            //     newRb.angularDrag = originalRb.angularDrag;
            //     newRb.useGravity = originalRb.useGravity;
            //     newRb.isKinematic = originalRb.isKinematic;
            // }
        }

        private static GameObject CreateMeshGameObject(GameObject original)
        {
            GameObject obj = new GameObject();
            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>().materials = original.GetComponent<MeshRenderer>().materials;
            return obj;
        }
    }
}
