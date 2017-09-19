using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;


namespace LemonSpawn
{

    [System.Serializable]
    public class MapItem
    {
        public int id = 1;
        public string command = "";
        public string text = "";

        [System.NonSerialized]
        float time = 0;
        [System.NonSerialized]
        int curFrame = 0;
        [System.NonSerialized]
        public Color color = Color.white;

        public MapItem(int pid)
        {
            id = pid;
            UpdateCategory();
        }

        public void UpdateCategory()
        {
            color = Color.white;
            category = SerializedMapCategories.mapCategories.get(id);
            getSprite();
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
            public GameObject go;
            public SpriteRenderer sr;
            public void Create(GameObject parent, Vector3 pos, float dx, float dy, float ls)
            {
                go = new GameObject();
                go.transform.parent = parent.transform;
                go.transform.position = pos;
                go.transform.localScale = new Vector3(dx, dy, 1)*ls;
                sr = go.AddComponent<SpriteRenderer>();
                //           dmi.bsr.size = Vector3.one*ls;
                sr.sprite = null;

            }
        }

        public class DisplayMapCompositeItem
        {
            public DisplayMapItem[] items;
            public DisplayMapCompositeItem()
            {
                items = new DisplayMapItem[3];
                items[0] = new DisplayMapItem();
                items[1] = new DisplayMapItem();
                items[2] = new DisplayMapItem();
            }
            public void setActive(bool a)
            {
                if (items == null)
                    return;
                for (int i = 0; i < items.Length; i++)
                    items[i].go.SetActive(a);
            }
            public void Attach(MapCompositeItem mci)
            {
                if (items == null)
                    return;
                for (int i = 0; i < items.Length; i++) {
                    items[i].sr.sprite = mci.items[i].getSprite();
                    items[i].sr.color = mci.items[i].color;
                }

            }
        }

        public GameObject parent;
        public DisplayMapCompositeItem[,] mapItems;
        public Map2D map;
        int sizeX, sizeY;
        float dx, dy;



        private DisplayMapCompositeItem Initialize(int x, int y)
        {
            DisplayMapCompositeItem dmi = new DisplayMapCompositeItem();

            float ls = 1 / Sprites.Get("Tiles/grass_test1").bounds.size.x;

            dmi.items[0].Create(parent, new Vector3((x - sizeX / 2) * dx, (y - sizeY / 2) * dy, 0), dx, dy, ls);

            dmi.items[1].Create(parent, new Vector3((x - sizeX / 2) * dx, (y - sizeY / 2) * dy, -0.005f), dx, dy, ls);

            dmi.items[2].Create(parent, new Vector3((x - sizeX / 2) * dx, (y - sizeY / 2) * dy, -0.01f), dx, dy, ls);

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
            
            mapItems = new DisplayMapCompositeItem[sizeX, sizeY];
            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    mapItems[i, j] = Initialize(i, j);
                }
        }

        public void Destroy()
        {
            if (parent != null)
                GameObject.Destroy(parent);
        }
        

        public void Update(float ci, float cj)
        {
            parent.transform.position = new Vector3(-(ci - Mathf.Floor(ci)) * dx, -(cj - Mathf.Floor(cj)) * dy, 0);

            for (int i = 0; i < sizeX; i++)
                for (int j = 0; j < sizeY; j++)
                {
                    int pi = i + (int)ci - sizeX / 2;
                    int pj = j + (int)cj - sizeY / 2;
                    DisplayMapCompositeItem dmi = mapItems[i, j];

                    MapCompositeItem mi = map.get(pi, pj);

                    if (mi == null)
                        dmi.setActive(false);

                    else
                    {
                        dmi.setActive(true);

                        dmi.Attach(mi);


/*                        if (mi.background.id == 0)
                            dmi.bgo.SetActive(false);
                        if (mi.foreground.id == 0)
                            dmi.fgo.SetActive(false);
                            */
                    }
                }

        }


    }



    [System.Serializable]
    public class MapCompositeItem
    {
        public static int BACKGROUND = 0;
        public static int MIDDLE = 1;
        public static int FOREGROUND = 2;
        public MapItem[] items = null;
        public MapCompositeItem(int a, int b, int c)
        {
            items = new MapItem[3];
            items[BACKGROUND] = new MapItem(a);
            items[MIDDLE] = new MapItem(b);
            items[FOREGROUND] = new MapItem(c);
        }
        public void UpdateCategories()
        {
            for (int i = 0; i < items.Length; i++)
                if (items[i] != null)
                    items[i].UpdateCategory();

        }
    }

    [System.Serializable]
    public class Map2D //: MonoBehaviour
    {

        public int sizeX = 16, sizeY = 16;
        public string name = "";
        public string fileName = "";
        public MapCompositeItem[] map = null;

        static string ebd = "/8BD/Resources/Maps/";


        private void UpdateCategories()
        {
            for (int i = 0; i < sizeX * sizeY; i++)
            {
                map[i].UpdateCategories();
            }

        }



        public static void Save(Map2D saveGame, string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.dataPath + ebd + fileName + ".bytes"); //you can call it anything you want
            bf.Serialize(file, saveGame);
            file.Close();
        }

        public static Map2D Load(string fileName)
        {
            TextAsset asset = Resources.Load("Maps/" + fileName) as TextAsset;
            Stream s = new MemoryStream(asset.bytes);

            BinaryFormatter bf = new BinaryFormatter();
            Map2D m = (Map2D)bf.Deserialize(s);
            m.UpdateCategories();
//            Debug.Log(m.sizeX);
            return m;
        }


        public void Create(int X, int Y)
        {
            sizeX = X;
            sizeY = Y;
            map = new MapCompositeItem[sizeX * sizeY];
            int x = 0;
            int y = 0;
            for (int i = 0; i < sizeX * sizeY; i++)
            {
                int fg = 0;
                if (Random.value > 0.8)
                    fg = 3;
                int d = 0;
                float scale = 1f / sizeX * 13f;
                if (Mathf.PerlinNoise(x * scale, y * scale) > 0.5)
                    d = 3;
                map[i] = new MapCompositeItem(Random.Range(1 + d, 3 + d),0,fg);
                x++;
                if (x >= sizeX - 1)
                {
                    x = 0;
                    y++;
                }

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
