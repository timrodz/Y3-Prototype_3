using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase instance;

    private List<Item> database = new List<Item>();
    private JsonData itemData;


    private void Start()
    {
        if (instance != null)
            Destroy(this);

        //Item item = new Item(0, "Honey", 5);
        //database.Add(item);
        //Debug.Log(database[0].Title);
        itemData = JsonMapper.ToObject(File.ReadAllText(Application.dataPath + "/StreamingAssets/Items.json"));

        ConstructItemDatabase();

//        Debug.Log(database[1].Title);
        Item item = new Item();

        instance = this;
    }


    public Item FetchItemByID(int ID)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if(database[i].ID == ID)
            {
                return database[i];
            }
        }

        return null;
    }

    void ConstructItemDatabase()
    {
        //print(itemData.Count);
        for (int i = 0; i < itemData.Count; i++)
        {
            database.Add(new Item((int)itemData[i]["id"], (string)itemData[i]["title"].ToString(), (int)itemData[i]["value"], (int)itemData[i]["stats"]["maxstack"], (int)itemData[i]["stats"]["edible"],
                (string)itemData[i]["description"].ToString(), itemData[i]["sprite"].ToString(), (int)itemData[i]["spreadsheet"], (int)itemData[i]["spriteIndex"]));
        }
    }

}

public class Item
{
    //LOADED VARIABLES FROM JSON LIST

    public int ID { get; set; }
    public string Title { get; set; }
    public int Value { get; set; }
    public int MaxStack { get; set; }
    public bool Edible { get; set; }
    public string Description { get; set; }
    public string spriteName { get; set; }
    public bool usingSpreadSheet { get; set; }
    public int sliceIndex { get; set; }


    public int count { get; set; }

    public Sprite Sprite { get; set; }

    public Item(int id, string title, int value, int maxStack, int edible, string description, string spriteName = "", int useSheet = 0, int sliceIndex = 0)
    {

        this.ID = id;
        this.Title = title;
        this.Value = value;
        this.MaxStack = maxStack;
        this.Edible = edible == 1 ? true : false;
        this.Description = description;
        this.spriteName = spriteName;
        this.usingSpreadSheet = useSheet == 1 ? true : false;
        this.sliceIndex = sliceIndex;

        count = 1;

        //IN GAME DATA

        if(spriteName != "")
        {
            if(usingSpreadSheet)
            {
                Sprite[] sprites = Resources.LoadAll<Sprite>("UI/Items");
                this.Sprite = sprites[sliceIndex];
            }

            else
            {
                Sprite sprite = Resources.Load<Sprite>("UI/Items/" + spriteName);
                this.Sprite = sprite;
            }
        }
    }

    public Item(Item item)
    {

        this.ID = item.ID;
        this.Title = item.Title;
        this.Value = item.Value;
        this.MaxStack = item.MaxStack;
        this.Edible = item.Edible;
        this.Description = item.Description;
        this.spriteName = item.spriteName;
        this.usingSpreadSheet = item.usingSpreadSheet;
        this.sliceIndex = item.sliceIndex;
        this.Sprite = item.Sprite;

        count = 1;
    }

    public Item()
    {
        this.ID = -1;
    }

}