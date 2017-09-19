using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LemonSpawn;
using MoonSharp.Interpreter;
using UnityEngine.UI;
using UnityEngine.Experimental.UIElements;
using System.IO;

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

        private List<GameObject> panels = new List<GameObject>(); // pnlEdit, pnlCreateNew, pnlLoad;
        private Dropdown drpLoad;
        private string[] strPanels = new string[] { "pnlEdit", "pnlCreateNew", "pnlLoad" };


        private void FindPanels()
        {
            foreach (string s in strPanels)
                panels.Add(GameObject.Find(s));
        }

        public void setPanel(string s)
        {
            foreach (GameObject g in panels)
            {
                if (g.name == s)
                    g.SetActive(true);
                else g.SetActive(false);
            }
        }


        private void Init()
        {
            editTypes.Clear();
            editTypes.Add(new EditType(0, "Background", 0));
            editTypes.Add(new EditType(1, "Middle", 0));
            editTypes.Add(new EditType(2, "Foreground", 0));

            allCategories = SerializedMapCategories.mapCategories.getAllCategories();
            FindPanels();
            drpLoad = GameObject.Find("drpLoad").GetComponent<Dropdown>();

            setPanel("pnlLoad");
            PopulateLoad();

        }

        public void onPnlLoad()
        {
            setPanel("pnlLoad");
        }

        public void onPnlEdit()
        {
            setPanel("pnlEdit");
        }

        public void onPnlCreateNew()
        {
            setPanel("pnlCreateNew");
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
            {
                GameObject go = GameObject.Find("imgCurrent");
                if (go!=null) 
                    go.GetComponent<UnityEngine.UI.Image>().sprite = currentCategory.sprites[0];
            }
        }


        public void PopulateLoad()
        {
            if (drpLoad == null)
                return;
            drpLoad.options.Clear();
            /*          DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/8BD/Resources/Maps/");
                      FileInfo[] fileInfo = info.GetFiles();
                      foreach (FileInfo f in fileInfo)
                      {
                          string name = f.Name.Remove(f.Name.Length - 4, 4);
                          Dropdown.OptionData d = new Dropdown.OptionData();
                          d.text = name;
                          cbx.options.Add(d);
                      }*/
            string name = "";
            if (currentLevel != null)
                name = currentLevel.sz.name;
            int i = 0;
            foreach (SerializedGameLevel gl in SerializedGameLevels.gameLevels.levels)
            {
                Dropdown.OptionData d = new Dropdown.OptionData();
                d.text = gl.name;
                drpLoad.options.Add(d);
                if (d.text == name)
                    drpLoad.value = i;
                i++;
            }
 
        }
        public void LoadLevelFromDropdown()
        {
            Dropdown cbx = GameObject.Find("drpLoad").GetComponent<Dropdown>();

            currentLevel.Destroy();
            currentLevel = SerializedGameLevels.getLevel(cbx.options[cbx.value].text);

        }


        public void CreateNew()
        {
            SerializedGameLevel gl = new SerializedGameLevel();
            gl.crtSettings_id = "crt";
            gl.mapName = GameObject.Find("inpMapName").GetComponent<InputField>().text;
            gl.name = GameObject.Find("inpNewName").GetComponent<InputField>().text;

            Map2D map = new Map2D();
            int x = int.Parse(GameObject.Find("inpX").GetComponent<InputField>().text);
            int y = int.Parse(GameObject.Find("inpY").GetComponent<InputField>().text);
            map.Create(x, y);
            Map2D.Save(map, gl.mapName);

            SerializedGameLevels.gameLevels.levels.Add(gl);
            SaveAll();

            currentLevel.Destroy();
            currentLevel = SerializedGameLevels.getLevel(gl.name);


        }

        public void SaveAll()
        {
            SerializedGameLevels.Serialize(SerializedGameLevels.gameLevels, Application.dataPath + "/8BD/Resources/" + Constants.GameLevelsXML + ".xml");
            Debug.Log("Saving : " + currentLevel.sz.mapName);
            Map2D.Save(currentLevel.map, currentLevel.sz.mapName);
            UnityEditor.AssetDatabase.Refresh();
            PopulateLoad();
        }

        // Update is called once per frame

        void UpdateColorBackground()
        {
            if (currentItem != null)
                currentItem.items[MapCompositeItem.BACKGROUND].color = Color.white;
            currentItem = currentLevel.map.get((int)targetPos.x, (int)targetPos.y);
            
            if (currentItem != null)
                currentItem.items[MapCompositeItem.BACKGROUND].color = new Color(0.45f, 0.8f, 1.5f);

        }

        void InputKeys()
        {
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
                currentEditType = (currentEditType + 1) % editTypes.Count;
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
                currentItem.items[e.idx].newId(currentCategory.id);
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

        void HideAllPanels()
        {

        }

        void Update()
        {

            UpdateText();
            int curid = allCategories[currentEditType][editTypes[currentEditType].currentID].id;
//          Debug.Log(curid);

            if (currentCategory == null || currentCategory.id != curid)
                currentCategory = SerializedMapCategories.mapCategories.get(curid);

            UpdateColorBackground();
    

            currentLevel.dMap.Update(currentPos.x, currentPos.y);

            float t = 0.6f;
            currentPos = t * currentPos + (1 - t) * targetPos;

            InputKeys();


        }
    }

}