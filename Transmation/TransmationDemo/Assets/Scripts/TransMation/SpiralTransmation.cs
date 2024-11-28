using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TransMation
{
    public class SpiralTransMation : TransMation<Vector3>
    {
        private Vector3 SpiralCenter { get; set; }
        public float MaxAngle { get; private set; }
        public float MinAngle { get; private set; }
        private float AngleSpread { get; set; }
        public float CurrentAngle { get { return MinAngle + CurrentAngleProgress; } }

        /// <summary>
        /// how far did the angle move already from MinAngle (depends on CurrentProgress)
        /// </summary>
        public float CurrentAngleProgress { get { return (1 - SpiralCurrentProgress) * AngleSpread; } }

        public Func<float, float, float, float> ProgressLerp { get; set; }
        public float SpiralCurrentProgress
        {
            get
            {
                return ProgressLerp(0, 1, CurrentProgress);
            }
        }
        public float RCoefficient { get; private set; }
        private Vector3 CenterToMinAnglePoint { get; set; }    //V1
        private Vector3 CenterToFrom { get; set; }  //V2

        public Boolean IsDebug { get; set; }

        /// <summary>
        /// calculate how to spiral to an destinationPosition, around a spiralCenterPosition using spiralInfo
        /// </summary>
        /// <param name="center">centerpoint around which the values spiral (not the startingpoint!)</param>
        /// <param name="to">endpoint of the animated object</param>
        /// <param name="rCoefficient">endpoint of the animated object</param>
        public SpiralTransMation(Vector3 center, Vector3 to, float rCoefficient = 0.5f, float rotations = 1)
            : base(null)
        {
            SetFrom(Vector3.zero);
            SetTo(to);
            ProgressLerp = TransMation<float>.TechAnimationUtilities.SinLerpFunction(Mathf.Lerp);
            LerpFunction = SpiralLerpFunction;
            SpiralCenter = center;
            //we move along a spiral (with a centerpoint) from a furter away point to a closer point
            //the closer point is identified as a specific point (since we want to end there)
            //the outerpoint is identified through the maxAngle (the distance will be calculated)
            //the centerpoint is probably not part of the movement

            //implemented logic: spiral center, From is most outer point, spiraling to the To position: both are on the spiral
            //From itself is not specified, the maxangle is
            //v1: vector from center to To: CenterToMinAnglePoint
            //v2: vector from center to From point of the spiral (outer most): CenterToFrom

            //progress: [0,1]
            //to find a progress point, we need it's angle and r value (pole coordinate system)
            // currentAngle = MaxAngle - progress * angleSpread
            //
            //      minAngle = atan2(v1z,v1x)
            //      angleSpread = maxAngle - minAngle

            //pole coordinate (r,theta)
            //formula: r = (1+C2*currentAngleProgress)*||v1||       currentangleprogress = pro
            //       progress=0 => length r is ok
            //       progress=1 => (1+rCoef*AngleSpread)||v1|| = ||v2|| => C2 = (||v1||/||v2||-1)/anglespread
            CenterToMinAnglePoint = To - SpiralCenter;
            MinAngle = Mathf.Atan2(CenterToMinAnglePoint.z, CenterToMinAnglePoint.x);
            MaxAngle = MinAngle + Mathf.PI * 2 * rotations;
            AngleSpread = MaxAngle - MinAngle;
            //v2 has to be calculated for this signature
            RCoefficient = rCoefficient;
        }




        private Vector3 SpiralLerpFunction(Vector3 from, Vector3 to, float progress)
        {
            Vector3 currentDestination = CurrentAnglePosition();

            float distance = (currentDestination - To).magnitude;
            if (distance < 0.1)
                currentDestination = To;
            return currentDestination;
        }

        private Vector3 CurrentAnglePosition()
        {
            Vector3 currentDestination;
            float angle = CurrentAngle;
            float r = (1 + RCoefficient * CurrentAngleProgress) * CenterToMinAnglePoint.magnitude;
            //float y = MyMath.SinusEasing(From.y, To.y, Info.NumberOfSteps, Info.CurrentStep);
            float y = Mathf.Lerp(From.y, To.y, CurrentProgress);

            Vector3 offset = r * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
            offset += Vector3.up * (y - SpiralCenter.y);

            currentDestination = SpiralCenter + offset;
            return currentDestination;
        }
    }
}
