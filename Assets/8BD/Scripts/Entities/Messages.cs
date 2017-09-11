using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LemonSpawn
{

    public class Message : GenericObject
    {
        private string message;
        private float time = 1;
        private Text text;
        public Message(Vector3 p, string s, Color c, GameObject parent, Font font, int fontSize)
        {
            message = s;
            go = new GameObject("Message");
            pos = p;
            go.transform.position = pos;
            go.transform.parent = parent.transform;
            text = go.AddComponent<Text>();
            text.text = s;
            text.font = font;
            text.color = c;
            text.rectTransform.sizeDelta = new Vector2(300, 70);
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = fontSize;
        }

        override public void Update()
        {
            base.Update();
            time -= Time.deltaTime*0.65f;
            if (time < 0) markForDeath = true;
            pos.y += Time.deltaTime*25;
            Color c = text.color;

            float ce = 0.5f;

            c.a = Mathf.Clamp(Mathf.Exp(-Mathf.Pow(time-ce,2)*10), 0, 1) ;
            text.color = c;
            if (go != null)
                go.transform.position = pos;
        }


    }


    public class Messages : GenericCollection
    {
        Font font;
        GameObject canvas;
        public Color colorNeutral = Color.yellow;
        public Color colorGood = new Color(0.5f, 1, 0.3f, 1.0f);// Color.green;
        public Color colorBad = new Color(1.0f, 0.4f, 0.2f);
        public float fontSize;
        public Camera MainCam;


        public void Initialize(Font f, GameObject can, float fs, Camera cam)
        {
            font = f;
            canvas = can;
            fontSize = fs;

            MainCam = cam;

        }

        public enum MessageType  {Neutral, Good, Bad };

        public void Add(string m, MessageType mt, Vector3 p) {
            Color c = colorBad;

            if (mt == MessageType.Neutral) c = colorNeutral;
            if (mt == MessageType.Good) c = colorGood;

            Vector3 pos = MainCam.WorldToScreenPoint(p);


            collection.Add(new Message(pos, m, c, canvas, font, (int)fontSize));


        }


    }
}
