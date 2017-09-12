using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class MainGame : MonoBehaviour
    {

        GameLevel gl = new GameLevel();
        Dialogue dialogue = null;

        public void Quit()
        {
            Application.Quit();
        }

        public void InvokeMenu()
        {
            if (BentoMenu.currentMessage[1] == "quit")
                Quit();

            if (BentoMenu.currentMessage[1] == "setLanguage")
                GameSettings.language = int.Parse(BentoMenu.currentMessage[2]);


        }


        void OnGUI()
        {
            Util.RenderFPS();

        }

        void Start()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            if (SerializedEntities.se == null)
            {
                SerializedEntities.se = SerializedEntities.DeSerialize(Constants.EntitiesXML);
                SerializedScenes.szScenes = SerializedScenes.DeSerialize(Constants.ScenesXML);
            }
            gl.Initialize();

            dialogue = new Dialogue("testscene", GameObject.Find("EffectsCamera").GetComponent<CRTScreen>(), GetComponent<Camera>());

        }

        // Update is called once per frame
        void Update()
        {
            //gl.Test(40);
            gl.Update();

            if (dialogue != null)
            {
                dialogue.Update();
                if (dialogue.isDone)
                {
                    dialogue = null;
                    GameSettings.state = GameSettings.GameState.Game;
                }

            }
        

        }
    }
}
