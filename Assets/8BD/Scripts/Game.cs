using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class Game
    {
        public static Game game = new Game();

        public void Initialize()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            SerializedEntities.se = SerializedEntities.DeSerialize(Constants.EntitiesXML);
            SerializedScenes.szScenes = SerializedScenes.DeSerialize(Constants.ScenesXML);
            SerializedMapCategories.mapCategories = SerializedMapCategories.DeSerialize(Constants.CategoriesXML);
            SerializedGameLevels.gameLevels = SerializedGameLevels.DeSerialize(Constants.GameLevelsXML);

        }





    }

}