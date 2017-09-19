using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace LemonSpawn
{


    [System.Serializable]
    public class MapCategory
    {
        public string name;
        public int id;
        public string description;
        public string texture;
        public int noFrames;
        public bool foreground = false;
        public bool collision = false;
        public float animSpeed = 1;
        public Sprite[] sprites;
        public int maincategory;
        public string subcategory;

        public void Initialize()
        {
            //            if (noFrames)
            if (noFrames == 1)
            {
                sprites = new Sprite[noFrames];
                sprites[0] = Sprites.Get(texture);
                if (sprites[0] == null)
                    Debug.Log("COULD not find sprite: " + texture);

            }
            else
                sprites = Sprites.GetASprite(texture);
                if (sprites==null)
                if (sprites[0] == null)
                    Debug.Log("COULD not find sprite (stack) : " + texture);


        }

    }


    [System.Serializable]
    public class SerializedMapCategories
    {
        public List<MapCategory> categories = new List<MapCategory>();
        public static SerializedMapCategories mapCategories;// = new SerializedMapCategories();


        public MapCategory get(int id)
        {
            foreach (MapCategory mc in categories)
                if (mc.id == id)
                    return mc;
            return null;
        }

        public List<MapCategory> getCategories(int cat)
        {
            List<MapCategory> c = new List<MapCategory>();
            foreach (MapCategory mc in categories)
                if (mc.maincategory == cat)
                    c.Add(mc);
            return c;
        }

        public List<List<MapCategory>> getAllCategories()
        {
            List<List<MapCategory>> c = new List<List<MapCategory>>();
            c.Add(getCategories(0));
            c.Add(getCategories(1));
            c.Add(getCategories(2));
            return c;
        }


        public static SerializedMapCategories DeSerialize(string filename)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(SerializedMapCategories));

            TextAsset textAsset = (TextAsset)Resources.Load(filename);
            TextReader textReader = new StringReader(textAsset.text);
            SerializedMapCategories sz = (SerializedMapCategories)deserializer.Deserialize(textReader);
            textReader.Close();

            foreach (MapCategory mc in sz.categories)
                mc.Initialize();

            return sz;
        }

    }




    [System.Serializable]
    public class Weapon
    {
        public string entityWeapon;
        public float reloadSpeed = 1;
        public bool autoShoot = true;
        public float weaponMaxDistance = -1;
        public string sound = "";
        private float timer = 0;
        


        public Weapon Copy()
        {
            Weapon w = new Weapon();
            w.entityWeapon = entityWeapon;
            w.reloadSpeed = reloadSpeed;
            w.autoShoot = autoShoot;
            w.weaponMaxDistance = weaponMaxDistance;
            w.timer = Random.value * reloadSpeed;
            w.sound = sound;
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
                float d = me.target.distanceTo(me);
                Vector3 tar = (me.target.directionTo(me) + me.target.particle.V*d).normalized;

                ec.Instantiate(entityWeapon, me.particle.P, me.team,tar, weaponMaxDistance);
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
        public string deathSound = "";
        public bool isWeapon = false;

        public List<string> deathTexts = new List<string>();
        public List<string> birthTexts = new List<string>();



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
            c.deathSound = deathSound;
            c.isWeapon = isWeapon;
            c.deathTexts = deathTexts;
            c.birthTexts = birthTexts;
//            Debug.Log(c.typeName + " " + birthTexts.Count);
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