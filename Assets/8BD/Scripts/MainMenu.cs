using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LemonSpawn
{

    public class MainMenu : MonoBehaviour
    {


        // Use this for initialization
        GameFightLevel gl = new GameFightLevel();

        public void LoadGame()
        {
            //            Application.LoadLevel(1);
            GameFightLevel.DestroyAll();
            SceneManager.LoadScene(1);
        }


        public void Quit()
        {
            Application.Quit();
        }

        public void InvokeMenu()
        {
            if (BentoMenu.currentMessage.Length == 0)
                return;

            if (BentoMenu.currentMessage[1] == "quit")
                Quit();
            if (BentoMenu.currentMessage[1] == "start")
                LoadGame();
            if (BentoMenu.currentMessage[1] == "setLanguage")
            {
                GameSettings.language = int.Parse(BentoMenu.currentMessage[2]);
            }
                


        }


        void OnGUI()
        {
            Util.RenderFPS();

        }

        void Start()
        {
            //            Color c = ColorUtility.TryParseHtmlString
            Game.game.Initialize();
            gl.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            gl.Test(40);
            gl.Update();
           
        }
    }
}
