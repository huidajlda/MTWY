using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MFarm.Inventory 
{
    public class Box : MonoBehaviour
    {
        public InventoryBag_SO boxBagTemplate;//箱子背包的模板数据
        public InventoryBag_SO boxBagData;//箱子背包数据
        public GameObject mouseIcon;//走近显示的鼠标图标
        private bool canOpen = false;//箱子是否可以打开
        private bool isOpen;//箱子是否已经打开
        public int index;//箱子序号
        private void OnEnable()
        {
            if (boxBagData == null) 
            {
                boxBagData=Instantiate(boxBagTemplate);//赋值模板数据
            }
        }
        //进入触发器
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) 
            {
                canOpen = true;//箱子可以打开
                mouseIcon.SetActive(true);//显示图标
            }
        }
        //离开触发器
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                canOpen = false;//箱子不能打开
                mouseIcon.SetActive(false);//关闭图标
            }
        }
        private void Update()
        {   //箱子没有打开,且可以打开并按了鼠标右键
            if (!isOpen && canOpen && Input.GetMouseButtonDown(1)) 
            {
                //打开箱子
                EventHandler.CallBaseBagOpenEvent(SlotType.Box, boxBagData);
                isOpen = true;
            }
            if (!canOpen && isOpen) //人物已经走了但箱子打开了
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen=false;
            }
            if (isOpen&&Input.GetKeyDown(KeyCode.Escape)) //按ESC关闭箱子
            {
                //关闭箱子
                EventHandler.CallBaseBagCloseEvent(SlotType.Box, boxBagData);
                isOpen = false;
            }
        }
        //初始化盒子
        public void InitBox(int boxIndex) 
        {
            index=boxIndex;
            var key = this.name + index;
            if (InventoryManager.Instance.GetBoxDataList(key) != null) //字典里已经有内容了
            {
                boxBagData.itemList = InventoryManager.Instance.GetBoxDataList(key);
            }
            else //新建箱子
            {
                InventoryManager.Instance.AddBoxDataDic(this);
            }
        }
    }
}