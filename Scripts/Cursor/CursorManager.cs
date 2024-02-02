using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MFarm.Map;
using MFarm.CropPlant;
using MFarm.Inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed,item;//鼠标图片(正常，使用工具，使用种子,商品)
    private Sprite currentSprite;//存储当前的鼠标图片
    private Image cursorImage;//图片组件
    private RectTransform cursorCanvas;//鼠标图片所在的画板
    //建造图标跟随
    private Image buildImage;//建造图标
    //鼠标检测（屏幕坐标转世界坐标再转网格坐标）
    private Camera mainCamera;//主摄像机
    private Grid currentGrid;//网格
    private Vector3 mouseWorldPos;//鼠标的世界坐标
    private Vector3Int mouseGridPos;//鼠标的网格坐标
    private bool cursorEnable;//鼠标是否可用
    private bool cursorPositionValid;//鼠标是否可用点按
    private ItemDetails currentItem;//物品信息
    private Transform PlayerTransform=>FindObjectOfType<Player>().transform;//玩家位置
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;//物品选中时切换鼠标图片的方法
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;//禁用鼠标
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneLoadedEvent;//拿到网格组件的事件
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneLoadedEvent;
    }

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();//鼠标所在画板
        cursorImage=cursorCanvas.GetChild(0).GetComponent<Image>();//获取鼠标UI的image组件
        //拿到建造图标
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();//获取鼠标UI的image组件
        buildImage.gameObject.SetActive(false);//关闭图标
        currentSprite = normal;//正常的鼠标图片
        SetCursorImage(normal);//设置鼠标图片
        mainCamera=Camera.main;//拿到主摄像机
    }
    private void Update()
    {
        if (cursorCanvas == null) return;
        cursorImage.transform.position=Input.mousePosition;//鼠标跟随
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();//获取鼠标网格坐标
            CheckPlayerInput();//执行点击鼠标的事件
        }
        else 
        {
            SetCursorImage(normal);//是UI就设置为正常鼠标图片
            buildImage.gameObject.SetActive(false);//跟UI有互动时就关闭建造图片
        }
    }
    //执行点击鼠标的事件
    private void CheckPlayerInput() 
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid) //按下左键且鼠标可用
        {
            //执行方案（鼠标点击事件）
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);//传入参数，当前鼠标坐标和当前使用的物品
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;//禁用鼠标
    }
    private void OnAfterSceneLoadedEvent() 
    {
        currentGrid=FindObjectOfType<Grid>();//拿到当前场景的网格
    }
    #region 设置鼠标样式
    //设置切换鼠标图片的方法
    private void SetCursorImage(Sprite sprite) 
    {
        cursorImage.sprite= sprite;//切换图片
        cursorImage.color = new Color(1, 1, 1, 1);//全部显示
    }
    //设置鼠标可用
    private void SetCursorValid() 
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);//显示完全的颜色
        buildImage.color = new Color(1, 1, 1, 0.5f);//设置建造图片的透明度
    }
    //设置鼠标不可用
    private void SetCursorInValid() 
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);//显示半透明的红色
        buildImage.color = new Color(1, 0, 0, 0.5f);//设置建造图片的颜色
    }
    #endregion
    //选中物品时返回对应的鼠标样式
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        currentItem =itemDetails;
        if (!isSelected)//没有被选中
        {
            currentSprite = normal;
            cursorEnable = false;
            currentItem = null;
            buildImage.gameObject.SetActive(false);//关闭建造图标
        }
        else //选中了物品
        {
            currentItem = itemDetails;
            //根据物品类型返回当前对应的鼠标图片
            currentSprite = itemDetails.itemType switch
            {//可以根据物品类型补全
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool=> tool,
                ItemType.BreakTool=> tool,
                ItemType.ReapTool=> tool,
                ItemType.Furniture=> tool,
                ItemType.CollectTool=> tool,//菜篮子
                _ => normal
            };
            cursorEnable = true;
            //显示建造物品图片
            if (itemDetails.itemType == ItemType.Furniture) 
            {
                buildImage.gameObject.SetActive(true);//显示建造图标
                buildImage.sprite = itemDetails.itemOnWorldSprite;//赋值图片
                buildImage.SetNativeSize();//确保图片显示正常
            }
        }

    }
    //判断鼠标是否可用
    private void CheckCursorValid() 
    {
        //mouseWorldPos=mainCamera.ScreenToWorldPoint(Input.mousePosition);//屏幕转世界坐标(只限2D，3D要考虑z轴)
        //标准写法
        mouseWorldPos=mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos =currentGrid.WorldToCell(mouseWorldPos);//世界坐标转网格坐标
        var playerGridPos=currentGrid.WorldToCell(PlayerTransform.position);//玩家当前世界坐标转网格坐标
        //建造图片跟随移动
        buildImage.rectTransform.position=Input.mousePosition;
        //判断在使用范围之内(鼠标位置减去玩家位置的绝对值是否大于使用范围)
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius) 
        {
            SetCursorInValid();//在使用范围外，都设置为不可用的鼠标样式
            return;
        }
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);//拿到当前网格的瓦片信息
        if ((currentTile != null))
        {
            //拿到当前格子对应的种植信息
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);//拿到种植信息
            switch (currentItem.itemType)
            {
                case ItemType.Seed://种子，当前瓦片挖坑了且没有种植种子
                    if(currentTile.daysSinceDug>-1&&currentTile.seedItemID==-1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity://如果物品类型为商品
                    //瓦片是可丢弃的位置且物品可丢弃，将鼠标设置可用，否则不可用
                    if (currentTile.canDropItem&&currentItem.canDropped) SetCursorValid(); else SetCursorInValid();
                    break;//未完善
                case ItemType.HoeTool://该瓦片可以挖坑，鼠标可用，不可以就不可用
                    if(currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool://可以浇水(该地方已经挖矿且没有被浇水)
                    if(currentTile.daysSinceDug>-1&&currentTile.daysSinceWatered==-1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.CollectTool://菜篮子收获
                    if (currentCrop != null)
                    {//种子已经成熟了
                        if (currentCrop.CheckToolAvailabele(currentItem.itemId))//判断该收集工具是否可收集该物体
                            if(currentTile.growthDays>=currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.BreakTool://十字镐工具
                case ItemType.ChopTool://斧头工具
                    if (crop != null) 
                    {
                        if (crop.CanHarvest&&crop.cropDetails.CheckToolAvailabele(currentItem.itemId)) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.ReapTool://除草工具
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos,currentItem)) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    //该瓦片可用建造且背包中的库存够用
                    var bluePrintDetails=InventoryManager.Instance.bluPrintData.GetBluePrintDetails(currentItem.itemId);
                    if (currentTile.canPlaceFurniture&&InventoryManager.Instance.CheckStock(currentItem.itemId)&&
                        !HaveFurnitureInRaduis(bluePrintDetails))
                        SetCursorValid();
                    else SetCursorInValid();
                    break;
            }
        }
        else //瓦片信息为空的话统一鼠标为不可用
        {
            SetCursorInValid();
        }
    }
    private bool HaveFurnitureInRaduis(BluePrintDetails bluePrintDetails) 
    {
        var buildItem = bluePrintDetails.buildPrefab;
        Vector2 point = mouseWorldPos;
        var size = buildItem.GetComponent<BoxCollider2D>().size;
        var otherColl = Physics2D.OverlapBox(point, size, 0);
        if (otherColl != null)
            return otherColl.GetComponent<Furniture>();
        return false;
    }
    //判断鼠标是否在UI上面
    private bool InteractWithUI() 
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        else return false;
    }
}
