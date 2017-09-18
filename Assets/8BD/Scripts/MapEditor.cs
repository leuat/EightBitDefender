using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LemonSpawn;

namespace LemonSpawn
{

    public class MapEditor : MonoBehaviour
    {
        Map2D map = new Map2D();
        DisplayMap dMap = new DisplayMap();

        MapCompositeItem currentItem;
        Vector2 currentPos = new Vector2(90, 90);
        Vector2 targetPos = new Vector2(90, 90);


        // Use this for initialization
        void Start()
        {
            Game.game.Initialize();

            map.Create(256, 256);
            dMap.Initialize(map, 30, 20, 1, 1);

        }

        // Update is called once per frame
        void Update()
        {
            if (currentItem!=null)
                currentItem.background.color = Color.white;
            currentItem = map.get((int)targetPos.x, (int)targetPos.y);
            if (currentItem!=null)
                currentItem.background.color = new Color(0.45f, 0.8f, 1.5f);
            dMap.Update(currentPos.x, currentPos.y);

            float t = 0.6f;
            currentPos = t * currentPos + (1 - t) * targetPos;


    
            if (Input.GetKeyUp(KeyCode.R))
            {
                currentItem.background.newId(currentItem.background.id+1);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                currentItem.background.newId(currentItem.background.id-1);
            }
            if (Input.GetKeyDown("up"))
                targetPos.y += 1;
            if (Input.GetKeyDown("down"))
                targetPos.y -= 1;
            if (Input.GetKeyDown("left"))
                targetPos.x -= 1;
            if (Input.GetKeyDown("right"))
                targetPos.x += 1;



        }
    }

}