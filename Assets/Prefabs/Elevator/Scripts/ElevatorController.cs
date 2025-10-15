using UnityEngine;

namespace VRArtMaking
{
    public class ElevatorController : MonoBehaviour
    {
        [SerializeField] private Animator elevatorAnimator;
        [SerializeField] private bool isOpen = false;
        
        private void LateStart()
        {
            elevatorAnimator.SetBool("isOpen", isOpen);
        }

        private void Update()
        {
            elevatorAnimator.SetBool("isOpen", isOpen);
        }
        
        public void Open()
        {
            isOpen = true;
        }
        
        public void Close()
        {
            isOpen = false;
        }
    }
}

