using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class MainMenu : MonoBehaviour
    {


        // Use this for initialization
        GameLevel gl = new GameLevel();

        MenuItem menu = null;
        MenuLayout mLayout;

        float deltaTime = 0.0f;


        public void LoadGame(System.Object o)
        {
            string name = (string)o;
            Debug.Log(name);

        }


        public void Quit()
        {
            Application.Quit();
        }

        public void InvokeMenu()
        {
            if (BentoMenu.currentMessage == "quit")
                Quit();
            if (BentoMenu.currentMessage == "start")
                Debug.Log("Start");
        }


        void OnGUI()
        {

//            menu.Render(new Vector2(0.5f*Screen.width, 0.1f*Screen.height));

            int w = Screen.width, h = Screen.height;

            GUIStyle style = new GUIStyle();

            Rect rect = new Rect(0, 0, w, h * 2 / 100);
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 4 / 100;
            style.normal.textColor = Color.white;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            GUI.Label(new Rect(10,10,100,100), text, style);
        }

        void Start()
        {
            Screen.orientation = ScreenOrientation.LandscapeLeft;
            SerializedEntities.se = SerializedEntities.DeSerialize(Constants.EntitiesXML);
            gl.Initialize();
        }

        // Update is called once per frame
        void Update()
        {
            gl.Test(40);
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            gl.Update();
           
        }
    }
}
