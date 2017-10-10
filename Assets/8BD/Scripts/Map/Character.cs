using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class Character
    {
        SerializedCharacterInstance sz;

        public GameObject go;
        public SpriteRenderer sr;
        private float moveto_pos_x, moveto_pos_y, time;
        private float dt = 1f;
        public Character(SerializedCharacterInstance src)
        {
            sz = src;
            go = new GameObject(sz.getCharacter().name.get());
            sr = go.AddComponent<SpriteRenderer>();

            sr.sprite = Sprites.GetASprite(sz.getCharacter().moveTexture)[0];
         //   go.transform.position

        }
        public void Update(DisplayMap dmap)
        {
            go.transform.position = dmap.getRealPosition((int)sz.pos_x, (int)sz.pos_y, -DisplayMap.Z_CHARS);
        }


    }


    public class Characters
    {
        public List<Character> characters = new List<Character>();

        public Characters(SerializedGameLevel lvl)
        {
            characters.Clear();
            foreach (SerializedCharacterInstance sc in lvl.characters)
                characters.Add(new Character(sc));
            //characters = lvl.characters;
        }
        public void Update(DisplayMap dmap)
        {   
            foreach (Character c in characters)
            {
                c.Update(dmap);
            }
        }



    }

}