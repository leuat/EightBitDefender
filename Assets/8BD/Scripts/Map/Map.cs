using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


namespace LemonSpawn
{

    [System.Serializable]
    public class MapItem
    {
        public int id = 1;
        float time = 0;
        int curFrame = 0;
        public Color color = Color.white;
        public MapItem(int pid)
        {
            id = pid;
            UpdateCategory();
        }

        public void UpdateCategory()
        {
            category = SerializedMapCategories.mapCategories.get(id);

        }

        public void newId(int newId)
        {
            id = newId;
            UpdateCategory();
            getSprite();
        }

        public Sprite getSprite()
        {
            if (id == 0)
                return null;
            if (category == null)
                return null;
            if (category.sprites == null)
                return null;
            return category.sprites[curFrame];
        }

        public void Update()
        {
            time += Time.deltaTime;
            if (time > category.animSpeed)
            {
                time = 0;
                curFrame = (curFrame + 1) % category.noFrames;
            }
        }

        [System.NonSerialized]
        public MapCategory category;
    }

    public class DisplayMap
    {
        public class DisplayMapItem
        {
            public GameObject fgo;
            public SpriteRenderer fsr;
            public GameObject bgo;
            public SpriteRenderer bsr;
        }

        public GameObject parent;
        public DisplayMapItem[,] mapItems;
        public Map2D map;
        int sizeX, sizeY;
        float dx, dy;


        private DisplayMapItem Initialize(int x, int y)
        {
            DisplayMapItem dmi = new DisplayMapItem();

            float ls = 1 / Sprites.Get("Tiles/grass_test1").bounds.size.x;

            dmi.bgo = new GameObject();
            dmi.bgo.transform.parent = parent.transform;
            dmi.bgo.transform.position = new Vector3((x - sizeX / 2) * dx, (y - sizeY / 2) * dy, 0);
            dmi.bgo.transform.localScale = new Vector3(dx, dy, 1) * ls;
            dmi.bsr = dmi.bgo.AddComponent<SpriteRenderer>();
 //           dmi.bsr.size = Vector3.one*ls;
            dmi.bsr.sprite = null;

            dmi.fgo = new GameObject();
            dmi.fgo.transform.parent = parent.transform;
            dmi.fgo.transform.position = new Vector3((x - sizeX / 2) * dx, (y - sizeY / 2) * dy, -0.01f);
            dmi.fgo.transform.localScale = new Vector3(dx, dy, 1) * ls;
            dmi.fsr = dmi.fgo.AddComponent<SpriteRenderer>();
    //        dmi.fsr.size = Vector3.one;
            dmi.fsr.sprite = null;



            return dmi;
        }

        public void Initialize(Map2D m, int x, int y, float sx, float sy)
        {

            map = m;
            sizeX = x;
            sizeY = y;
            dx = sx;
            dy = sy;

            parent = new GameObject("Map");

            mapItems = new DisplayMapItem[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    mapItems[i, j] = Initialize(i, j);
                }
        }

        public void Update(float ci, float cj)
        {
            parent.transform.position = new Vector3(-(ci - Mathf.Floor(ci)) * dx, -(cj - Mathf.Floor(cj)) * dy, 0);

            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    int pi = i + (int)ci - sizeX / 2;
                    int pj = j + (int)cj - sizeY / 2;
                    DisplayMapItem dmi = mapItems[i, j];

                    MapCompositeItem mi = map.get(pi, pj);

                    if (mi == null)
                    {
                        dmi.fgo.SetActive(false);
                        dmi.bgo.SetActive(false);
                    }

                    else
                    {
                        dmi.fgo.SetActive(true);
                        dmi.bgo.SetActive(true);
                        dmi.bsr.sprite = mi.background.getSprite();
                        dmi.fsr.sprite = mi.foreground.getSprite();
                        dmi.bsr.color = mi.background.color;
                        dmi.fsr.color = mi.foreground.color;


                        /*                        if (dmi.bsr.sprite != null)
                                                {
                                                    float factor = 1 * dx / dmi.bsr.bounds.size.x;
                                                    dmi.bgo.transform.localScale = new Vector3(factor, factor, factor);
                                                }
                                                if (dmi.fsr.sprite != null)
                                                {
                                                    float factor = 1 * dx / dmi.fsr.bounds.size.x;
                                                    dmi.fgo.transform.localScale = new Vector3(factor, factor, factor);
                                                }
                                                */
                        if (mi.background.id == 0)
                            dmi.bgo.SetActive(false);
                        if (mi.foreground.id == 0)
                            dmi.fgo.SetActive(false);

                    }
                }

        }


    }




public class MapCompositeItem
{
    public MapItem foreground;
    public MapItem background;
    public MapCompositeItem(int a, int b)
    {
        foreground = new LemonSpawn.MapItem(a);
        background = new LemonSpawn.MapItem(b);
    }
}


public class Map2D //: MonoBehaviour
{

    public int sizeX = 256, sizeY = 256;
    private MapCompositeItem[] map = null;



    /*        public static void Save(SerializedMap2D saveGame)
            {

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Create("C:/Savegames/" + saveGame.savegameName + ".sav"); //you can call it anything you want
                bf.Serialize(file, saveGame);
                file.Close();
                Debug.Log("Saved Game: " + saveGame.savegameName);

            }
            */


    public void Create(int X, int Y)
    {
        sizeX = X;
        sizeY = Y;
        map = new MapCompositeItem[sizeX * sizeY];
        for (int i = 0; i < sizeX * sizeY; i++)
        {
            int fg = 0;
            if (Random.value > 0.8)
                fg = 3;
            map[i] = new MapCompositeItem(fg, Random.Range(1, 3));
        }
    }

    public MapCompositeItem get(int i, int j)
    {
        if (map == null)
            return null;

        if (i >= 0 && i < sizeX && j >= 0 && j < sizeY)
            return map[i + sizeY * j];

        return null;
    }


}

}
