using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class GameFightLevel
    {
//        public Scenario scenario = new Scenario();

        public static EntityCollection entities = new EntityCollection();
        public static Explosions explosions = new Explosions();
        public static Messages messages = new Messages();

        public static void DestroyAll()
        {
            entities.DestroyAll();
            messages.DestroyAll();
            explosions.DestroyAll();
        }


        public GameFightLevel()
        {
        }

        public void Initialize()
        {
            messages.Initialize((Font)Resources.Load("Fonts/ModerDos"), GameObject.Find("Canvas"), 18, GameObject.Find("Main Camera").GetComponent<Camera>());

        }


        public void Update()
        {
            entities.Update();
            explosions.Update();
            messages.Update();
        }

        public void Test(int count)
        {
            System.Random rnd = new System.Random();
            if (entities.entities.Count < count)
                for (int i = 0; i < 1; i++)
                {
                    float s = 20;
                    float d1 = (float)rnd.NextDouble() * s - s / 2;
                    float d2 = (float)rnd.NextDouble() * s - s / 2;

                    Entity.Teams t = Entity.Teams.AI1;
                    if (rnd.NextDouble() > 0.5)
                        t = Entity.Teams.AI2;

                    if ((float)rnd.NextDouble() > 0.5)
                        entities.Instantiate("Sopwith", new Vector3(d1, d2, 0), t, Vector3.zero, -1);
                    else
                        entities.Instantiate("ParaTrooper", new Vector3(d1, d2, 0), t, Vector3.zero, -1);
                }

        }


    }

}
