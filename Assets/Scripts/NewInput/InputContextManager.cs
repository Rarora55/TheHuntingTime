using System.Collections.Generic;
using UnityEngine;

namespace TheHunt.Input
{
    public enum InputContext
    {
        Gameplay,
        Inventory,
        Dialog,
        Pause,
        Cutscene
    }

    public class InputContextManager : MonoBehaviour
    {
        private Stack<InputContext> contextStack = new Stack<InputContext>();
        
        public InputContext CurrentContext => contextStack.Count > 0 ? contextStack.Peek() : InputContext.Gameplay;
        
        void Awake()
        {
            PushContext(InputContext.Gameplay);
        }
        
        public void PushContext(InputContext context)
        {
            contextStack.Push(context);
        }
        
        public void PopContext()
        {
            if (contextStack.Count > 1)
            {
                InputContext popped = contextStack.Pop();
            }
            else
            {
                Debug.LogWarning("[INPUT CONTEXT] Cannot pop last context (Gameplay must remain)");
            }
        }
        
        public bool IsInContext(InputContext context)
        {
            return CurrentContext == context;
        }
        
        public bool AllowsGameplayInput()
        {
            return CurrentContext == InputContext.Gameplay;
        }
        
        public void ClearToGameplay()
        {
            contextStack.Clear();
            PushContext(InputContext.Gameplay);
        }
    }
}
