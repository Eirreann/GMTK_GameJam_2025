using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Interactions
{
    public class EndGoalInteractable : Interactable
    {
        [SerializeField] private SkinnedMeshRenderer _renderer;
        [SerializeField] private Material _activeMat;
        [SerializeField] private AnimationClip _activateAnim;
        
        private Animator _anim;
        private bool _isGameComplete = false;

        private void Start()
        {
            _anim = GetComponent<Animator>();
        }

        public void SetReadyForCompletion()
        {
            triggered = false;
            _isGameComplete = true;
        }

        public override bool Interact(bool status)
        {
            if (_isGameComplete)
            {
                isEnabled = false;
                _renderer.material = _activeMat;
                _anim.Play(_activateAnim.name);
                StartCoroutine(_activateOnDelay());
                return true;
            }
            else
            {
                return base.Interact(status);
            }
        }

        private IEnumerator _activateOnDelay()
        {
            yield return new WaitForSeconds(_activateAnim.length + 1f);
            base.Interact(true);
        }
    }
}