namespace TransMation
{
    public class TransMationEndedState<T> : TransMationState<T> where T : struct
    {
        public TransMationEndedState(TransMation<T> transMation)
            : base(transMation)
        {
        }

        public override TransMationStates State { get => TransMationStates.Ended; }

        public override TransMationState<T> Update()
        {
            return this;
        }

        public override TransMationState<T> TogglePause()
        {
            return this;
        }
    }
}
