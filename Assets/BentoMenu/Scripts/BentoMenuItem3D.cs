using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LemonSpawn
{
    public class BentoState
    {
//        public enum BentoStateCategory { None, HiddenX, HiddenY, HiddenXY, RotY, RotX };
       

        public Vector3 position;
        public Vector3 localScale;
        public Vector3 rotation;

        public BentoState(Vector3 pos, Vector3 local, Vector3 rot)
        {
            position = pos;
            localScale = local;
            rotation = rot;

        }


        public BentoState(BentoState other, Vector3 pos, Vector3 scale, Vector3 rot)
        {
            position = other.position;
            localScale = other.localScale;
            rotation = other.rotation;
            if (pos.x!=0)
                position.x = pos.x;

            if (pos.y != 0)
                position.y = pos.y;

        }
        public void Apply(GameObject go)
        {
            go.transform.position = position;
            go.transform.localScale = localScale;
//            go.transform.rotation = Quaternion.Euler(rotation);
        }
        public void Near (BentoState o, float t)
        {
            position = position * t + (o.position * (1 - t));
            rotation = rotation * t + (o.rotation * (1 - t));
            localScale = localScale * t + (o.localScale * (1 - t));
        }



    }

    [System.Serializable]
    public class BentoMenuItem3D
    {
        public List<BentoMenuItem3D> children = new List<BentoMenuItem3D>();
        //        public Callback callback;
        public Texture2D image;
        private GameObject go;
        private GameObject textGo;
        public enum DisplayFormat { ImageOnly, TextOnly, ImageLeft, ImageRight };
        public DisplayFormat displayFormat = DisplayFormat.ImageLeft;
        public Color color = Color.gray;
        public Vector3 initialPos = new Vector3(0, 0, 0);

        public Vector3 size = new Vector3(5, 3, 2);
        public Vector3 position = new Vector3(0, 0, 0);

        public int AutoLoadMenu = -1;

        private static GUIStyle emptyStyle = new GUIStyle();
        private BentoRenderPacket brp;

        private BentoState currentState;
        private BentoState targetState;
        private BentoState initialState;

        private float clickedSpeed;

        public string text;
        public string eventMessage;

        private Material material;

        public void CheckClick(GameObject c)
        {
            if (c == go)
            {
                clickedSpeed = 1;

                if (AutoLoadMenu != -1)
                    BentoMenu.autoLoadMenu = AutoLoadMenu;
            }
            
        }


        public void Create(BentoRenderPacket rp)
        {
            if (go != null)
                return;


                brp = rp;
                go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "command " + eventMessage;

                targetState = new BentoState(position, size, Vector3.zero);
                initialState = new BentoState(targetState, initialPos, Vector3.zero, Vector3.zero);
                currentState = new BentoState(initialState.position, initialState.localScale, initialState.rotation);

                go.transform.position = position;
                go.transform.localScale = size;
                material = new Material(brp.layout.material.shader);
                material.CopyPropertiesFromMaterial(brp.layout.material);
                material.SetTexture("_MainTex", rp.layout.background);
                go.GetComponent<Renderer>().material = material;


                GameObject go2 = new GameObject();
                go2.transform.parent = go.transform;
                Vector3 ls = go2.transform.localScale;
                go2.transform.localPosition = new Vector3(0, 0, -0.5f*size.z* ls.z);
                TextMesh t = go2.AddComponent<TextMesh>();
                t.alignment = TextAlignment.Center;
                t.font = brp.layout.font;
                t.anchor = TextAnchor.MiddleCenter;
                t.text = text;
                color.a = 1;
                t.color = color;
                t.fontSize = (int)brp.layout.fontSize;
                t.characterSize = 0.3f;

            // Updated font
                MeshRenderer rend = go2.GetComponentInChildren<MeshRenderer>();
                rend.material = t.font.material;  

            }


        


        public void Update(bool isActive, float phase)
        {
            clickedSpeed *= 0.9f;
            if (go != null)
            {
                go.transform.rotation = Quaternion.Euler(
                    brp.layout.centerRotate.x+ brp.layout.wobbleAmplitude.x * (Mathf.Sin((brp.layout.wobbleSpeed.x) * Time.time + phase) + clickedSpeed*brp.layout.clickRotate.x),
                    brp.layout.centerRotate.y + brp.layout.wobbleAmplitude.y * (Mathf.Sin((brp.layout.wobbleSpeed.y) * Time.time + phase) + clickedSpeed * brp.layout.clickRotate.y),
                    brp.layout.centerRotate.z + brp.layout.wobbleAmplitude.z * (Mathf.Sin((brp.layout.wobbleSpeed.z) * Time.time + phase) + clickedSpeed * brp.layout.clickRotate.z)   );

                currentState.Apply(go);

                if (isActive)
                    currentState.Near(targetState, 0.9f);
                else
                    currentState.Near(initialState, 0.995f);

            }

        }


    }

}
