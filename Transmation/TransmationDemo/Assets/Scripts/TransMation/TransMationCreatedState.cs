using UnityEngine;

namespace TransMation
{
    public class TransMationCreatedState<T> : TransMationState<T> where T : struct
    {
        public TransMationCreatedState(TransMation<T> transMation)
            : base(transMation)
        {
        }

        public override TransMationStates State { get => TransMationStates.Created; }

        public override TransMationState<T> Update()
        {
            TransMation.StartIteration();
            if (TransMation.Delay == 0)
                return new TransMationRunningState<T>(TransMation);
            else
                return new TransMationInDelayState<T>(TransMation);
        }

        public override TransMationState<T> TogglePause()
        {
            return this;
        }
    }
}
