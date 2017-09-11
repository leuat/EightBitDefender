using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class Explosion : GenericObject
    {
        private SpriteRenderer spriteRenderer;
        private Sprite[] sprites;
        private string baseName;
        float time = 0;
        float timeScale = 1;
        Vector3 V;
        int maxFrames;

        public Explosion(string n, Vector3 p, float ts, Vector3 v, float scale, int max)
        {
            V = v;
            timeScale = ts;
            pos = p;
            baseName = n;
            maxFrames = max;

            go = new GameObject(n);
            spriteRenderer = go.AddComponent<SpriteRenderer>();
            go.transform.position = pos;
            go.transform.localScale = Vector3.one * scale;

        }

        override public void Update() 
        {
            base.Update();
            if (sprites == null)
                sprites = Sprites.GetASprite(baseName);

            time += Time.deltaTime*timeScale;
            if (time > 1)
                markForDeath = true;

            int i = ((int)(time * maxFrames))%sprites.Length;
            spriteRenderer.sprite = sprites[i];

            go.transform.position += V * Time.deltaTime;


                
        }

    }


    public class Explosions : GenericCollection
    {
  
        public void Add(string n, Vector3 p, float ts, Vector3 v, float scale, int max)
        {
            collection.Add(new LemonSpawn.Explosion(n, p, ts, v, scale, max));
        }


    }
}
