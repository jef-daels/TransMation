using UnityEngine;

namespace TransMation
{
    public class TransMationPausedState<T> : TransMationState<T> where T : struct
    {
        //private float _pauseStartTime;
        private TransMationStates _resumeState;
        public TransMationPausedState(TransMation<T> transMation)
            : base(transMation)
        {
            //_pauseStartTime = Time.realtimeSinceStartup;
            //_pauseStartTime = Time.time;
            _resumeState = transMation.State.State;
        }

        public override TransMationStates State { get => TransMationStates.Paused; }

        public override TransMationState<T> Update()
        {
            return this;
        }

        public override TransMationState<T> TogglePause()
        {
            //since we are in paused state, we should resume
            //TransMation.AddPauseDuration(Time.realtimeSinceStartup - _pauseStartTime);
            TransMation.AddPausedDuration(Time.deltaTime);

            //only 2 states can be paused so far: indelay and running
            if (_resumeState == TransMationStates.InDelay)
                return new TransMationInDelayState<T>(TransMation);
            else
                return new TransMationRunningState<T>(TransMation);
        }
    }
}
