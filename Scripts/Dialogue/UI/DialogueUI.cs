using MFarm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;//UI�Ի���
    public Text dialogueText;//�Ի��ı�
    public Image faceRight, faceLeft;//���ҶԻ�������ͼƬ
    public Text nameRight,nameLeft;//���ҶԻ�������������ı�
    public GameObject continueBox;//���ո�������ı�����
    private void Awake()
    {
        continueBox.SetActive(false);//Ĭ�ϲ���ʾ���ո����
    }
    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDialogueEvent;
    }
    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
    }

    private void OnShowDialogueEvent(DialoguePiece piece)
    {
        StartCoroutine(showDialogue(piece));//����Э��
    }
    //��ʾ�Ի���Э��
    private IEnumerator showDialogue(DialoguePiece piece) 
    {
        if (piece != null)
        {
            piece.isDone = false;
            dialogueBox.SetActive(true);//��ʾ�Ի���
            continueBox.SetActive(false);//�رվͼ�����ť
            dialogueText.text = string.Empty;//��նԻ���
            if (piece.name != string.Empty)//���ֲ�Ϊ��
            {
                if (piece.onLeft)//�Ƿ���ʾ�����
                {
                    faceRight.gameObject.SetActive(false);//�ұ�ͷ��ر�
                    faceLeft.gameObject.SetActive(true);//���ͷ����ʾ
                    faceLeft.sprite = piece.faceImage;//��ʾ���ͼƬ
                    nameLeft.text = piece.name;//��ʾ�������
                }
                else
                {
                    faceRight.gameObject.SetActive(true);//�ұ�ͷ����ʾ
                    faceLeft.gameObject.SetActive(false);//���ͷ��ر�
                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else //û�����ֵĻ��Ͷ��ر�
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();//�ȴ��Ի����
            piece.isDone = true;//�Ի�����
            if (piece.hasToPause && piece.isDone)
                continueBox.SetActive(true);//��ʾ�����Ի���ť
        }
        else 
        {
            dialogueBox.SetActive(false);//�رնԻ���
            yield break;
        }
    }
}
