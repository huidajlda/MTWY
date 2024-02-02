using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;
namespace MFarm.Transition //和场景有关的就放到这个命名空间下
{
    public class TransitionManager : Singleton<TransitionManager>,ISaveable
    {
        [SceneName]
        public string startSceneName = string.Empty;//初始场景（默认为空）
        private CanvasGroup FadeCanvasGroup;//淡入淡出PanelUI的CanvasGroup组件
        bool isFade;//判断淡入淡出有没有完成的变量

        public string GUID => GetComponent<DataGUID>().guid;
        protected override void Awake()
        {
            base.Awake();
            //游戏一开始就要加载UI场景，因为打包后只能显示一个场景，那么就只会显示出主场景
            //所有要一开始就加载UI场景
            SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        }

        private void Start()
        {
            ISaveable saveable = this;
            saveable.RegisterSaveable();//添加进接口列表
            FadeCanvasGroup =FindObjectOfType<CanvasGroup>();//获取CanvasGroup组件
        }
        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
            EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
            EventHandler.EndGameEvent += OnEndGameEvent;//结束游戏的事件
        }
        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
            EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
            EventHandler.EndGameEvent -= OnEndGameEvent;//结束游戏的事件
        }

        private void OnEndGameEvent()
        {
            StartCoroutine(UnloadScene());
        }
        //结束游戏卸载场景
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

        //调用切换场景的事件
        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            if(!isFade)
                StartCoroutine (Transition(sceneToGo,positionToGo));
        }

        //切换场景的协程(场景名称，和目标位置)
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();//卸载场景前调用需要处理的函数的事件
            yield return Fade(1);//场景淡入变黑
            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());//卸载当前激活的场景
            yield return LoadSceneSetActive(sceneName);//协程返回的是一个接口,StartCoroutine相当于使用接口,所以这里不用写
            EventHandler.CallMovePosition(targetPosition);//场景加载后人物移动到指定位置的事件 
            EventHandler.CallAfterSceneUnloadEvent();//加载场景后调用需要处理的函数的事件
            yield return Fade(0);//场景淡出 
        }
        //加载激活场景的协程
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            //LoadSceneMode.Additive加载模式就是在原有的场景进行叠加，而不是切换场景
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);//加载场景
            //GetSceneAt获取场景的列表
            //SceneManager.sceneCount总共在游戏中加载的场景数量,-1就表示当前加载出场景的序号
            Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);//获取当前加载的场景
            SceneManager.SetActiveScene(newScene);//激活当前场景
        }
        //淡入淡出的协程
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;//开始变化
            FadeCanvasGroup.blocksRaycasts = true;//开始变化是什么都不能点击
            float speed=Mathf.Abs(FadeCanvasGroup.alpha-targetAlpha)/Settings.sceneFadeDuration;//变化速度
            while (!Mathf.Approximately(FadeCanvasGroup.alpha, targetAlpha)) //判断当前的透明度和目标透明度是否一样
            {
                //MoveTowards,是当前值向目标值靠近的方法
                //本质和插值方法作用相似
                FadeCanvasGroup.alpha = Mathf.MoveTowards(FadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;  
            }
            FadeCanvasGroup.blocksRaycasts = false;
            isFade = false;
        }
        //加载存储场景的方法
        private IEnumerator LoadSaveDataScene(string sceneName) 
        {   //渐入渐出
            yield return Fade(1F);//1是黑，0是透明
            //判断当前激活场景是不是主场景
            if (SceneManager.GetActiveScene().name != "SampleScene") //为了在游戏过程中加载另外游戏进度
            {
                EventHandler.CallBeforeSceneUnloadEvent();
                yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);//卸载当前场景
            }
            yield return LoadSceneSetActive(sceneName);//新游戏
            EventHandler.CallAfterSceneUnloadEvent();
            yield return Fade(0);
        }
        //保存数据
        public GameSaveData GenerateSaveData()
        {
            GameSaveData saveData = new GameSaveData();
            saveData.dataSceneName=SceneManager.GetActiveScene().name;//保存场景
            return saveData;
        }
        //加载数据
        public void RestoreData(GameSaveData saveData)
        {
            //加载读取场景
            StartCoroutine(LoadSaveDataScene(saveData.dataSceneName));
        }
    }
}

