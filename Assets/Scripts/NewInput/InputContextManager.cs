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
            Debug.Log("<color=green>[INPUT CONTEXT] ✓ Initialized with Gameplay context</color>");
        }
        
        public void PushContext(InputContext context)
        {
            contextStack.Push(context);
            Debug.Log($"<color=cyan>[INPUT CONTEXT] Pushed: {context} (Stack: {contextStack.Count})</color>");
        }
        
        public void PopContext()
        {
            if (contextStack.Count > 1)
            {
                InputContext popped = contextStack.Pop();
                Debug.Log($"<color=yellow>[INPUT CONTEXT] Popped: {popped} (Stack: {contextStack.Count})</color>");
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
            Debug.Log("<color=magenta>[INPUT CONTEXT] Reset to Gameplay</color>");
        }
    }
}
