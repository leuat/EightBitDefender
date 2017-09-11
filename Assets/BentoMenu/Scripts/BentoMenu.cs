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
        public Vector3 wobbleAmplitude = new Vector3(1, 1, 4);
        public Vector3 wobbleSpeed = new Vector3(2, 1.25f, 3.252f);
        public Vector3 clickRotate = new Vector3(200, 0, 0);
        public Material material;
        public Vector3 centerRotate = Vector3.zero;


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
        public List<BentoSingleMenu> bentoMenus = new List<BentoSingleMenu>();
        BentoSingleMenu activeMenu = null;
        public static string currentMessage = "";
        public static int autoLoadMenu = -1;
        public BentoLayout layout = new BentoLayout();

        void Start() {
            foreach (BentoSingleMenu bm in bentoMenus)
                bm.Initialize(GetComponent<Camera>(), layout);

            if (bentoMenus.Count>0)
                activeMenu = bentoMenus[0];

        }
        void Update()
        {
            foreach (BentoSingleMenu bm in bentoMenus)
            {
                if (activeMenu == bm)
                    bm.Update(true);

                else bm.Update(false);
            }
            if (autoLoadMenu!=-1)
            {
                activeMenu = bentoMenus[autoLoadMenu];
                autoLoadMenu = -1;
            }
        }
    }

    [System.Serializable]
    public class BentoSingleMenu
    {

        //        public BentoMenuItem3D root = new BentoMenuItem3D();
        public List<BentoMenuItem3D> items = new List<BentoMenuItem3D>();
        public BentoRenderPacket rp = new BentoRenderPacket();
        public string menuName = "";
        public Vector2 position;
        public Vector3 size;
        public UnityEvent callbackEvent;
        private Camera camera;


        // Use this for initialization
        public void Initialize(Camera cam, BentoLayout bl)
        {
            //root = this.gameObject.AddComponent<BentoMenuItem>();
 //           root.hidden = true;
            rp.layout = bl;
            rp.unityEvent = callbackEvent;
            camera = cam;
            foreach (BentoMenuItem3D bmi in items)
                bmi.Create(rp);
            //root.Create(new Vector3(position.x, position.y, 0), size, rp);
        }

        // Update is called once per frame
        public void Update(bool isActive)
        {
            //            root.Update(0);
            int i = 0;
            foreach (BentoMenuItem3D bmi in items)
                bmi.Update(isActive, i++);

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000.0f,11))
                {
                    //StartCoroutine(ScaleMe(hit.transform));
//                    Debug.Log("You selected the " + hit.transform.name); // ensure you picked right object
                    if (hit.transform.name.Contains("command"))
                    {
                        BentoMenu.currentMessage = hit.transform.name.Split(' ')[1];
                        callbackEvent.Invoke();
                        foreach (BentoMenuItem3D bmi in items)

                            bmi.CheckClick(hit.transform.gameObject);
                    }
                }
            }

        }


        void OnGUI()
        {
//            root.Render(rp, position);
        }


    }
}
