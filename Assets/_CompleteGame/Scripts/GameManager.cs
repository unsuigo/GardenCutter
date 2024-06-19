using UnityEngine;

namespace GardenCutter
{
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Cutter _cutter; 
        [SerializeField] private Scisslers _scisslers;

        private AudioSource _cutterAudioSource;

        private void Awake()
        {
            if (_cutter != null)
            {
                _cutterAudioSource = _cutter.GetComponent<AudioSource>();
            }
            else
            {
                Debug.LogError("Chainsaw object is not assigned in the GameManager.");
            }

            // EnableCutting(true);
        }

        public void EnableCutting(bool enable)
        {
            if (enable)
            {
                _cutter.IsCutting = true;
                _cutterAudioSource.Play();
                _scisslers.IsCutting = true;
                _scisslers.Cut();
                Debug.Log($"Cut");
            }
            else
            {
               _cutter.IsCutting = false;
                _cutterAudioSource.Stop();
               _scisslers.IsCutting = false;
               _scisslers.StopCut();
               Debug.Log($"Stop Cut");

            }

        }
    }
}