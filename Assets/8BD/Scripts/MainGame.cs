using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class MainGame : MonoBehaviour
    {


        // Use this for initialization
        EntityCollection eCollection = new EntityCollection();

        float deltaTime = 0.0f;


        Texture2D test=null;
        void OnGUI()
        {
//            if (test == null)
  //              test = (Texture2D)Resources.Load("Textures/entity_sopwith");
    //        GUI.DrawTexture(new Rect(0, 0, 300, 300), test);


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
            SerializedEntities.se = SerializedEntities.DeSerialize(Constants.EntitiesXML);
            eCollection.Test();
        }

        // Update is called once per frame
        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            eCollection.Update();
        }
    }
}
