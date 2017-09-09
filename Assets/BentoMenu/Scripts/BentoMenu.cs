using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LemonSpawn
{


    [System.Serializable]
    public class BentoLayout
    {
        private GUIStyle style = null;// = new GUILayout();
        public Font font;
        public float fontSize = 10;
        public Color backgroundColor = Color.black;
        public Color textColor = Color.white;
        public Texture2D background;
        public Vector2 spacing;
        public float hoverScale =0.18f;
        public float wobbleAmplitude = 4;
        public float wobbleSpeed = 4;


        public GUIStyle GetStyle(Color c)
        {
           if (style==null)
                style = new GUIStyle();

            style.font = font;
            //            style.normal.textColor = textColor;
            style.normal.textColor = c;
            style.fontSize = (int)(fontSize * Screen.height / (float)800);
            style.alignment = TextAnchor.MiddleCenter;

            return style;
        }

    }


    public class BentoRenderPacket
    {
        public BentoLayout layout;
        public UnityEvent unityEvent;
    }

    public class BentoMenu : MonoBehaviour
    {

        public BentoMenuItem2D root = new BentoMenuItem2D();
        public BentoLayout layout = new BentoLayout();
        public BentoRenderPacket rp = new BentoRenderPacket();
        public string menuName = "";
        public Vector2 position;
        public UnityEvent callbackEvent;
        public static string currentMessage = "";



        // Use this for initialization
        void Start()
        {
            //root = this.gameObject.AddComponent<BentoMenuItem>();
            root.hidden = true;
            rp.layout = layout;
            rp.unityEvent = callbackEvent;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            root.Render(rp, position);
        }

        public static float getX(Vector2 p)
        {
            return p.x * Screen.width;
        }
        public static float getY(Vector2 p)
        {
            return p.y * Screen.height;
        }

        public static float getX(float p)
        {
            return p * Screen.width;
        }
        public static float getY(float p)
        {
            return p * Screen.height;
        }


    }
}
