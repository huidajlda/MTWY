using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;//菜单画板
    public GameObject menuPrefab;//Panel预设体
    public Button settingsBtn;//获取右上角的按钮
    public GameObject pausePanel;//暂停面板
    public Slider volumeSlider;//滑动条
    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);
        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//注册加载场景后的事件
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
    }
    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        //一开始生成UI
        Instantiate(menuPrefab, menuCanvas.transform);
    }
    private void OnAfterSceneUnloadEvent()
    {
        if (menuCanvas.transform.childCount > 0)
            Destroy(menuCanvas.transform.GetChild(0).gameObject);//删除UI
    }
    //启动暂停面板
    private void TogglePausePanel() 
    {
        bool isOpen = pausePanel.activeInHierarchy;//是否启动
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else 
        {
            System.GC.Collect();//暂停时垃圾回收
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    //返回主菜单
    public void ReturnMenuCanvas() 
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu() 
    {
        pausePanel.SetActive(false);
        EventHandler.CallEndGameEvent();//呼叫结束游戏的事件
        yield return new WaitForSeconds(1);
        Instantiate(menuPrefab, menuCanvas.transform);
    }
}
