using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LemonSpawn
{

    public class InstantiateQueueItem
    {
        public string type;
        public Vector3 pos, dir;
        public Entity.Teams team;
        public float overrideMaxDistance;

        public InstantiateQueueItem(string t, Vector3 p, Vector3 d, Entity.Teams tm, float omx)
        {
            type = t;
            pos = p;
            dir = d;
            team = tm;
            overrideMaxDistance = omx;
        }

    }

    public class EntityGrid
    {
        public int sizex, sizey;
        public float gridSize;
        public List<Entity>[,] grid;// = new float[1,1];

        public EntityGrid(float gs)
        {
            gridSize = gs;
        }

        private void Clear()
        {
            for (int i = 0; i < sizex; i++)
                for (int j = 0; j < sizey; j++)
                    grid[i, j].Clear();
        }


        public void DebugGridLines()
        {
            float dx = (max.x - min.x) / sizex;
            float dy = (max.y - min.y) / sizey;
            for (int i = 0; i < sizex; i++)
            {
                Vector3 p1 = new Vector3(dx * i + min.x, min.y, 0);
                Vector3 p2 = new Vector3(dx * i + min.x, max.y, 0);
                Debug.DrawLine(p1, p2);
            }
            for (int i = 0; i < sizey; i++)
            {
                Vector3 p1 = new Vector3(min.x, dy * i + min.y, 0);
                Vector3 p2 = new Vector3(max.x, dy * i + min.y, 0);
                Debug.DrawLine(p1, p2);
            }
        }

        Vector2 min = new Vector2(1E20f, 1E20f);
        Vector2 max = new Vector2(-1E20f, -1E20f);

        public void InsertCollection(EntityCollection ec, float gs)
        {
            gridSize = gs;
            if (ec.entities.Count < 2)
                return;

            min = new Vector2(1E20f, 1E20f);
            max = new Vector2(-1E20f, -1E20f);
            foreach (Entity e in ec.entities)
            {
                min.x = Mathf.Min(min.x, e.particle.P.x);
                min.y = Mathf.Min(min.y, e.particle.P.y);
                max.x = Mathf.Max(max.x, e.particle.P.x);
                max.y = Mathf.Max(max.y, e.particle.P.y);
            }

            sizex = (int)((max.x - min.x) / gridSize);
            sizey = (int)((max.y - min.y) / gridSize);

            if (sizex == 0 || sizey == 0)
                return;


            grid = new List<Entity>[sizex, sizey];
            for (int i = 0; i < sizex; i++)
                for (int j = 0; j < sizey; j++)
                    grid[i, j] = new List<Entity>();


            foreach (Entity e in ec.entities)
            {
                int i = Mathf.Clamp((int)((e.particle.P.x - min.x) / (max.x - min.x) * sizex), 0, sizex - 1);
                int j = Mathf.Clamp((int)((e.particle.P.y - min.y) / (max.y - min.y) * sizey), 0, sizey - 1);

                grid[i, j].Add(e);
            }



        }
        public List<Entity> getNbhList(Entity e, int w)
        {
            List<Entity> nbh = new List<Entity>();

            int i = Mathf.Clamp((int)((e.particle.P.x - min.x) / (max.x - min.x) * sizex), 0, sizex - 1);
            int j = Mathf.Clamp((int)((e.particle.P.y - min.y) / (max.y - min.y) * sizey), 0, sizey - 1);


            for (int dx = -w; dx <= w; dx++)
                for (int dy = -w; dy <= w; dy++)
                {
                    int x = dx + i;
                    int y = dy + j;
                    if (x >= 0 && x < sizex && y >= 0 && y < sizey)
                        nbh.AddRange(grid[x, y]);
                }



            return nbh;
        }



    }


    public class EntityCollection
    {
        public List<Entity> entities = new List<Entity>();
        //        public List<Entity>  = new List<Entity>();
        EntityGrid entityGrid = new EntityGrid(1);
        Dictionary<string, Transform> prefabs = new Dictionary<string, Transform>();

        List<InstantiateQueueItem> queuedItems = new List<InstantiateQueueItem>();
        List<Entity> deathList = new List<Entity>();


        public void Instantiate(string type, Vector3 pos, Entity.Teams team, Vector3 dir, float overrideMaxDistance)
        {
            queuedItems.Add(new InstantiateQueueItem(type, pos, dir, team, overrideMaxDistance));
        }


        public void InstantiateNow(string type, Vector3 pos, Entity.Teams team, Vector3 dir, float overrideMaxDistance)
        {

            bool isNewType = !prefabs.ContainsKey(type);
            Entity e = null;
            if (isNewType)
            {
                GameObject go = new GameObject(type);
                e = go.AddComponent<Entity>();
                e.Initialize(SerializedEntities.se.Get(type), pos, team, dir);
                prefabs.Add(type, go.transform);
                go.SetActive(false);
            }
            else
            {

                GameObject go = GameObject.Instantiate(prefabs[type], pos, Quaternion.identity).gameObject;
                go.SetActive(true);
                e = go.GetComponent<Entity>();
                e.InitializePrefab(SerializedEntities.se.Get(type), pos, team, dir);

                if (overrideMaxDistance > 0)
                    e.serializedEntity.maxDistance = overrideMaxDistance;
                entities.Add(e);
            }

        }

        public void DestroyAll()
        {
            queuedItems.Clear();
            foreach (Entity e in entities)
                GameObject.Destroy(e.gameObject);

            entities.Clear();
        }


        public void MaintainPopulation()
        {
            // First kill off
            deathList.Clear();

            foreach (Entity e in entities)
                if (e.killObject)
                    deathList.Add(e);


            foreach (Entity e in deathList)
            {
                GameObject.Destroy(e.gameObject);
                entities.Remove(e);
            }

            foreach (InstantiateQueueItem qi in queuedItems)
                InstantiateNow(qi.type, qi.pos, qi.team, qi.dir, qi.overrideMaxDistance);

            queuedItems.Clear();

        }




        public void UpdateCollectionN2()
        {
            foreach (Entity a in entities)
                a.UpdateBody(this);

        }

        private void CoreN2(Entity a)
        {
            foreach (Entity b in entities)
            {
                if (a != b)
                    a.Affect(b);
            }
        }

        private float CoreGrid(Entity a)
        {
            List<Entity> nbh = entityGrid.getNbhList(a, 1);
            foreach (Entity b in nbh)
                a.Affect(b);

            return nbh.Count;
        }

        private float CoreSphere(Entity a)
        {
            Vector2 p = new Vector2(a.particle.P.x, a.particle.P.y);
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(p, 2);
            int i = 0;
            while (i < hitColliders.Length && i < 25)
            {
                a.Affect(hitColliders[i].gameObject.transform.parent.gameObject.GetComponent<Entity>());
                i++;
            }
            return i;
        }



        public void UpdateCollectionGrid()
        {
            float avg = 0;
            foreach (Entity a in entities)
            {
                //Debug.Log(a.body);

                a.UpdateBody(this);

                if (a.target == null && a.serializedEntity.homing == true)
                    CoreN2(a);
                else
                    avg += CoreGrid(a);
            }


            avg /= entities.Count;
            //    Debug.Log("avg: " + avg);
        }

        public void UpdateCollectionSphere()
        {
            float avg = 0;
            foreach (Entity a in entities)
            {
                a.UpdateBody(this);

                avg += CoreSphere(a);
            }
            avg /= entities.Count;
                Debug.Log("avg: " + avg);
        }

        private void UpdateGrid()
        {
             entityGrid.InsertCollection(this, 1);
            //            entityGrid.DebugGridLines();
            UpdateCollectionGrid();
        }



        public void Update()
        {
            MaintainPopulation();
            UpdateGrid();
//            UpdateCollectionSphere();
//            Debug.Log(entities.Count);
        }



    }

}