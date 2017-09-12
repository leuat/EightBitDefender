using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LemonSpawn
{

    public class GenericObject
    {
        public GameObject go;
        public Vector3 pos;
        public bool markForDeath = false;


        public void Die()
        {
            markForDeath = true;
        }

        public void Destroy()
        {
            if (go != null)
                GameObject.Destroy(go);
            go = null;
        }

        virtual public void Update()
        {

        }

    }


    public class GenericCollection
    {
        protected List<GenericObject> collection = new List<GenericObject>();


        public void Update()
        {
            List<GenericObject> deathList = new List<GenericObject>();
            foreach (GenericObject e in collection)
                if (e.markForDeath)
                    deathList.Add(e);

            foreach (GenericObject e in deathList)
            {
                e.Destroy();
                collection.Remove(e);
            }

            foreach (GenericObject e in collection)
                e.Update();

        }

        public void DestroyAll()
        {
            foreach (GenericObject e in collection)
                GameObject.Destroy(e.go);

            collection.Clear();
        }


    }
}
