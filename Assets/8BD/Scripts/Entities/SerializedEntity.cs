using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace LemonSpawn
{

    [System.Serializable]
    public class Weapon
    {
        public string entityWeapon;
        public float reloadSpeed = 1;
        public bool autoShoot = true;
        public float weaponMaxDistance = -1;
        private float timer = 0;


        public Weapon Copy()
        {
            Weapon w = new Weapon();
            w.entityWeapon = entityWeapon;
            w.reloadSpeed = reloadSpeed;
            w.autoShoot = autoShoot;
            w.weaponMaxDistance = weaponMaxDistance;
            w.timer = Random.value * reloadSpeed;
            return w;
        }
        

        public void Update(Entity me, EntityCollection ec)
        {
            if (me.target == null)
                return;

            if (me.distanceTo(me.target) > weaponMaxDistance)
                return;


            timer += Time.deltaTime;
            if (timer>reloadSpeed)
            {
                timer = 0;
                ec.Instantiate(entityWeapon, me.particle.P, me.team, me.target.directionTo(me), weaponMaxDistance);
            }
        }


    }

    [System.Serializable]
    public class SerializedBodyPart {
        public string texture;
        public float moveSpeed=1;
        public float rotateSpeed=1;
        public float minSpeed = 1;
        public float translate_x=0, translate_y=0, translate_z=0;
        public float scale=1;
        public float rotateTexture = 0;
        public bool directionFromSpeed = true;
        public Weapon weapon;

        public SerializedBodyPart Copy()
        {
            SerializedBodyPart bp = new SerializedBodyPart();
            bp.texture = texture;
            bp.moveSpeed = moveSpeed;
            bp.rotateSpeed = rotateSpeed;
            bp.minSpeed = minSpeed;
            bp.translate_x = translate_x;
            bp.translate_y = translate_y;
            bp.translate_z = translate_z;
            bp.scale = scale;
            bp.rotateTexture = rotateTexture;
            bp.directionFromSpeed = directionFromSpeed;

            if (weapon != null)
                bp.weapon = weapon.Copy();

            foreach (SerializedBodyPart nbp in bodyParts)
                bp.bodyParts.Add(nbp.Copy());

            return bp;

        }

        public Vector3 Translate()
        {
            return new Vector3(translate_x, translate_y, translate_z);
        }

        public List<SerializedBodyPart> bodyParts = new List<SerializedBodyPart>();


    }

    [System.Serializable]
    public class SerializedEntity
    {
        public string typeName = "";
        public float value = 1;

        // If weapon
        public bool isTargetable = true;
        public float minDamage, maxDamage;
        public float detonateDistance;
        public string detonateEffect;
        public float damageRadius;
        public bool homing = true;
        public bool autoRotate = true;
        public float maxDistance = -1; // -1 means inf
        public float health = 10;


        public SerializedBodyPart body;

        public SerializedEntity Copy()
        {
            SerializedEntity c = new SerializedEntity();
            c.typeName = typeName;
            c.value = value;
            c.isTargetable = isTargetable;
            c.minDamage = minDamage;
            c.maxDamage = maxDamage;
            c.detonateDistance = detonateDistance;
            c.detonateEffect = detonateEffect;
            c.damageRadius = damageRadius;
            c.homing = homing;
            c.autoRotate = autoRotate;
            c.maxDistance = maxDistance;
            c.health = health;
            c.body = body.Copy();



            return c;
        }

    }

    [System.Serializable]
    public class SerializedEntities
    {
        public List<SerializedEntity> entities = new List<SerializedEntity>();
        public SerializedEntity Get(string e)
        {
            foreach (SerializedEntity se in entities)
                if (se.typeName == e)
                    return se;

            return null;
        }


        public static SerializedEntities se;




        public static void Generate()
        {
            SerializedEntities test = new SerializedEntities();
            SerializedEntity sen = new SerializedEntity();
            SerializedBodyPart bp = new SerializedBodyPart();
            bp.bodyParts.Add(new SerializedBodyPart());
            sen.body = bp;
            test.entities.Add(sen);
            Serialize(test, Constants.EntitiesXML);
        }

        public static SerializedEntities DeSerialize(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializedEntities));

            TextAsset textAsset = (TextAsset)Resources.Load(filename);
            TextReader textReader = new StringReader(textAsset.text);
            SerializedEntities sz = (SerializedEntities)deserializer.Deserialize(textReader);
            textReader.Close();
            return sz;
        }
        static public void Serialize(SerializedEntities sz, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializedEntities));
            TextWriter textWriter = new StreamWriter(filename);
            serializer.Serialize(textWriter, sz);
            textWriter.Close();
        }




    }


}