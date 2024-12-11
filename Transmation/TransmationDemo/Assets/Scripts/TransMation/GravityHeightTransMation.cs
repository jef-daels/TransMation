using System;
using TransMation;
using UnityEngine;

namespace TransMation
{
    /// <summary>
    /// to animate gravity, we need more info than just the TransMation ones
    /// from is the starting height, we assume going up
    /// to is the final height (either going up or down needs to be specified in the property isToTop)
    /// </summary>
    public class GravityHeightTransMation : TransMation<float>
    {
        //public bool IsToTop { get; set; } = true;
        public Vector3 AnimationGravity { get; private set; } = Physics.gravity;
        /// <summary>
        /// from is the starting value, to is the end value, they don't have to be the same actually
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="duration"></param>
        /// <param name="delay"></param>
        /// <param name="animationGravity">must be set in the ctor, since it helps construct the lerp function</param>
        public GravityHeightTransMation(float from, float to, float duration, float delay, Vector3 animationGravity)
            : base(null)
        {
            SetFrom(from);
            SetTo(to);
            SetAnimationGravity(animationGravity);
            Duration = duration;
            SetDelay(delay);
            LerpFunction = GravityLerpFunction;
        }
        public override TransMation<float> SetDuration(float duration)
        {
            throw new Exception("GravityHeightTransMation doesn't allow to change it's duration outside the constructor");
        }

        /// <summary>
        /// must not be made public, should only be called in ctor
        /// </summary>
        /// <param name="animationGravity"></param>
        protected void SetAnimationGravity(Vector3 animationGravity)
        {
            AnimationGravity = animationGravity;
        }

        public Func<float, float, float, float> GravityLerpFunction
        {
            get
            {
                //yt = y0 + v0t + 1/2at^2
                //vt = vo + at
                float v0 = (To - From - AnimationGravity.y * Duration * Duration / 2) / Duration;
                return new Func<float, float, float, float>((from, to, progress)
                     => from + v0 * progress * Duration
                     + AnimationGravity.y * progress * progress * Duration * Duration / 2
                );
            }
        }

    }
}
