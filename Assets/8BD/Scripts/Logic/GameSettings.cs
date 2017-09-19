using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LemonSpawn
{

    public class Constants
    {
        public static string EntitiesXML = "Entities";
        public static string ScenesXML = "Scenes";
        public static string CategoriesXML = "MapCategories";
        public static string GameLevelsXML = "GameLevels";
    }





    public class GameSettings
    {
        public static float zHeight = 0;
        //        public static string BillboardMaterial = "Materials/Billboard";
        public static Material BillboardMaterial = (Material)Resources.Load("Materials/Billboard");
        public static Material BubbleMaterial = (Material)Resources.Load("Materials/BubbleMaterial");
        public static float birthMessageProbability = 0.25f;
        public static float deathMessageProbability = 0.25f;
        public static int language = 0;
        public enum GameState { Game, Dialogue, Pause, Menu};
        public static GameState state = GameState.Menu;
            

        

    }

}