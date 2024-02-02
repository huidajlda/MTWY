using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;
    private ItemDataList_SO database;//物品数据
    private List<ItemDetails> itemList=new List<ItemDetails>();//物品列表
    private VisualTreeAsset itemPowTemplate;//列表格子模板
    private ListView itemListView;
    private ScrollView itemDetailsSection;
    private ItemDetails activeItem;//当前物品信息
    private VisualElement iconPreview;
    private Sprite defaultIcon;
    [MenuItem("Window/UI Toolkit/ItemEditor")]//进入窗口的路径
    public static void ShowExample()
    {
        ItemEditor wnd = GetWindow<ItemEditor>();
        wnd.titleContent = new GUIContent("ItemEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;//根节点

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);
        //拿到模板数据
        itemPowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/UI Builder/ItemRow Template.uxml");
        //变量赋值
        itemListView = root.Q<VisualElement>("ItemList").Q<ListView>("ListView");
        itemDetailsSection = root.Q<ScrollView>("ItemDetails");
        iconPreview = itemDetailsSection.Q<VisualElement>("Icon");
        //拿到默认的Icon
        defaultIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Items/Icons/icon_M.png");
        //获取按钮
        root.Q<Button>("AddButton").clicked += OnAddItemClicked;
        root.Q<Button>("DeleteButton").clicked += OnDeleteItemClicked;
        //加载数据
        LoadDataBase();
        GenerateListView();
    }
    //按键事件删除物品
    private void OnDeleteItemClicked()
    {
        itemList.Remove(activeItem);
        itemListView.Rebuild();
        itemDetailsSection.visible = false;//隐藏详细面板
    }
    //按键事件添加物品
    private void OnAddItemClicked()
    {
        ItemDetails newItem=new ItemDetails();//创建物品类
        newItem.itemName = "New Item";//初始名字
        newItem.itemId=1000+itemList.Count;
        itemList.Add(newItem);
        itemListView.Rebuild();//刷新
    }

    //获取路径加载数据
    private void LoadDataBase() 
    {
        //找到指定的文件类型的文件，返回的是一个GUID数组
        var dataArray= AssetDatabase.FindAssets("ItemDataList_SO");
        if (dataArray.Length > 1) 
        {
            //将GUID转换成文件路径
            var path = AssetDatabase.GUIDToAssetPath(dataArray[0]);
            //加载数据文件
            database = AssetDatabase.LoadAssetAtPath(path, typeof(ItemDataList_SO)) as ItemDataList_SO;
        }
        itemList = database.itemDetailsList;
        EditorUtility.SetDirty(database);//标记数据，如果不标记则无法保存数据
    }
    //生成物品列表
    private void GenerateListView() 
    {
        Func<VisualElement> makeItem = () => itemPowTemplate.CloneTree();//生成一份模板
        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (i < itemList.Count)
            {
                if (itemList[i].itemIcon!=null)
                    e.Q<VisualElement>("Icon").style.backgroundImage = itemList[i].itemIcon.texture;
                e.Q<Label>("Name").text = itemList[i] == null ? "NO ITEM" : itemList[i].itemName;
            }
        };
        itemListView.fixedItemHeight=60;
        itemListView.itemsSource = itemList;
        itemListView.makeItem = makeItem;
        itemListView.bindItem = bindItem;
        itemListView.onSelectionChange += OnListSelectionChange;
        itemDetailsSection.visible = false;//正常不可见,没有显示
    }

    private void OnListSelectionChange(IEnumerable<object> selectedItem)
    {
        activeItem=(ItemDetails)selectedItem.First();
        GetItemDetails();
        itemDetailsSection.visible = true;//列表选择了东西才让内容可见
    }
    //更新数据
    private void GetItemDetails() 
    {
        itemDetailsSection.MarkDirtyRepaint();//写入这个可以让面板数据更改时可以撤销
        itemDetailsSection.Q<IntegerField>("ItemID").value = activeItem.itemId;
        //重新赋值,ID,NAME,ICON
        itemDetailsSection.Q<IntegerField>("ItemID").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemId = evt.newValue;
        });
        itemDetailsSection.Q<TextField>("ItemName").value = activeItem.itemName;
        itemDetailsSection.Q<TextField>("ItemName").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemName = evt.newValue;
            itemListView.Rebuild();
        });
        iconPreview.style.backgroundImage = activeItem.itemIcon== null ? defaultIcon.texture : activeItem.itemIcon.texture;
        itemDetailsSection.Q<ObjectField>("ItemIcon").value = activeItem.itemIcon;
        itemDetailsSection.Q<ObjectField>("ItemIcon").RegisterValueChangedCallback(evt =>
        {
            Sprite newIcon = evt.newValue as Sprite;
            activeItem.itemIcon = newIcon;
            iconPreview.style.backgroundImage = newIcon == null ? defaultIcon.texture : newIcon.texture;
            itemListView.Rebuild();
        });
        //其他所有变量的绑定
        itemDetailsSection.Q<ObjectField>("ItemSprite").value = activeItem.itemOnWorldSprite;
        itemDetailsSection.Q<ObjectField>("ItemSprite").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemOnWorldSprite = (Sprite)evt.newValue;
        });

        itemDetailsSection.Q<EnumField>("ItemType").Init(activeItem.itemType);
        itemDetailsSection.Q<EnumField>("ItemType").value = activeItem.itemType;
        itemDetailsSection.Q<EnumField>("ItemType").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemType = (ItemType)evt.newValue;
        });

        itemDetailsSection.Q<TextField>("Description").value = activeItem.itemDescription;
        itemDetailsSection.Q<TextField>("Description").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemDescription = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("ItemUseRadius").value = activeItem.itemUseRadius;
        itemDetailsSection.Q<IntegerField>("ItemUseRadius").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemUseRadius = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanPickedUp").value = activeItem.canPickedup;
        itemDetailsSection.Q<Toggle>("CanPickedUp").RegisterValueChangedCallback(evt =>
        {
            activeItem.canPickedup = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanDropped").value = activeItem.canDropped;
        itemDetailsSection.Q<Toggle>("CanDropped").RegisterValueChangedCallback(evt =>
        {
            activeItem.canDropped = evt.newValue;
        });

        itemDetailsSection.Q<Toggle>("CanCarried").value = activeItem.canCarried;
        itemDetailsSection.Q<Toggle>("CanCarried").RegisterValueChangedCallback(evt =>
        {
            activeItem.canCarried = evt.newValue;
        });

        itemDetailsSection.Q<IntegerField>("Price").value = activeItem.itemPrice;
        itemDetailsSection.Q<IntegerField>("Price").RegisterValueChangedCallback(evt =>
        {
            activeItem.itemPrice = evt.newValue;
        });

        itemDetailsSection.Q<Slider>("SellPercentage").value = activeItem.sellPercentage;
        itemDetailsSection.Q<Slider>("SellPercentage").RegisterValueChangedCallback(evt =>
        {
            activeItem.sellPercentage = evt.newValue;
        });
    }
}
