using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class Explosion
    {
        private GameObject go;
        private SpriteRenderer spriteRenderer;
        private Sprite[] sprites;
        private string baseName;
        private Vector3 pos;
        float time = 0;
        float timeScale = 1;
        Vector3 V;
        public bool markForDeath = false;
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

        public void Die()
        {
            markForDeath = true;
        }

        public void Update()
        {
            if (sprites == null)
                sprites = Sprites.GetASprite(baseName);

            Debug.Log(sprites);


            time += Time.deltaTime*timeScale;
            if (time > 1)
                markForDeath = true;

            int i = ((int)(time * maxFrames))%sprites.Length;
            spriteRenderer.sprite = sprites[i];

            go.transform.position += V * Time.deltaTime;


                
        }

    }


    public class Explosions
    {
        List<Explosion> explosions = new List<Explosion>();

        public void Add(string n, Vector3 p, float ts, Vector3 v, float scale, int max)
        {
            explosions.Add(new LemonSpawn.Explosion(n, p, ts, v, scale, max));
        }

        public void Update()
        {
            List<Explosion> deathList = new List<Explosion>();
            foreach (Explosion e in explosions)
                if (e.markForDeath)
                    deathList.Add(e);

            foreach (Explosion e in deathList)
                explosions.Remove(e);

            foreach (Explosion e in explosions)
                e.Update();

        }


    }
}
