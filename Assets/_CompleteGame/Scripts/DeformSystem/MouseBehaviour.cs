using UnityEngine;

namespace GardenCutter
{

    public class MouseBehaviour : MonoBehaviour
    {
        [SerializeField] private float _radius = 0.8f;
        [SerializeField] private float _stepRadius = 0.4f;
        [SerializeField] private float _sterength = 0.01f;
        [SerializeField] private float _stepStrength = 0.01f;
        [Space] [SerializeField] private float _radiusErase = 1.9f;
        [SerializeField] private float _stepRadiusErase = 0.4f;
        [SerializeField] private float _sterengthErase = -0.4f;

        [SerializeField] private float _stepStrengthErase = 0.03f;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            RaycastHit hit;

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.transform.gameObject.tag == "DeformableMesh")
                    {
                        // hit.transform.GetComponent<MeshDeformer> ().Deform (hit.point, 1.0f, 0.1f, -1.0f, -0.1f, hit.normal);

                        hit.transform.GetComponent<MeshDeformer>()
                            .Deform(
                                hit.point,
                                _radius,
                                _stepRadius,
                                _sterength,
                                _stepStrength,
                                hit.normal);
                    }
                }
            }

            if (Input.GetMouseButton(1))
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    if (hit.transform.gameObject.tag == "DeformableMesh")
                    {
                        hit.transform.GetComponent<MeshDeformer>()
                            .Deform(
                                hit.point,
                                _radiusErase,
                                _stepRadiusErase,
                                _sterengthErase,
                                _stepStrengthErase,
                                hit.normal);
                    }
                }
            }
        }
    }
}