﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace LemonSpawn
{
    [System.Serializable]
    public class BentoMenuItem2D 
    {
        public List<BentoMenuItem2D> children = new List<BentoMenuItem2D>();
//        public Callback callback;
        public Texture2D image;
        public bool childrenHorizontal = false;
        public enum DisplayFormat { ImageOnly, TextOnly, ImageLeft, ImageRight};
        public DisplayFormat displayFormat = DisplayFormat.ImageLeft;
        public bool hidden = false;
        public Color color = Color.gray;
        public Vector2 childrenSize = new Vector2(0.3f, 0.2f);
        public bool childrenAspectFixed = true;
        private static GUIStyle emptyStyle = new GUIStyle();

        public string text;
        public string eventMessage;

        private float hover = 0;
        private static Texture2D background = null;
        private Vector2 pivot;
        private float rotAngle, time;

        public Rect getRect(Vector2 pos)
        {
            return new Rect(0, 0, 0, 0);
            /*
            if (childrenAspectFixed)
                return new Rect(BentoMenu.getX(pos.x - childrenSize.x / 2), BentoMenu.getY(pos.y) - BentoMenu.getX(childrenSize.y / 2),
                                BentoMenu.getX(childrenSize.x), BentoMenu.getX(childrenSize.y));

            return new Rect(BentoMenu.getX(pos.x - hover * childrenSize.x / 2), BentoMenu.getY(pos.y - childrenSize.y / 2),
                            BentoMenu.getX(childrenSize.x), BentoMenu.getY(childrenSize.y));

            */
        }


        private void RenderAll(BentoRenderPacket rp, Rect r, Vector2 scale, float rotPhase, float timeScale)
        {

/*            pivot = new Vector2(r.xMin + r.width * 0.5f, r.yMin + r.height * 0.5f);
            time += Time.deltaTime * timeScale;

            Matrix4x4 matrixBackup = GUI.matrix;
            GUIUtility.ScaleAroundPivot(scale, pivot);
            GUIUtility.RotateAroundPivot(rotAngle, pivot);

            rotAngle = Mathf.Sin(time + rotPhase)*rp.layout.wobbleAmplitude;

            if (background == null)
                background = Util.createSolidTexture(rp.layout.backgroundColor);



            if (rp.layout.background != null)
                GUI.DrawTexture(r, rp.layout.background);

            GUI.DrawTexture(r, background);



            // Decay

            GUI.Label(r, text, rp.layout.GetStyle(color));

            if (r.Contains(Event.current.mousePosition) && hover<=1)
            {
                hover += Time.deltaTime*3;
                if (hover > 1) hover = 1;
            }
            else
            hover *= 0.95f;


            if (GUI.Button(r, "", emptyStyle))
            {
                //performSelect = this;
                hover = 2;
                if (rp.unityEvent != null)
                {
                    BentoMenu.currentMessage = eventMessage;
                    rp.unityEvent.Invoke();
                }
            }
            GUI.matrix = matrixBackup;
            */
        }

        public void Render(BentoRenderPacket rp, Vector2 pos)
        {

            Vector2 dSize = new Vector2(0, childrenSize.y + rp.layout.spacing.y);
            if (childrenHorizontal)
                dSize = new Vector2(childrenSize.x + rp.layout.spacing.x, 0);

            Vector2 cpos = pos;
            foreach (BentoMenuItem2D bmi in children)
            {
                Rect r = getRect(cpos);
                float hoverScale = 1 + bmi.hover * rp.layout.hoverScale;
//                bmi.RenderAll(rp, r, Vector2.one*hoverScale, cpos.y*123.23453f, hoverScale*rp.layout.wobbleSpeed);


                bmi.Render(rp, cpos);
                cpos += dSize;
            }


        }

    }

}
