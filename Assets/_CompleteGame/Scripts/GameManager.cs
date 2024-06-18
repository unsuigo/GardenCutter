using UnityEngine;

namespace GardenCutter
{
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Cutter cutter; // Reference to the chainsaw object

        private AudioSource audioSource;

        private void Awake()
        {
            if (cutter != null)
            {
                audioSource = cutter.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("Chainsaw object is not assigned in the GameManager.");
            }
        }

        public void EnableCutting(bool enable)
        {
            if (enable)
            {
                cutter.IsCutting = true;
                audioSource.Play();
                    
            }
            else
            {
               cutter.IsCutting = false;
                audioSource.Stop();
            }

        }
    }
}