using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LemonSpawn;
using MoonSharp.Interpreter;
using UnityEngine.UI;

namespace LemonSpawn
{

    public class EditType
    {
        public int idx;
        public string name;
        public int currentID;

        public EditType(int i, string n, int id)
        {
            idx = i;
            name = n;
            currentID = id;
        }
    }

    public class MapEditor : MonoBehaviour
    {

        MapCompositeItem currentItem;
        Vector2 currentPos = new Vector2(0, 0);
        Vector2 targetPos = new Vector2(0, 0);

        GameLevel currentLevel = null;

        public List<EditType> editTypes = new List<EditType>();
        private int currentEditType = 0;
        private MapCategory currentCategory = null;
        private List<List<MapCategory>> allCategories;

        
        private void Init()
        {
            editTypes.Clear();
            editTypes.Add(new EditType(0, "Background", 0));
            editTypes.Add(new EditType(1, "Middle", 0));
            editTypes.Add(new EditType(2, "Foreground", 0));

            allCategories = SerializedMapCategories.mapCategories.getAllCategories();
        }

        double MoonSharpFactorial()
        {
            string scriptCode = @"    
		-- defines a factorial function
		function fact (n)
			if (n == 0) then
				return 1
			else
				return n*fact(n - 1)
			end
		end";

            Script script = new Script();

            script.DoString(scriptCode);

            DynValue luaFactFunction = script.Globals.Get("fact");

            DynValue res = script.Call(luaFactFunction, 4);

            return res.Number;
        }
        // Use this for initialization
        void Start()
        {
            Game.game.Initialize();
            Init();

            currentLevel = SerializedGameLevels.getLevel("testlevel");


        }

        private void UpdateText()
        {
            Util.SetText("lblName", currentLevel.sz.name);
            Util.SetText("lblMapName", currentLevel.sz.mapName);
            Util.SetText("lblCurrentType", editTypes[currentEditType].name);

            if (currentCategory != null)
                GameObject.Find("imgCurrent").GetComponent<Image>().sprite = currentCategory.sprites[0];
        }

        public void SaveAll()
        {
            SerializedGameLevels.Serialize(SerializedGameLevels.gameLevels, Application.dataPath + "/8BD/Resources/" + Constants.GameLevelsXML + ".xml");
            Debug.Log("Saving : " + currentLevel.sz.mapName);
            Map2D.Save(currentLevel.map, currentLevel.sz.mapName);
            UnityEditor.AssetDatabase.Refresh();
        }

        // Update is called once per frame

        void Update()
        {

            UpdateText();
            int curid = allCategories[currentEditType][editTypes[currentEditType].currentID].id;
            if (currentCategory == null || currentCategory.id != curid)
            {
                currentCategory = SerializedMapCategories.mapCategories.get(curid);
            }

            

            if (currentItem != null)
                currentItem.items[MapCompositeItem.BACKGROUND].color = Color.white;


            currentItem = currentLevel.map.get((int)targetPos.x, (int)targetPos.y);
            if (currentItem != null)
                currentItem.items[MapCompositeItem.BACKGROUND].color = new Color(0.45f, 0.8f, 1.5f);

            currentLevel.dMap.Update(currentPos.x, currentPos.y);

            float t = 0.6f;
            currentPos = t * currentPos + (1 - t) * targetPos;


            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                editTypes[currentEditType].currentID = (editTypes[currentEditType].currentID + 1) % allCategories[currentEditType].Count;
            }
            if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                editTypes[currentEditType].currentID = (editTypes[currentEditType].currentID - 1);
                if (editTypes[currentEditType].currentID < 0)
                    editTypes[currentEditType].currentID = allCategories[currentEditType].Count - 1;
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                currentEditType = (currentEditType + 1)%editTypes.Count;
                //currentItem.items[MapCompositeItem.BACKGROUND].newId(currentItem.items[MapCompositeItem.BACKGROUND].id + 1);
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                currentEditType = (currentEditType - 1);
                if (currentEditType < 0)
                    currentEditType = editTypes.Count - 1;
                //currentItem.items[MapCompositeItem.BACKGROUND].newId(currentItem.items[MapCompositeItem.BACKGROUND].id - 1);
            }

            if (Input.GetKeyUp(KeyCode.Space))
            {
                EditType e = editTypes[currentEditType];
                currentItem.items[e.idx].newId(  currentCategory.id);
            }
            
            if (Input.GetKeyUp(KeyCode.Backspace))
            {
                EditType e = editTypes[currentEditType];
                currentItem.items[e.idx].newId(0);
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