using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class DisplayChar
    {
        public GameObject go;
        public Vector3 targetPosition = Vector3.zero;

        public float distanceFromTarget()
        {
            return (targetPosition - go.transform.position).magnitude;
        }

        public DisplayChar(Vector3 ip, Vector3 tp, Vector3 size, Material mat, SerializedCharacter ch)
        {
            targetPosition = tp;
            go = CreateBox(ip, size, mat, "Textures/Characters/" + ch.texture, Dialogue.getColor(ch.bcolor));

        }

        public DisplayChar(Vector3 ip, Vector3 tp, Vector3 size)
        {
            targetPosition = tp;
            //            go = CreateBox(ip, size, mat, "Textures/Characters/" + ch.texture, Dialogue.getColor(ch.bcolor));
            go = new GameObject();
            go.transform.rotation = Quaternion.Euler(0, 0, 0);

            //            gos.Add(go);
            go.transform.position = ip;
            go.transform.localScale = size;

        }



        public void Update()
        {
            float t = 0.9f;
            go.transform.position = t * go.transform.position + (1 - t) * targetPosition;


        }

        public void Destroy()
        {
            if (go != null)
                GameObject.Destroy(go);
        }

        public static GameObject CreateBox(Vector3 pos, Vector3 scale, Material mat, string texture, Color color)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.rotation = Quaternion.Euler(0, 0, 180);

            go.name = texture;
            //            gos.Add(go);
            go.transform.position = pos;
            go.transform.localScale = scale;

            Material m = new Material(mat.shader);
            m.CopyPropertiesFromMaterial(mat);
            if (texture!="")
                m.SetTexture("_MainTex", (Texture2D)Resources.Load(texture));
            m.SetColor("_bgcolor", color);

            go.GetComponent<Renderer>().material = m;
            return go;

        }


    }


    public class Dialogue
    {
        private SerializedScene scene;
        private SerializedCharacter char1, char2, currentChar = null;
        private SerializedCRTSettings crt, orgSettings;
        private CRTScreen crtScreen;
        private GameObject dCam;
        private Camera mainCam;
        private int currentDialogue = 0;
        private int currentTextCounter = 0;
        private float currentTime = 0;
        private float maxTime = 0.1f;
        private DisplayChar dBubble, textGO;
        private TextMesh textMesh;
        private Material bubbleMaterial;
        DisplayChar dchar1, dchar2;
        private bool advance = false;
        private List<GameObject> gos = new List<GameObject>();




        public Dialogue(string scene_id, CRTScreen sc, Camera mc)
        {
            scene = SerializedScenes.szScenes.getScene(scene_id);
            if (scene == null)
            {
                Debug.Log("ERROR: COULD NOT FIND SCENE " + scene_id);
                return;
            }
            char1 = SerializedScenes.szScenes.getCharacter(scene.char1_id);
            char2 = SerializedScenes.szScenes.getCharacter(scene.char2_id);
            crt = SerializedScenes.szScenes.getCRTSettings(scene.CRTSettings_id);
            if (crt == null)
                Debug.Log("CANNOT FIND CRT SETTINGS " + scene.CRTSettings_id);

            crtScreen = sc;
            GameSettings.state = GameSettings.GameState.Dialogue;
            mainCam = mc;

            orgSettings = sc.settings;

            sc.settings = crt;
            CreateObjects();
            CreateAudio(scene.music);
        }

        AudioSource aus = null;
        AudioClip orgClip = null;

        private void CreateAudio(string audio)
        {
            aus = GameObject.Find("Audio Source").GetComponent<AudioSource>();
            if (aus == null)
                return;
            orgClip = aus.clip;
            AudioClip ac = (AudioClip)Resources.Load("Music/" + audio);
            if (ac != null)
            {
                aus.clip = ac;
                aus.loop = true;
                aus.Play();
            }
            else Debug.Log("Could not find audio " + audio);
        }


        public static Color getColor(string col)
        {
            Color color = Color.white;
            ColorUtility.TryParseHtmlString(col, out color);
            color.a = 1;
            return color;


        }

        private void CreateDialogueBubble(SerializedCharacter ch, string text)
        {
            if (dBubble != null)
                dBubble.Destroy();
          
            


            dBubble = new DisplayChar(new Vector3(0, -30, 0), new Vector3(0, -3, 0), new Vector3(18, 3, 0), GameSettings.BubbleMaterial, currentChar);
            //            dBubble.transform.position = new Vector3(0, -3, 0);
            //          dBubble.transform.localScale = new Vector3(18, 3, 0);
            //            tgo.transform.position = Vector3
            //tgo.transform.position = Vector3.zero;
            //SpriteRenderer s = tgo.AddComponent<SpriteRenderer>();
            //s.sprite = 

            //            bubbleMaterial = new Material(GameSettings.BubbleMaterial.shader);
            //          dBubble.GetComponent<Renderer>().material = bubbleMaterial;
            //        bubbleMaterial.SetColor("_bgcolor", getColor(ch.bcolor));



            if (textGO != null)
                textGO.Destroy();
               

            textGO = new DisplayChar(new Vector3(-80, -2f, -0.1f), new Vector3(-7, -2f, -0.1f), Vector3.one);
            //((new GameObject("Text");
            //textGO.go.transform.position = new Vector3(-8, -1.7f, -0.1f);

            textMesh = textGO.go.AddComponent<TextMesh>();
            textMesh.font = (Font)Resources.Load("Fonts/" + ch.font);
            textMesh.text = text;
            textMesh.color = getColor(ch.fcolor);
            textMesh.fontSize = 40;
            textMesh.characterSize = 0.2f;

            MeshRenderer rend = textGO.go.GetComponentInChildren<MeshRenderer>();
            rend.material = textMesh.font.material;




        }


        private void CreateObjects()
        {
            mainCam.enabled = false;
            //            mainCam.SetActive(false);

            dCam = new GameObject("DialogueCam");
            dCam.transform.position = new Vector3(0, 0, -8);
            //            dCam.transform.rotation = Quaternion.Euler(0, 0, 180);
            Camera cam = dCam.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Color;
            cam.backgroundColor = Color.black;
            gos.Add(dCam);

            float dx = -4;
            float y = 1f;
            float w = 0;
            float z = 1;
            float k = 1.3f;
            gos.Add(DisplayChar.CreateBox(new Vector3(0, 0, 9.5f), new Vector3(40, 30, w), GameSettings.BillboardMaterial,"Textures/Background/" + scene.background, Color.white));
            dchar1 = new DisplayChar(new Vector3(dx * 100, y, z), new Vector3(dx, y, z), new Vector3(7 * k, 5 * k, w), GameSettings.BillboardMaterial, char1);
            dchar2 = new DisplayChar(new Vector3(-dx * 100, y, z), new Vector3(-dx, y, z), new Vector3(7 * k, 5 * k, w), GameSettings.BillboardMaterial,  char2);

        }

        private void Restore()
        {
            foreach (GameObject g in gos)
                GameObject.Destroy(g);

            if (dBubble != null)
                dBubble.Destroy();
            if (textGO != null)
                textGO.Destroy();
            dchar1.Destroy();
            dchar2.Destroy();
            mainCam.enabled = true;
            crtScreen.settings = orgSettings;
            if (aus != null)
            {
                aus.Stop();
                aus.clip = orgClip;
                aus.Play();
            }
            

        }



        public void Update()
        {
            dchar1.Update();
            dchar2.Update();
            if (dBubble != null)
                dBubble.Update();
            if (textGO != null)
                textGO.Update();


            if (Input.GetMouseButtonUp(0))
            {
                advance = true;
                maxTime = 0.01f;
            }

            if (currentChar == null)
            {
                currentChar = SerializedScenes.szScenes.getCharacter(scene.texts[currentDialogue].character_id);
                CreateDialogueBubble(currentChar, currentChar.name.get() + ": ");
                currentTextCounter = 0;
                currentTime = 0;
            }
            else
            {
                currentTime += Time.deltaTime;
                if (currentTime > maxTime)
                {
                    currentTime = 0;
                    string s = scene.texts[currentDialogue].text.get();
                    if (currentTextCounter < s.Length)
                    {
                        textMesh.text += s[currentTextCounter];
                        currentTextCounter++;
                    }
                    else
                    {
                        if (advance && currentDialogue < scene.texts.Count-1)
                        {
                            currentTextCounter = 0;
                            currentDialogue++;
                            currentChar = null;
                            maxTime = 0.1f;
                            advance = false;
                        }
                        if (advance && currentDialogue == scene.texts.Count-1)
                        {
                            Finish();
                        }
                    }


                }
            }

        }

        bool initializeFinish = false;
        public bool isDone = false;
        void Finish()
        {
            if (!initializeFinish)
            {
                dchar1.targetPosition.x = dchar1.targetPosition.x * 4;
                dchar2.targetPosition.x = dchar2.targetPosition.x * 4;
                initializeFinish = true;
            }
            if (dchar1.distanceFromTarget() < 0.1)
            {
                Restore();
                isDone = true;
            }
                
        }

    }
}