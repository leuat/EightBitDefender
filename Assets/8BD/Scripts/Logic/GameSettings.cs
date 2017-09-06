using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LemonSpawn
{

    public class Constants
    {
        public static string EntitiesXML = "Entities";
    }


    public class GameSettings
    {
        public static float zHeight = 0;
        //        public static string BillboardMaterial = "Materials/Billboard";
        public static Material BillboardMaterial = (Material)Resources.Load("Materials/Billboard");

    }

}