using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
namespace MFarm.Dialogue 
{
    //挂载到NPC身上时确保NPC身上有这两个组件
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        private NPCMovement npc => GetComponent<NPCMovement>();//拿到身上的NPCMovement脚本组件
        public UnityEvent OnFinshEvent;//事件
        public List<DialoguePiece> dialogueList=new List<DialoguePiece>();//对话数据的列表
        private Stack<DialoguePiece> dailogueStack;//堆栈对话列表(将对话列表数据压进去，然后每次从头开始读就好了)
        private bool canTalk;//可以对话
        private bool isTalking;//是否正在说话
        private GameObject uiSign;//可互动显示的按钮
        private void Awake()
        {
            uiSign = transform.GetChild(1).gameObject;//获取可互动显示的按钮
            FillDialogueStack();//将对话数据压入栈中
        }
        private void Update()
        {
            uiSign.SetActive(canTalk);//设置激活失活
            if (canTalk && Input.GetKeyDown(KeyCode.Space)&&!isTalking) //且按下空格
            {
                StartCoroutine(DialogueRoutine());
            }
        }
        private void OnTriggerEnter2D(Collider2D other)//进入触发器
        {
            if (other.CompareTag("Player")) 
            {
                canTalk = !npc.isMoving && npc.interactable;//npc没有移动且可以互动那么就可以对话
            }
        }
        private void OnTriggerExit2D(Collider2D other)//离开触发器
        {
            if (other.CompareTag("Player"))
            {
                canTalk = false;//不可以对话
            }
        }
        //将对话数据压入栈中
        private void FillDialogueStack() 
        {
            dailogueStack = new Stack<DialoguePiece>();//创建栈
            //倒叙压栈，这样在栈里取出来就是正的
            for (int i = dialogueList.Count - 1; i > -1; i--) 
            {
                dialogueList[i].isDone = false;//对话没有结束
                dailogueStack.Push(dialogueList[i]);//压入栈
            }
        }
        private IEnumerator DialogueRoutine() 
        {
            isTalking = true;//在说话中
            if (dailogueStack.TryPop(out DialoguePiece result)) //只要栈里面有就会一直拿
            {
                //传到UI显示对话
                EventHandler.CallShowDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(GameState.Pause);//对话时暂停移动
                yield return new WaitUntil(() => result.isDone);//对话显示完才会继续
                isTalking = false;
            }
            else 
            {
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);//对话结束后正常移动
                EventHandler.CallShowDialogueEvent(null);//传空就可以关闭了
                FillDialogueStack();//再次填充，可以继续对话
                isTalking= false;
                if (OnFinshEvent != null) 
                {
                    OnFinshEvent?.Invoke();//触发结束对话后的事件
                    canTalk = false;
                }
            }
        }
    }
}
