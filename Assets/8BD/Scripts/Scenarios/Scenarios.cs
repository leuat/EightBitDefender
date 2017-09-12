using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    [System.Serializable]
    public class SerializedCRTSettings
    {
        [Range(0, 1024)]
        public float pixels = 320f;
        [Range(-1, 1)]
        public float radialDistort = 1.0f;
        [Range(0, 0.5f)]
        public float vignetteDistance = 0.2f;
        [Range(0, 1)]
        public float tintStrength = 0.5f;
        [Range(0, 1)]
        public float pixelBlend = 0.5f;
        [Range(0, 4)]
        public float gamma = 1f;
        [Range(-1, 1)]
        public float correction = 0;
        [Range(0, 1)]
        public float tint_r;
        [Range(0, 1)]
        public float tint_g;
        [Range(0, 1)]
        public float tint_b;
        public string settings_id;


        public Color tint()
        {
            return new Color(tint_r, tint_g, tint_b);
        }

        //    [Range(0, 1)]
    }

    [System.Serializable]
    public class MonsterScenario
    {
        public string monster;
        public bool capturable = false;
        // What level to capture monster
        public int captureLevel = 10;
        // Start base
        public int baseWaveCount = 2;

        // Per level
        public int baseWaveIncrease = 2;
        public float levelScale = 1.2f;

    }


    [System.Serializable]
    public class SerializedScenario
    {
        public string name;
        public string introTexture;
        public string crtSettings_id;
        public List<string> backgrounds = new List<string>();
        public List<MonsterScenario> monsters = new List<MonsterScenario>();
        public string centerImage;
        public string music;

    }

    [System.Serializable]
    public class Scenarios {

    }


/*    public class Scenario
    {
        public SerializedScenario serializedScenario;        
        public 

    }
    */

}
