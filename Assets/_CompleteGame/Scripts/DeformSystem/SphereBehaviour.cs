using System.Collections;
using System.Collections.Generic;
using GardenCutter;
using UnityEngine;

namespace GardenCutter
{

    public class SphereBehaviour : MonoBehaviour
    {
        public void OnCollisionStay(Collision collision)
        {
            if (collision.transform.gameObject.tag == "DeformableMesh")
            {
                MeshDeformer meshDeformer = collision.transform.GetComponent<MeshDeformer>();
                ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
                collision.GetContacts(contactPoints);
                foreach (ContactPoint contactPoint in contactPoints)
                {
                    meshDeformer.Deform(contactPoint.point, 0.5f, 0.05f, -0.5f, -0.05f, Vector3.up);
                }
            }
        }
    }
}