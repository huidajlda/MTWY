using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameObject menuCanvas;//�˵�����
    public GameObject menuPrefab;//PanelԤ����
    public Button settingsBtn;//��ȡ���Ͻǵİ�ť
    public GameObject pausePanel;//��ͣ���
    public Slider volumeSlider;//������
    private void Awake()
    {
        settingsBtn.onClick.AddListener(TogglePausePanel);
        volumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//ע����س�������¼�
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
    }
    private void Start()
    {
        menuCanvas = GameObject.FindWithTag("MenuCanvas");
        //һ��ʼ����UI
        Instantiate(menuPrefab, menuCanvas.transform);
    }
    private void OnAfterSceneUnloadEvent()
    {
        if (menuCanvas.transform.childCount > 0)
            Destroy(menuCanvas.transform.GetChild(0).gameObject);//ɾ��UI
    }
    //������ͣ���
    private void TogglePausePanel() 
    {
        bool isOpen = pausePanel.activeInHierarchy;//�Ƿ�����
        if (isOpen)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else 
        {
            System.GC.Collect();//��ͣʱ��������
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }
    //�������˵�
    public void ReturnMenuCanvas() 
    {
        Time.timeScale = 1;
        StartCoroutine(BackToMenu());
    }
    private IEnumerator BackToMenu() 
    {
        pausePanel.SetActive(false);
        EventHandler.CallEndGameEvent();//���н�����Ϸ���¼�
        yield return new WaitForSeconds(1);
        Instantiate(menuPrefab, menuCanvas.transform);
    }
}
