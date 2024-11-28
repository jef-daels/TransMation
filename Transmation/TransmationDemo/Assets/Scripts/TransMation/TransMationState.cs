
using System.Collections;
using TransMation;
using Unity.VisualScripting;

namespace TransMation
{
    public enum TransMationStates
    {
        Created,
        InDelay,
        Running,
        Paused,
        Ended
    }
    public abstract class TransMationState<T> where T : struct
    {
        public TransMation<T> TransMation { get; protected set; }
        public abstract TransMationStates State { get; }
        public TransMationState(TransMation<T> transMation)
        {
            TransMation = transMation;
        }
        public abstract TransMationState<T> TogglePause();
        public abstract TransMationState<T> Update();
    }
}
