using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LemonSpawn;

namespace LemonSpawn
{

    public class MapGame : MonoBehaviour
    {
        Map2D map = new Map2D();
        DisplayMap dMap = new DisplayMap();

        // Use this for initialization
        void Start()
        {
            Game.game.Initialize();

            map.Create(256, 256);
            dMap.Initialize(map, 30, 20, 1, 1);

//            map
        }

        // Update is called once per frame
        void Update()
        {
            float ts = 0.3f;
            float x = 90 + Mathf.Sin(Time.time*ts) * 30;
            float y = 90 + Mathf.Cos(Time.time*0.865f*ts) * 30;

            dMap.Update(x, y);
        }
    }

}