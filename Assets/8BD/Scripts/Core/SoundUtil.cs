using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class SoundUtil
    {

        public static Dictionary<string, AudioClip> clips = new Dictionary<string, AudioClip>();

        private static AudioSource source = null;

        public static AudioClip GetSound(string s)
        {
            if (clips.ContainsKey(s))
                return clips[s];

            AudioClip cl = (AudioClip)Resources.Load("Music/" + s);
            if (cl == null)
                Debug.Log("Error: Could not find sound " + s);
            if (cl != null)
                clips.Add(s, cl);
            return cl;
        }

        public static void PlaySound(string s, float v)
        {
            if (source == null)
                source = GameObject.Find("Audio Source").GetComponent<AudioSource>();

            AudioClip ac = GetSound(s);
            if (ac!=null)
                source.PlayOneShot(ac, v);
                
            
        }

    }
}


