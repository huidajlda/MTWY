using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using MFarm.Map;
using MFarm.CropPlant;
using MFarm.Inventory;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed,item;//���ͼƬ(������ʹ�ù��ߣ�ʹ������,��Ʒ)
    private Sprite currentSprite;//�洢��ǰ�����ͼƬ
    private Image cursorImage;//ͼƬ���
    private RectTransform cursorCanvas;//���ͼƬ���ڵĻ���
    //����ͼ�����
    private Image buildImage;//����ͼ��
    //����⣨��Ļ����ת����������ת�������꣩
    private Camera mainCamera;//�������
    private Grid currentGrid;//����
    private Vector3 mouseWorldPos;//������������
    private Vector3Int mouseGridPos;//������������
    private bool cursorEnable;//����Ƿ����
    private bool cursorPositionValid;//����Ƿ���õ㰴
    private ItemDetails currentItem;//��Ʒ��Ϣ
    private Transform PlayerTransform=>FindObjectOfType<Player>().transform;//���λ��
    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;//��Ʒѡ��ʱ�л����ͼƬ�ķ���
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;//�������
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneLoadedEvent;//�õ�����������¼�
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneLoadedEvent;
    }

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();//������ڻ���
        cursorImage=cursorCanvas.GetChild(0).GetComponent<Image>();//��ȡ���UI��image���
        //�õ�����ͼ��
        buildImage = cursorCanvas.GetChild(1).GetComponent<Image>();//��ȡ���UI��image���
        buildImage.gameObject.SetActive(false);//�ر�ͼ��
        currentSprite = normal;//���������ͼƬ
        SetCursorImage(normal);//�������ͼƬ
        mainCamera=Camera.main;//�õ��������
    }
    private void Update()
    {
        if (cursorCanvas == null) return;
        cursorImage.transform.position=Input.mousePosition;//������
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();//��ȡ�����������
            CheckPlayerInput();//ִ�е�������¼�
        }
        else 
        {
            SetCursorImage(normal);//��UI������Ϊ�������ͼƬ
            buildImage.gameObject.SetActive(false);//��UI�л���ʱ�͹رս���ͼƬ
        }
    }
    //ִ�е�������¼�
    private void CheckPlayerInput() 
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid) //���������������
        {
            //ִ�з�����������¼���
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);//�����������ǰ�������͵�ǰʹ�õ���Ʒ
        }
    }
    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;//�������
    }
    private void OnAfterSceneLoadedEvent() 
    {
        currentGrid=FindObjectOfType<Grid>();//�õ���ǰ����������
    }
    #region ���������ʽ
    //�����л����ͼƬ�ķ���
    private void SetCursorImage(Sprite sprite) 
    {
        cursorImage.sprite= sprite;//�л�ͼƬ
        cursorImage.color = new Color(1, 1, 1, 1);//ȫ����ʾ
    }
    //����������
    private void SetCursorValid() 
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);//��ʾ��ȫ����ɫ
        buildImage.color = new Color(1, 1, 1, 0.5f);//���ý���ͼƬ��͸����
    }
    //������겻����
    private void SetCursorInValid() 
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.5f);//��ʾ��͸���ĺ�ɫ
        buildImage.color = new Color(1, 0, 0, 0.5f);//���ý���ͼƬ����ɫ
    }
    #endregion
    //ѡ����Ʒʱ���ض�Ӧ�������ʽ
    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        currentItem =itemDetails;
        if (!isSelected)//û�б�ѡ��
        {
            currentSprite = normal;
            cursorEnable = false;
            currentItem = null;
            buildImage.gameObject.SetActive(false);//�رս���ͼ��
        }
        else //ѡ������Ʒ
        {
            currentItem = itemDetails;
            //������Ʒ���ͷ��ص�ǰ��Ӧ�����ͼƬ
            currentSprite = itemDetails.itemType switch
            {//���Ը�����Ʒ���Ͳ�ȫ
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool=> tool,
                ItemType.BreakTool=> tool,
                ItemType.ReapTool=> tool,
                ItemType.Furniture=> tool,
                ItemType.CollectTool=> tool,//������
                _ => normal
            };
            cursorEnable = true;
            //��ʾ������ƷͼƬ
            if (itemDetails.itemType == ItemType.Furniture) 
            {
                buildImage.gameObject.SetActive(true);//��ʾ����ͼ��
                buildImage.sprite = itemDetails.itemOnWorldSprite;//��ֵͼƬ
                buildImage.SetNativeSize();//ȷ��ͼƬ��ʾ����
            }
        }

    }
    //�ж�����Ƿ����
    private void CheckCursorValid() 
    {
        //mouseWorldPos=mainCamera.ScreenToWorldPoint(Input.mousePosition);//��Ļת��������(ֻ��2D��3DҪ����z��)
        //��׼д��
        mouseWorldPos=mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,-mainCamera.transform.position.z));
        mouseGridPos =currentGrid.WorldToCell(mouseWorldPos);//��������ת��������
        var playerGridPos=currentGrid.WorldToCell(PlayerTransform.position);//��ҵ�ǰ��������ת��������
        //����ͼƬ�����ƶ�
        buildImage.rectTransform.position=Input.mousePosition;
        //�ж���ʹ�÷�Χ֮��(���λ�ü�ȥ���λ�õľ���ֵ�Ƿ����ʹ�÷�Χ)
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius) 
        {
            SetCursorInValid();//��ʹ�÷�Χ�⣬������Ϊ�����õ������ʽ
            return;
        }
        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);//�õ���ǰ�������Ƭ��Ϣ
        if ((currentTile != null))
        {
            //�õ���ǰ���Ӷ�Ӧ����ֲ��Ϣ
            CropDetails currentCrop = CropManager.Instance.GetCropDetails(currentTile.seedItemID);
            Crop crop = GridMapManager.Instance.GetCropObject(mouseWorldPos);//�õ���ֲ��Ϣ
            switch (currentItem.itemType)
            {
                case ItemType.Seed://���ӣ���ǰ��Ƭ�ڿ�����û����ֲ����
                    if(currentTile.daysSinceDug>-1&&currentTile.seedItemID==-1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity://�����Ʒ����Ϊ��Ʒ
                    //��Ƭ�ǿɶ�����λ������Ʒ�ɶ�������������ÿ��ã����򲻿���
                    if (currentTile.canDropItem&&currentItem.canDropped) SetCursorValid(); else SetCursorInValid();
                    break;//δ����
                case ItemType.HoeTool://����Ƭ�����ڿӣ������ã������ԾͲ�����
                    if(currentTile.canDig) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.WaterTool://���Խ�ˮ(�õط��Ѿ��ڿ���û�б���ˮ)
                    if(currentTile.daysSinceDug>-1&&currentTile.daysSinceWatered==-1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.CollectTool://�������ջ�
                    if (currentCrop != null)
                    {//�����Ѿ�������
                        if (currentCrop.CheckToolAvailabele(currentItem.itemId))//�жϸ��ռ������Ƿ���ռ�������
                            if(currentTile.growthDays>=currentCrop.TotalGrowthDays) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.BreakTool://ʮ�ָ乤��
                case ItemType.ChopTool://��ͷ����
                    if (crop != null) 
                    {
                        if (crop.CanHarvest&&crop.cropDetails.CheckToolAvailabele(currentItem.itemId)) SetCursorValid(); else SetCursorInValid();
                    }
                    else SetCursorInValid();
                    break;
                case ItemType.ReapTool://���ݹ���
                    if (GridMapManager.Instance.HaveReapableItemsInRadius(mouseWorldPos,currentItem)) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Furniture:
                    buildImage.gameObject.SetActive(true);
                    //����Ƭ���ý����ұ����еĿ�湻��
                    var bluePrintDetails=InventoryManager.Instance.bluPrintData.GetBluePrintDetails(currentItem.itemId);
                    if (currentTile.canPlaceFurniture&&InventoryManager.Instance.CheckStock(currentItem.itemId)&&
                        !HaveFurnitureInRaduis(bluePrintDetails))
                        SetCursorValid();
                    else SetCursorInValid();
                    break;
            }
        }
        else //��Ƭ��ϢΪ�յĻ�ͳһ���Ϊ������
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
    //�ж�����Ƿ���UI����
    private bool InteractWithUI() 
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return true;
        else return false;
    }
}
