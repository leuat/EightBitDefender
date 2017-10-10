using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;


namespace LemonSpawn
{

    [System.Serializable]
    public class Language
    {
        public int id;
        public string language;
    }



    [System.Serializable]
    public class LString
    {
        public List<string> text = new List<string>();
        public string get()
        {
            if (GameSettings.language >= text.Count)
                return " [ text not found for language " + GameSettings.language + " ]";
            else
                return text[GameSettings.language];

        }

    }

    [System.Serializable]
    public class CharacterText
    {
        public LString text = new LString();
        public string character_id;
        
    }

    [System.Serializable]
    public class SerializedCharacter
    {
        public string texture;
        public string iconTexture;
        public string moveTexture;
        public string character_id;
        public LString name = new LString();
        public string font;
        public string fcolor, bcolor;
    }

    [System.Serializable]
    public class SerializedScene
    {
        public string scene_id;
        public string music;
        public string char1_id, char2_id;
        public string background;
        public string CRTSettings_id = "";
        public List<CharacterText> texts = new List<CharacterText>();
    }


    [System.Serializable]
    public class SerializedScenes
    {
        public List<SerializedScene> scenes = new List<SerializedScene>();
        public List<SerializedCharacter> characters = new List<SerializedCharacter>();
        public List<SerializedScenario> scenarios = new List<SerializedScenario>();
        public List<SerializedCRTSettings> CRTSettings = new List<SerializedCRTSettings>();


        public SerializedScene getScene(string n)
        {
            foreach (SerializedScene sc in scenes)
                if (sc.scene_id == n)
                    return sc;

            return null;
        }


        public SerializedCharacter getCharacter(string n)
        {
            foreach (SerializedCharacter sc in characters)
                if (sc.character_id == n)
                    return sc;

            return null;
        }
        public SerializedCRTSettings getCRTSettings(string n)
        {
            foreach (SerializedCRTSettings sc in CRTSettings)
                if (sc.settings_id == n)
                    return sc;

            return null;
        }



        public static SerializedScenes DeSerialize(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializedScenes));

            TextAsset textAsset = (TextAsset)Resources.Load(filename);
            TextReader textReader = new StringReader(textAsset.text);
            SerializedScenes sz = (SerializedScenes)deserializer.Deserialize(textReader);
            textReader.Close();
            return sz;
        }


        public static SerializedScenes szScenes;


    }


}