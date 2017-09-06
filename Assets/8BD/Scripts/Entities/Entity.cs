﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace LemonSpawn
{



    public class ParameterModifier
    {

    }


    public class Parameter
    {
        public string name, displayName;
        public Vector3 value = new Vector3(0, 0, 0); // val, min, max



        public void Randomize(System.Random rnd)
        {
            value.x = (float)(rnd.NextDouble() * (value.z - value.y) + value.y);
        }

        public Parameter(string n, string dn, float v)
        {
            value.x = v;
            name = n;
            displayName = dn;
        }

        public Parameter(string n, string dn, float v, float min, float max)
        {
            value.x = v;
            value.y = min;
            value.z = max;
            name = n;
            displayName = dn;
        }

    }

    public class ParameterList
    {
        List<Parameter> parameters = new List<Parameter>();

        public void Add(Parameter p)
        {
            parameters.Add(p);
        }

        public Parameter GetParameter(string parameter)
        {
            foreach (Parameter p in parameters)
                if (p.name == parameter)
                    return p;

            return null;
        }


    }


    public class EntityBodyPart
    {
        public GameObject go = null;
        public Material material;
        public Vector3 displacement = Vector3.zero;
        public SerializedBodyPart serializedBodyPart;
        public List<EntityBodyPart> bodyparts = new List<EntityBodyPart>();
        public Vector3 direction = new Vector3(0, 1, 0);

/*        public void Move(Particle p)
        {
            p.A = p.A + p.Dir * serializedBodyPart.moveSpeed;
        }
        */

/*        public EntityBodyPart(Transform parent, SerializedBodyPart bp) {
            serializedBodyPart = bp;

            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.parent = parent;
            go.transform.localScale = bp.scale * Vector3.one;

            material = new Material(GameSettings.BillboardMaterial.shader);
            material.CopyPropertiesFromMaterial(GameSettings.BillboardMaterial);
            material.SetTexture("_MainTex", (Texture2D)Resources.Load("Textures/" + bp.texture));
            go.GetComponent<Renderer>().material = material;

            foreach (SerializedBodyPart sbp in bp.bodyParts)
                bodyparts.Add(new EntityBodyPart(go.transform, sbp));



        }
        */

        public void Update(Entity me, EntityCollection ec)
        {
            if (serializedBodyPart.weapon != null)
                serializedBodyPart.weapon.Update(me, ec);


            Vector3 dir = me.particle.V;

            if (!serializedBodyPart.directionFromSpeed) {
                dir = Vector3.up;
                if (me.target != null)
                {
                    float t = serializedBodyPart.rotateSpeed;
                    direction = direction * t + (1 - t) * me.directionTo(me.target);
                    dir = direction;
                }
            }

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (go!=null)
                go.transform.rotation = Quaternion.AngleAxis(angle + serializedBodyPart.rotateTexture, Vector3.forward   );



            foreach (EntityBodyPart bp in bodyparts)
                bp.Update(me, ec);
        }

        public EntityBodyPart(Transform parent, SerializedBodyPart bp, bool hasCollider)
        {
            serializedBodyPart = bp;

            go = new GameObject();
            go.transform.parent = parent;
            go.transform.localScale = bp.scale * Vector3.one;
            
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingLayerName = "Foreground";
            sr.sortingLayerID = 9;
            sr.color = Color.white;
            sr.sprite = Sprites.Get(bp.texture);
            if (hasCollider)
                go.AddComponent<BoxCollider2D>();
  //          go.AddComponent<Rigidbody2D>();
            Physics2D.gravity = Vector2.zero;
            foreach (SerializedBodyPart sbp in bp.bodyParts)
                bodyparts.Add(new EntityBodyPart(go.transform, sbp, false));



        }

        public EntityBodyPart(Transform parent, SerializedBodyPart bp)
        {
            serializedBodyPart = bp;
            go = parent.GetChild(0).gameObject;
            foreach (SerializedBodyPart sbp in bp.bodyParts)
                bodyparts.Add(new EntityBodyPart(go.transform, sbp));



        }

    }




    public class Entity : MonoBehaviour
    {

        public SerializedEntity serializedEntity;
        public EntityBodyPart body;
        public Particle particle = new Particle();
        public Entity currentTarget = null;
        public bool markDie = false;
        public enum Teams { Player1, Player2, AI1, AI2 };
        public Teams team = Teams.Player1;
        public Entity target = null;



        public void Initialize(SerializedEntity se, Vector3 pos, Teams t, Vector3 dir)
        {
            serializedEntity = se.Copy();//Util.DeepClone<SerializedEntity>(se);
            body = new EntityBodyPart(transform, serializedEntity.body, true);
            team = t;
            particle.P = pos;
            particle.V = dir;

        }

        public void InitializePrefab(SerializedEntity se, Vector3 pos, Teams t, Vector3 dir)
        {
            serializedEntity = se.Copy();//Util.DeepClone<SerializedEntity>(se);
            //serializedEntity = e.serializedEntity.Copy();
           
            body = new EntityBodyPart(transform, serializedEntity.body);
            team = t;
            particle.P = pos;
            particle.V = dir;

        }

        // Use this for initialization
        void Start()
        {
            particle.P = transform.position;
        }

        
         
        void MarkDie()
        {

            markDie = true;
        }

/*        void Destroy()
        {
            GameObject.DestroyImmediate(this);
        }
        */

        public float distanceTo(Entity o)
        {
            return (particle.P - o.particle.P).magnitude;
        }
        public Vector3 directionTo(Entity o)
        {
            return (particle.P - o.particle.P).normalized;
        }


        public void AnalyzeTarget(Entity o)
        {
            if (o  == null)
                return;
            if (!o.serializedEntity.isTargetable)
                return;
            if (o.team == team)
                return;

            if (target==null)
            {
                target = o;
                return;
            }
            // if distance is lower then select target
            if (distanceTo(o)<distanceTo(target)) {
                target = o;
            }
            
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            ValidateExistence();
        }

        void ValidateExistence()
        {
            if (serializedEntity.maxDistance>0) {
                if (particle.travelDistance > serializedEntity.maxDistance)
                    MarkDie();
            }
//            if (particle.travelDistance>)
        }

        
        public void Hit(float damage)
        {
            serializedEntity.health -= damage;
            if (serializedEntity.health < 0)
                MarkDie();
        }

        public void Damage(Entity other)
        {
            if (other == null)
                return;
            // Cannot hurt own team
            if (other.team == team)
                return; ;
            if (serializedEntity.maxDamage <= 0)
                return;
            if (distanceTo(other)<serializedEntity.detonateDistance)
            {
                other.Hit(serializedEntity.maxDamage);
                MarkDie();
            }
        }

        public void Affect(Entity other)
        {

            if (serializedEntity.homing)
            {
                if (other == null)
                    if (Random.value > 0.99)
                        Debug.Log("OTHER NULL");
                AnalyzeTarget(other);
                MoveAction(other);
            }
            Damage(other);

        }


        public void RenderGUI()
        {

        }


        public void MoveAction(Entity t)
        {
            if (t == null)
                return;
            particle.Avoid(t.particle);

        }



        public void Move()
        {
            if (target != null)
                particle.MoveTowards(target.particle);

            if (particle.V.magnitude < serializedEntity.body.minSpeed)
                particle.V = particle.V.normalized * serializedEntity.body.minSpeed;

            particle.Move();

            if (body!=null)
                particle.CapSpeed(body.serializedBodyPart.moveSpeed);
            transform.position = particle.P;

    }


    }
}