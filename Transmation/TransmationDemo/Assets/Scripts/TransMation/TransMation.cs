using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TransMation
{
    public class ProgressEventArgs<T> : EventArgs
    {
        public T CurrentValue { get; private set; }
        public ProgressEventArgs(T currentValue)
        {
            CurrentValue = currentValue;
        }
    }
    public enum TransMationReverseMode
    {
        None,
        ReverseOnly,
        DuringDuration,
        DoubleDuration
    }
    public class TransMation<T> where T : struct
    {
        public event EventHandler IterationStarted;
        protected virtual void OnIterationStarted()
        {
            IterationStarted?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<ProgressEventArgs<T>> Progressed;
        protected virtual void OnProgressed(T newCurrentValue)
        {
            Progressed?.Invoke(this, new ProgressEventArgs<T>(newCurrentValue));
        }

        public event EventHandler IterationEnded;
        protected virtual void OnIterationEnded()
        {
            IterationEnded?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler DelayEnded;
        protected virtual void OnDelayEnded()
        {
            DelayEnded?.Invoke(this, EventArgs.Empty);
        }

        public bool IsStarted { get => State.State != TransMationStates.Created; }
        public bool IsFinished { get => State.State == TransMationStates.Ended; }
        public bool IsTranforming { get { return IsStarted && !IsFinished; } }

        //public float StartTime { get; private set; }

        private T currentValue;

        /// <summary>
        /// use a negative value for infinite interations 
        /// </summary>
        public int MaxIterations { get; protected set; } = 1;
        public int CurrentIteration { get; internal set; } = 0;
        public TransMationState<T> State { get; set; }
        public T From { get; protected set; }
        public T To { get; protected set; }
        public T CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
            }
        }

        public float CurrentProgressTime { get; private set; }
        private float _currentProgress = 0;
        public float CurrentProgress
        {
            get { return _currentProgress; }
            private set { _currentProgress = value; }
        }

        public TransMationReverseMode ReverseMode { get; protected set; } = TransMationReverseMode.None;
        protected float Duration { get; set; }
        /// <summary>
        /// time it takes before the Progress starts
        /// </summary>
        public float Delay { get; private set; } = 0;

        public virtual Func<T, T, float, T> LerpFunction { get; protected set; }

        public TransMation(Func<T, T, float, T> LerpFunction)
        {
            State = new TransMationCreatedState<T>(this);
            this.LerpFunction = LerpFunction;
        }

        /// <summary>
        /// set's the starting value of the transmation
        /// </summary>
        /// <param name="from"></param>
        /// <returns></returns>
        public TransMation<T> SetFrom(T from)
        {
            From = from;
            return this;
        }
        /// <summary>
        /// sets the endvalue of the transmation
        /// </summary>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual TransMation<T> SetTo(T to)
        {
            To = to;
            return this;
        }
        /// <summary>
        /// sets the duration of one transmation execution
        /// </summary>
        /// <param name="duration">The time (seconds) it takes for the transmation to complete 1 execution</param>
        /// <returns></returns>
        public virtual TransMation<T> SetDuration(float duration)
        {
            Duration = duration;
            return this;
        }
        /// <summary>
        /// sets the startdelay waited until the values start changing after the transmation started
        /// </summary>
        /// <param name="delay">the time (seconds) spend in a waiting state after the transmation starts, before values start changing</param>
        /// <returns></returns>
        public virtual TransMation<T> SetDelay(float delay)
        {
            Delay = delay;
            return this;
        }

        /// <summary>
        /// sets the maximum number of iterations the transmation will perform (default is 1)
        /// use a negative value for infinite iterations
        /// </summary>
        /// <param name="maxIterations">The number of iterations to perform before the animation ends</param>
        /// <returns></returns>
        public virtual TransMation<T> SetMaxIterations(int maxIterations)
        {
            MaxIterations = maxIterations;
            return this;
        }
        /// <summary>
        /// sets whether the TransMation yields values in reverse order (starting at to, going to from)
        /// </summary>
        /// <param name="reverseMode">What reverseMode is used</param>
        /// <returns></returns>
        public virtual TransMation<T> SetReverseMode(TransMationReverseMode reverseMode)
        {
            ReverseMode = reverseMode;
            return this;
        }


        /// <summary>
        /// how long does 1 iteration take, including the reverse mode setting
        /// </summary>
        public float TransMationDuration
        {
            get => (ReverseMode == TransMationReverseMode.DoubleDuration) ? Duration * 2 : Duration;
        }
        public bool TransMationDurationExceeded
        {
            get
            {
                //return Time.realtimeSinceStartup - StartTime - Delay
                //return Time.time - StartTime - Delay
                //    >= TransMationDuration + PausedDuration;
                return CurrentProgressTime - Delay - PausedDuration >= TransMationDuration;
            }
        }

        public bool HasDelayEnded
        {
            get
            {
                //return Time.realtimeSinceStartup >= StartTime + Delay + PausedDuration;
                //return Time.time >= StartTime + Delay + PausedDuration;
                return CurrentProgressTime >= Delay + PausedDuration;
            }
        }

        /// <summary>
        /// this is the co-routine which should be called to start the animation
        /// </summary>
        /// <returns></returns>
        public IEnumerator Animate()
        {
            while (State.State != TransMationStates.Ended)
            {
                //the state determines the next state during it's animate
                State = State.Update();
                yield return null;
            }
        }

        internal void Progress()
        {
            //float currentTime = Time.realtimeSinceStartup;
            //float currentTime = Time.time;
            CurrentProgressTime += Time.deltaTime;  //scaled!

            //progress should be a value in the range [0,1]
            float progress = (CurrentProgressTime - Delay - PausedDuration)
                / TransMationDuration;
            if (ReverseMode == TransMationReverseMode.DoubleDuration || ReverseMode == TransMationReverseMode.DuringDuration)
            {
                //if reversing then to-value is reached when progress == 0.5
                //to fit this in the lerpfunction, multiply by 2 (so it ranges to 1, which the lerpfunction expects)
                //on reversing (progress is above 0.5), we subtract the progress above 0.5, and multiply by 2 again to reach 0 in the end
                // on reverse: progress = (1- (progress -1))*2
                if (progress > 0.5f)
                    progress = 2 - 2 * progress;     // 1 + (1-progress*2)
                else
                    progress *= 2;
            }
            else if (ReverseMode == TransMationReverseMode.ReverseOnly)
            {
                progress = 1 - progress;
            }
            CurrentValue = LerpFunction(From, To, progress);
            CurrentProgress = progress;
            OnProgressed(currentValue);
        }

        internal virtual void StartIteration()
        {
            //StartTime = Time.realtimeSinceStartup;
            //StartTime = Time.time;
            CurrentProgressTime = 0;
            PausedDuration = 0;
            //OnStart?.Invoke();
            OnIterationStarted();
            //foreach (ITransmationListener<T> listener in _listeners)
            //    listener.OnStart();
        }
        internal virtual void EndIteration()
        {
            OnProgressed(currentValue);
            OnIterationEnded();
        }

        public void Reset()
        {
            CurrentIteration = 0;
            State = new TransMationCreatedState<T>(this);
            PausedDuration = 0;
            CurrentProgressTime = 0;
        }

        #region pause
        public void TogglePause()
        {
            State = State.TogglePause();
        }

        public float PausedDuration { get; protected set; }
        internal void AddPausedDuration(float duration)
        {
            PausedDuration += duration;
        }
        #endregion

        public class TechAnimationUtilities
        {
            public static Func<T, T, float, T> SinLerpFunction(Func<T, T, float, T> linearLerp)
            {
                //the actual function returned is defined below (return statement)
                //this function will be called with a (linear) progress in [0,1]

                //we will use this to calculate a general sin function result [0,1]: sinProgress
                //this sinProgress wil also be in the range [0,1]
                //use sinProgress with the linear Lerpfunction<T> to calculate a progress value 

                //Concerning this sin function: r*sin(pulsation*progress*phase)+intercept

                //r = 1/2, since the sin-function ranges distance 2 (-1=> 1) and we want 1
                //the sin will start at -pi/2 and end at pi/2 => half a circle
                //    this half a circle is completed when progress = 1
                //so period T (in the general sin function) = 2 (progress wise) (complete circel)
                //and pulsation=2*pi/T = pi
                //progress is the linear progress (in range [0,1]) we received as input
                //phase= -pi/2, since we want the sin to start at -1 and move up to 1
                //intercept = 0.5 to shift the total result to [0,1]
                // r*sin(pulsation*progress +phase) + 0.5
                return new Func<T, T, float, T>((from, to, progress) =>
                {
                    float sinProgress = 0.5f * MathF.Sin(MathF.PI * progress - MathF.PI / 2) + 0.5f;
                    return linearLerp(from, to, sinProgress);
                });
            }
            public static float GravityLerp(float from, float to, float progress)
            {
                //progress is normalized: ranges in [0,1]
                //assumption: the starting vertical velocity is 0
                //the lerp function should reach the top halfway the duration (t=0.5) => v=0 = v0+a*t
                //v0 = -gravity*duration/2   normalized duration = 1 (progress in [0,1]
                //yt = yfrom + v0*t + 1/2*a*t^2
                float v0 = -Physics.gravity.y / 2;
                return from + v0 * progress + Physics.gravity.y * progress * progress / 2;
            }
        }
    }
}
