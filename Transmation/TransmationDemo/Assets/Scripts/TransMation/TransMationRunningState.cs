using UnityEngine;

namespace TransMation
{
    public class TransMationRunningState<T> : TransMationState<T> where T : struct
    {
        public TransMationRunningState(TransMation<T> transMation)
            : base(transMation)
        {
        }

        public override TransMationStates State { get => TransMationStates.Running; }

        public override TransMationState<T> Update()
        {
            //while in progress, if we animate a frame, there's some possibilities:
            //we animated through a complete iteration (exceeded the animation duration)
            //    if no more iterations should be performed: move to end state
            //    else: start over
            //the animation iteration is not complete yet: perform the logic and stay in this state
            if (TransMation.TransMationDurationExceeded) //1 total animation iteration ended
            {
                TransMation.CurrentIteration++;
                TransMation.CurrentValue =
                    (TransMation.ReverseMode == TransMationReverseMode.None)
                    ? TransMation.To : TransMation.From;

                if (TransMation.CurrentIteration >= TransMation.MaxIterations
                    && TransMation.MaxIterations > 0)
                {
                    TransMation.EndIteration();
                    return new TransMationEndedState<T>(TransMation);
                }
                else //add one more iteration
                {
                    //needs to Start to reset the state
                    TransMation.EndIteration();
                    TransMation.StartIteration();
                    return new TransMationInDelayState<T>(TransMation);
                }
            }
            else
            {
                TransMation.Progress();
                return this;
            }
        }

        public override TransMationState<T> TogglePause()
        {
            //if in running state, this only means we need to pause
            return new TransMationPausedState<T>(TransMation);
        }
    }
}
