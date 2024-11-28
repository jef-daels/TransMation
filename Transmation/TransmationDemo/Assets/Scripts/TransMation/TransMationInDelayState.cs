namespace TransMation
{
    public class TransMationInDelayState<T> : TransMationState<T> where T : struct
    {
        public TransMationInDelayState(TransMation<T> transMation)
            : base(transMation)
        {
        }

        public override TransMationStates State { get => TransMationStates.InDelay; }

        public override TransMationState<T> Update()
        {
            if (TransMation.HasDelayEnded)
                return new TransMationRunningState<T>(TransMation);
            else
                return this;
        }

        public override TransMationState<T> TogglePause()
        {
            return new TransMationPausedState<T>(TransMation);
        }


    }
}
