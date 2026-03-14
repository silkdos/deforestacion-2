using UnityEngine;
using System;
using System.Collections;

namespace Deforestation.Interaction
{
    public enum MachineInteractionType
    {
        Door,
        Stairs,
        Machine
    }

    public class MachineInteraction : MonoBehaviour, IInteractable
    {
        [SerializeField] protected MachineInteractionType _type;
        [SerializeField] protected Transform _target;
        [SerializeField] protected InteractableInfo _interactableInfo;

        [SerializeField] private float _autoCloseDelay = 5f;

        private Vector3 _closedPosition;
        private bool _isOpen = false;

        private void Start()
        {
            // Guardamos la posición local (cerrada)
            _closedPosition = transform.localPosition;
        }

        public InteractableInfo GetInfo()
        {
            _interactableInfo.Type = _type.ToString();
            return _interactableInfo;
        }

        public virtual void Interact()
        {
            if (_type == MachineInteractionType.Door)
            {
                if (!_isOpen)
                {
                    OpenDoor();
                }
                else
                {
                    CloseDoor();
                }
            }

            if (_type == MachineInteractionType.Stairs)
            {
                GameController.Instance.TeleportPlayer(_target.position);
            }

            if (_type == MachineInteractionType.Machine)
            {
                GameController.Instance.MachineMode(true);
            }
        }

        private void OpenDoor()
        {
            transform.localPosition = _target.localPosition;
            _isOpen = true;

            StartCoroutine(AutoCloseDoor());
        }

        private void CloseDoor()
        {
            transform.localPosition = _closedPosition;
            _isOpen = false;
        }

        private IEnumerator AutoCloseDoor()
        {
            yield return new WaitForSeconds(_autoCloseDelay);

            if (_isOpen)
            {
                CloseDoor();
            }
        }
    }
}