using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TransMation
{
    /// <summary>
    /// spirals in the X,Z plane
    /// Y is untouched
    /// </summary>

    public class ArchimedeanSpiralTransMation : TransMation<Vector3>
    {
        float _endAlfa, _alfaTotal;
        float _pulsation;
        float _aCoefficient;    //spiral amplitude evolves
        public float Rotations { get; private set; }
        public ArchimedeanSpiralTransMation(Vector3 from, Vector3 to, float duration
            , float delay
            , float rotations)
            : base(  null)
        {
            SetFrom(from);
            SetTo(to);
            Duration =duration; //Don't call setDuration: it's not implemented
            SetDelay(delay);
            Rotations = rotations;
            _endAlfa = Mathf.Atan2(to.z - from.z, to.x- from.x);
            _alfaTotal = Mathf.PI * 2 * Rotations + _endAlfa;
            _pulsation = _alfaTotal / TransMationDuration;
            Vector3 distance = to - from;
            float distanceMagnitude = MathF.Sqrt(distance.x * distance.x + distance.z * distance.z);
            _aCoefficient = distanceMagnitude / _alfaTotal;
        }

        public override TransMation<Vector3> SetDuration(float duration)
        {
            throw new Exception("ArchimedeanSpiralTransMation doesn't allow to change it's duration outside the constructor");
        }

        public override Func<Vector3, Vector3, float, Vector3> LerpFunction
        {
            get => ArchimedeanSpiralLerp;
        }

        private Vector3 ArchimedeanSpiralLerp(Vector3 from, Vector3 to, float progress)
        {
            //on progress 0, we must be at from
            //on progress 1, we must be at to
            float progressAlfa = _pulsation * progress * TransMationDuration;
            float r = _aCoefficient * progressAlfa;
            return from + new Vector3(r * Mathf.Cos(progressAlfa), 0, r * Mathf.Sin(progressAlfa));
        }
    }
}
