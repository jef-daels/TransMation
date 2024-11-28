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
        public bool IsToTop { get; set; } = true;
        public GravityHeightTransMation(float from, float to, float duration, float delay
            , Action onStart)
            : base(null)
        {
            SetFrom(from);
            SetTo(to);
            Duration = duration;
            SetDelay(delay);
            LerpFunction = GravityLerpFunction;
        }
        public override TransMation<float> SetDuration(float duration)
        {
            throw new Exception("GravityHeightTransMation doesn't allow to change it's duration outside the constructor");
        }

        public Func<float, float, float, float> GravityLerpFunction
        {
            get
            {
                //yt = y0 + v0t + 1/2at^2
                //vt = vo + at
                float v0 = (To - From - Physics.gravity.y * Duration * Duration / 2) / Duration;
                return new Func<float, float, float, float>((from, to, progress)
                     => from + v0 * progress * Duration
                     + Physics.gravity.y * progress * progress * Duration * Duration / 2
                );
            }
        }

    }
}
