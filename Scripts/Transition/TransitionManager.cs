using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;
namespace MFarm.Transition //�ͳ����йصľͷŵ���������ռ���
{
    public class TransitionManager : Singleton<TransitionManager>,ISaveable
    {
        [SceneName]
        public string startSceneName = string.Empty;//��ʼ������Ĭ��Ϊ�գ�
        private CanvasGroup FadeCanvasGroup;//���뵭��PanelUI��CanvasGroup���
        bool isFade;//�жϵ��뵭����û����ɵı���

        public string GUID => GetComponent<DataGUID>().guid;
        protected override void Awake()
        {
            base.Awake();
            //��Ϸһ��ʼ��Ҫ����UI��������Ϊ�����ֻ����ʾһ����������ô��ֻ����ʾ��������
            //����Ҫһ��ʼ�ͼ���UI����
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//��ӽ��ӿ��б�
            FadeCanvasGroup =FindObjectOfType<CanvasGroup>();//��ȡCanvasGroup���
        }
        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
            EventHandler.EndGameEvent += OnEndGameEvent;//������Ϸ���¼�
        }
        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;//������Ϸ���¼�
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }
        //������Ϸж�س���
        private IEnumerator UnloadScene() 
        {
            EventHandler.CallBeforeSceneUnloadEvent();
            yield return Fade(1f);
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            yield return Fade(0);
        }

        private void OnStartNewGameEvent(int obj)
        {
            StartCoroutine(LoadSaveDataScene(startSceneName));
        }

        //�����л��������¼�
        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            if(!isFade)
                StartCoroutine (Transition(sceneToGo,positionToGo));
        }

        //�л�������Э��(�������ƣ���Ŀ��λ��)
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();//ж�س���ǰ������Ҫ����ĺ������¼�
            yield return Fade(1);//����������
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());//ж�ص�ǰ����ĳ���
            yield return LoadSceneSetActive(sceneName);//Э�̷��ص���һ���ӿ�,StartCoroutine�൱��ʹ�ýӿ�,�������ﲻ��д
            EventHandler.CallMovePosition(targetPosition);//�������غ������ƶ���ָ��λ�õ��¼� 
            EventHandler.CallAfterSceneUnloadEvent();//���س����������Ҫ����ĺ������¼�
            yield return Fade(0);//�������� 
        }
        //���ؼ������Э��
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            //LoadSceneMode.Additive����ģʽ������ԭ�еĳ������е��ӣ��������л�����
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);//���س���
            //GetSceneAt��ȡ�������б�
            //SceneManager.sceneCount�ܹ�����Ϸ�м��صĳ�������,-1�ͱ�ʾ��ǰ���س����������
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);//��ȡ��ǰ���صĳ���
            SceneManager.SetActiveScene(newScene);//���ǰ����
        }
        //���뵭����Э��
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;//��ʼ�仯
            FadeCanvasGroup.blocksRaycasts = true;//��ʼ�仯��ʲô�����ܵ��
            float speed=Mathf.Abs(FadeCanvasGroup.alpha-targetAlpha)/Settings.sceneFadeDuration;//�仯�ٶ�
            while (!Mathf.Approximately(FadeCanvasGroup.alpha, targetAlpha)) //�жϵ�ǰ��͸���Ⱥ�Ŀ��͸�����Ƿ�һ��
            {
                //MoveTowards,�ǵ�ǰֵ��Ŀ��ֵ�����ķ���
                //���ʺͲ�ֵ������������
                FadeCanvasGroup.alpha = Mathf.MoveTowards(FadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;  
            }
            FadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }
        //���ش洢�����ķ���
        private IEnumerator LoadSaveDataScene(string sceneName) 
        {   //���뽥��
            yield return Fade(1F);//1�Ǻڣ�0��͸��
            //�жϵ�ǰ������ǲ���������
            if (SceneManager.GetActiveScene().name != "SampleScene") //Ϊ������Ϸ�����м���������Ϸ����
            {
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);//ж�ص�ǰ����
            }
            yield return LoadSceneSetActive(sceneName);//����Ϸ
            EventHandler.CallAfterSceneUnloadEvent();
            yield return Fade(0);
        }
        //��������
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName=SceneManager.GetActiveScene().name;//���泡��
            return saveData;
        }
        //��������
        public void RestoreData(GameSaveData saveData)
        {
            //���ض�ȡ����
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}

