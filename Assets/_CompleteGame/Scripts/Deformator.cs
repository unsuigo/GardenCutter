using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GardenCutter
{

    public class Deformator : MonoBehaviour
    {

        [SerializeField] private float _radius = 0.5f;
        [SerializeField] private float _stepRadius = 0.05f;
        [SerializeField] private float _strength = -0.5f;
        [SerializeField] private float _stepStrength = -0.05f;
        [SerializeField] private Vector3 _direction = Vector3.up;


        public bool IsCutting { get; set; }

        public void OnCollisionStay(Collision collision)
        {
            if (!IsCutting)
            {
                return;
            }
            Debug.Log($"OnCollisionStay");

            if (collision.transform.gameObject.tag == "DeformableMesh")
            {
                MeshDeformer meshDeformer = collision.transform.GetComponent<MeshDeformer>();
                ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
                collision.GetContacts(contactPoints);
                foreach (ContactPoint contactPoint in contactPoints)
                {
                    meshDeformer.Deform(contactPoint.point, _radius, _stepRadius, _strength, _stepStrength, _direction);
                }
            }
        }
    }
}