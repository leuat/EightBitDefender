using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;



namespace LemonSpawn
{

    [System.Serializable]
    public class SerializedGameLevel
    {
        public string mapName;
        public string name;
        public string crtSettings_id;

    }


    public class GameLevel
    {
        public SerializedGameLevel sz;
        public Map2D map;
        public DisplayMap dMap = new DisplayMap();
        private CRTScreen crtScreen;

        public GameLevel(SerializedGameLevel s)
        {
            sz = s;
            Initialize();
        }

        private void Test()
        {
            map = new Map2D();
            map.Create(32, 20);
            Map2D.Save(map, "test_map1");

        }

        public void Destroy()
        {
            dMap.Destroy();

        }

        public void Initialize()
        {
//            Test();
            map = Map2D.Load(sz.mapName);
            dMap.Initialize(map, 30, 20, 1, 1);
            GameObject ec = GameObject.Find("EffectsCamera");
            if (ec != null)
            {
                crtScreen = ec.GetComponent<CRTScreen>();
                crtScreen.settings = SerializedScenes.szScenes.getCRTSettings(sz.crtSettings_id);
            }
        }

        public void Update()
        {
             
        }

    }


    [System.Serializable]
    public class SerializedGameLevels
    {
        public List<SerializedGameLevel> levels = new List<SerializedGameLevel>();

        [System.NonSerialized]
        public static SerializedGameLevels gameLevels;
        
        public SerializedGameLevels()
        {

        }

        public static GameLevel getLevel(string name)
        {
            foreach (SerializedGameLevel sz in gameLevels.levels)
            {
                if (sz.name == name)
                {
                    GameLevel gl = new LemonSpawn.GameLevel(sz);
                    return gl;
                }
            }
            return null;
        }

        public static SerializedGameLevels DeSerialize(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializedGameLevels));

            TextAsset textAsset = (TextAsset)Resources.Load(filename);
            TextReader textReader = new StringReader(textAsset.text);
            SerializedGameLevels sz = (SerializedGameLevels)deserializer.Deserialize(textReader);
            textReader.Close();
            return sz;
        }

        static public void Serialize(SerializedGameLevels sz, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializedGameLevels));
            TextWriter textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, sz);
            textWriter.Close();
        }


    }
}
