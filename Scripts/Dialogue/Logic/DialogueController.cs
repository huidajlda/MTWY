using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
namespace MFarm.Dialogue 
{
    //���ص�NPC����ʱȷ��NPC���������������
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        private NPCMovement npc => GetComponent<NPCMovement>();//�õ����ϵ�NPCMovement�ű����
        public UnityEvent OnFinshEvent;//�¼�
        public List<DialoguePiece> dialogueList=new List<DialoguePiece>();//�Ի����ݵ��б�
        private Stack<DialoguePiece> dailogueStack;//��ջ�Ի��б�(���Ի��б�����ѹ��ȥ��Ȼ��ÿ�δ�ͷ��ʼ���ͺ���)
        private bool canTalk;//���ԶԻ�
        private bool isTalking;//�Ƿ�����˵��
        private GameObject uiSign;//�ɻ�����ʾ�İ�ť
        private void Awake()
        {
            uiSign = transform.GetChild(1).gameObject;//��ȡ�ɻ�����ʾ�İ�ť
            FillDialogueStack();//���Ի�����ѹ��ջ��
        }
        private void Update()
        {
            uiSign.SetActive(canTalk);//���ü���ʧ��
            if (canTalk && Input.GetKeyDown(KeyCode.Space)&&!isTalking) //�Ұ��¿ո�
            {
                StartCoroutine(DialogueRoutine());
            }
        }
        private void OnTriggerEnter2D(Collider2D other)//���봥����
        {
            if (other.CompareTag("Player")) 
            {
                canTalk = !npc.isMoving && npc.interactable;//npcû���ƶ��ҿ��Ի�����ô�Ϳ��ԶԻ�
            }
        }
        private void OnTriggerExit2D(Collider2D other)//�뿪������
        {
            if (other.CompareTag("Player"))
            {
                canTalk = false;//�����ԶԻ�
            }
        }
        //���Ի�����ѹ��ջ��
        private void FillDialogueStack() 
        {
            dailogueStack = new Stack<DialoguePiece>();//����ջ
            //����ѹջ��������ջ��ȡ������������
            for (int i = dialogueList.Count - 1; i > -1; i--) 
            {
                dialogueList[i].isDone = false;//�Ի�û�н���
                dailogueStack.Push(dialogueList[i]);//ѹ��ջ
            }
        }
        private IEnumerator DialogueRoutine() 
        {
            isTalking = true;//��˵����
            if (dailogueStack.TryPop(out DialoguePiece result)) //ֻҪջ�����оͻ�һֱ��
            {
                //����UI��ʾ�Ի�
                EventHandler.CallShowDialogueEvent(result);
                EventHandler.CallUpdateGameStateEvent(GameState.Pause);//�Ի�ʱ��ͣ�ƶ�
                yield return new WaitUntil(() => result.isDone);//�Ի���ʾ��Ż����
                isTalking = false;
            }
            else 
            {
                EventHandler.CallUpdateGameStateEvent(GameState.GamePlay);//�Ի������������ƶ�
                EventHandler.CallShowDialogueEvent(null);//���վͿ��Թر���
                FillDialogueStack();//�ٴ���䣬���Լ����Ի�
                isTalking= false;
                if (OnFinshEvent != null) 
                {
                    OnFinshEvent?.Invoke();//���������Ի�����¼�
                    canTalk = false;
                }
            }
        }
    }
}
