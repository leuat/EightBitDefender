using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace LemonSpawn
{

    class LSprite
    {
        public Sprite sprite;
        public string name;
    }

    class Sprites
    {
        public static List<LSprite> sprites = new List<LSprite>();
        public static Sprite Get(string name)
        {
            foreach (LSprite s in sprites)
                if (s.name == name)
                    return s.sprite;

            LSprite ls = new LSprite();
            ls.name = name;
            Texture2D t = (Texture2D)Resources.Load("Textures/" + name);
            ls.sprite = Sprite.Create(t,
                new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));

            sprites.Add(ls);
            return ls.sprite;

        }
    }


    class Util
    {

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }

        }
    }

}