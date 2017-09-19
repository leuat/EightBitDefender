using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class MainGame : MonoBehaviour
    {

        GameFightLevel gl = new GameFightLevel();
        Dialogue dialogue = null;
        int currentDialogue = 0;
        string[] dialogues = new string[] { "testscene", "testscene2" };

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


        }

        // Update is called once per frame
        void Update()
        {
            //gl.Test(40);
            gl.Update();

            if (dialogue == null && currentDialogue < dialogues.Length)
                dialogue = new Dialogue(dialogues[currentDialogue], GameObject.Find("EffectsCamera").GetComponent<CRTScreen>(), GetComponent<Camera>());


            if (dialogue != null)
            {
                dialogue.Update();
                if (dialogue.isDone)
                {
                    dialogue = null;
                    GameSettings.state = GameSettings.GameState.Game;
                    currentDialogue++;
                }

            }
        

        }
    }
}
