using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GardenCutter
{

    public class Scisslers : MonoBehaviour, ICut
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Animator _animator;
        [SerializeField] private Deformator _deformator;
        public bool IsCutting { get; set; }

        public void Cut()
        {
            _audioSource.Play();
            _deformator.IsCutting = true;
            _animator.SetTrigger("Cut");
            _animator.SetTrigger("Cut");
        }

        public void StopCut()
        {
            _audioSource.Stop();
            _deformator.IsCutting = false;
            _animator.SetTrigger("Stop");
            _animator.SetTrigger("Stop");
            _animator.SetTrigger("Stop");

Debug.Log($"Stop stop!!!!!");
        }
    }
}