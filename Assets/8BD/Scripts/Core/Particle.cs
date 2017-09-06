using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class Particle
    {
        public Vector3 P;
        public Vector3 V;
        public Vector3 A;
        public float travelDistance = 0;

        public void Move()
        {
            travelDistance += V.magnitude * Time.deltaTime;
            P = P + V * Time.deltaTime;
            V = V + A * Time.deltaTime;
            A.Set(0, 0, 0);
        }

        public void CapSpeed(float speed)
        {
            if (V.magnitude > speed)
                V = V.normalized * speed;
        }

        public void Force(Vector3 f)
        {
            A = A + f;
        }


        public void Gravity(Particle o, float pow, float str, float keepDistance)
        {
            Vector3 dist = o.P - P;
            float d = Mathf.Clamp(dist.magnitude - keepDistance, 0.5f, 1E10f);
            Vector3 F = str / Mathf.Pow(d, pow)*dist.normalized;
            Force(F);

        }

        public void Avoid(Particle o)
        {
            Gravity(o, 2, -2, 0);
        }

        public void MoveTowards(Particle o)
        {
            Gravity(o, 0, 10, 0);
        }



    }
}
