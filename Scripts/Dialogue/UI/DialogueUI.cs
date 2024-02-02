using MFarm.Dialogue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;//UI对话框
    public Text dialogueText;//对话文本
    public Image faceRight, faceLeft;//左右对话的人物图片
    public Text nameRight,nameLeft;//左右对话的人物的名称文本
    public GameObject continueBox;//按空格继续的文本内容
    private void Awake()
    {
        continueBox.SetActive(false);//默认不显示按空格继续
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
        StartCoroutine(showDialogue(piece));//开启协程
    }
    //显示对话的协程
    private IEnumerator showDialogue(DialoguePiece piece) 
    {
        if (piece != null)
        {
            piece.isDone = false;
            dialogueBox.SetActive(true);//显示对话框
            continueBox.SetActive(false);//关闭就继续按钮
            dialogueText.text = string.Empty;//清空对话框
            if (piece.name != string.Empty)//名字不为空
            {
                if (piece.onLeft)//是否显示在左边
                {
                    faceRight.gameObject.SetActive(false);//右边头像关闭
                    faceLeft.gameObject.SetActive(true);//左边头像显示
                    faceLeft.sprite = piece.faceImage;//显示左边图片
                    nameLeft.text = piece.name;//显示左边名称
                }
                else
                {
                    faceRight.gameObject.SetActive(true);//右边头像显示
                    faceLeft.gameObject.SetActive(false);//左边头像关闭
                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else //没有名字的话就都关闭
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }
            yield return dialogueText.DOText(piece.dialogueText, 1f).WaitForCompletion();//等待对话完成
            piece.isDone = true;//对话结束
            if (piece.hasToPause && piece.isDone)
                continueBox.SetActive(true);//显示继续对话按钮
        }
        else 
        {
            dialogueBox.SetActive(false);//关闭对话框
            yield break;
        }
    }
}
