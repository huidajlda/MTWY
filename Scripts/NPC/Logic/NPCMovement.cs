using MFarm.AStar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MFarm.Save;
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class NPCMovement : MonoBehaviour,ISaveable
{
    public ScheduleDetails_SO scheduleData;//行程列表
    private SortedSet<ScheduleDetails> scheduleSet;//排序后的行程列表
    private ScheduleDetails currentSchedule;//当前的行程
    //临时储存的信息
    [SerializeField]private string currentScene;//当前场景
    private string targetScene;//目标场景
    private Vector3Int currentGridPosition;//当前网格位置
    private Vector3Int targetGridPosition;//目标网格坐标
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;
    public string StartScene { set => currentScene = value; }
    [Header("移动属性")]
    public float normalSpeed= 2f;//移动速度
    private float minSpeed = 1;//最小移动速度
    private float maxSpeed = 3;//最大移动速度
    private Vector2 dir;//方向
    public bool isMoving;//是否移动(动画)
    private bool npcMove;
    private Rigidbody2D rb;//刚体组件
    private SpriteRenderer spriteRenderer;//精灵渲染器
    private BoxCollider2D coll;//触发器
    private Animator anim;//动画控制器
    private Grid grid;//网格
    private Stack<MovementStep> movementSteps;
    private Coroutine npcMoveRoutine;//npc的协程
    private bool isInitialised;//是否已经加载过了这个NPC
    private bool sceneLoaded;
    public bool interactable;//是否可以互动
    public bool isFirstLoad;//是不是第一次加载人物
    private Season currentSeason;//游戏的季节
    //动画计时器
    private float animationBreakTime;
    private bool canPlayStopAnimation;
    private AnimationClip stopAnimationClip;//停止动画片段
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;
    private TimeSpan GameTime=>TimeManager.Instance.GameTime;//获取游戏时间

    public string GUID => GetComponent<DataGUID>().guid;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        movementSteps=new Stack<MovementStep>();
        animOverride = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController=animOverride;
        scheduleSet = new SortedSet<ScheduleDetails>();//初始化
        foreach (var schedule in scheduleData.scheduleList) 
        {
            scheduleSet.Add(schedule);
        }
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//添加进加载场景后的事件
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;//场景加载前NPC不能移动
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;//时刻的分钟事件
        EventHandler.EndGameEvent += OnEndGameEvent;//结束游戏的事件
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始游戏的事件
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;//时刻的分钟事件
        EventHandler.EndGameEvent -= OnEndGameEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        isInitialised = false;
        isFirstLoad = true;
    }

    private void OnEndGameEvent()
    {
        sceneLoaded = false;
        npcMove = false;
        if(npcMoveRoutine!=null)
            StopCoroutine(npcMoveRoutine);
    }

    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();//添加进接口列表
    }
    private void Update()
    {
        if (sceneLoaded)
            SwitchAnimation();
        animationBreakTime -= Time.deltaTime;
        canPlayStopAnimation = animationBreakTime <= 0;
    }
    private void FixedUpdate()
    {
        if(sceneLoaded)
            Movement();
    }
    private void OnGameMinuteEvent(int minute, int hour, int day, Season season)
    {
        int time = (hour * 100) + minute;
        currentSeason = season;
        ScheduleDetails matchSchedule = null;
        foreach (var schedule in scheduleSet) //找到符合时间的行程
        {
            if (schedule.Time == time)
            {
                if (schedule.day != day&&schedule.day!=0)
                    continue;
                if (schedule.season != season)
                    continue;
                matchSchedule = schedule;
            }
            else if (schedule.Time > time) 
                break;
        }
        if(matchSchedule!=null)
            BuildPath(matchSchedule);//创建路径
    }
    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded =false;
    }
    //加载场景后事件调用的方法
    private void OnAfterSceneUnloadEvent()
    {
        grid=FindObjectOfType<Grid>();//获取网格
        CheckVisiable();
        if (!isInitialised) 
        {
            InitNPC();//NPC初始化
            isInitialised = true;
        }
        sceneLoaded=true;
        if (!isFirstLoad) 
        {
            currentGridPosition = grid.WorldToCell(transform.position);
            var schedule = new ScheduleDetails(0, 0, 0, 0, currentSeason,targetScene,(Vector2Int)targetGridPosition,stopAnimationClip,interactable);
            BuildPath(schedule);
            isFirstLoad = true;
        }
    }
    //是否可见
    private void CheckVisiable() 
    {
        if(currentScene==SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }
    //初始化NPC
    private void InitNPC()
    {
        targetScene = currentScene;//目标场景等于当前的场景
        //保持在当前坐标的网格中心点
        currentGridPosition=grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f,
                                        currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition = currentGridPosition;
    }
    //移动的方法
    private void Movement() 
    {
        if (!npcMove) 
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();//拿出第一步
                currentScene = step.sceneName;
                CheckVisiable();//是否在该场景显示NPC
                nextGridPosition = (Vector3Int)step.gridCoordinate;//拿到下一步的网格坐标
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);//拿到这步的时间戳
                MoveToGridPosition(nextGridPosition, stepTime);//移动
            }
            else if (!isMoving&&canPlayStopAnimation) 
            {
                StartCoroutine(SetStopAnimation());
            }
        }
    }
    private void MoveToGridPosition(Vector3Int gridPos, TimeSpan stepTime) 
    {
        npcMoveRoutine=StartCoroutine(MoveRoutine(gridPos,stepTime));
    }
    //移动的协程
    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;//npc正在移动
        nextWorldPosition=GetWorldPosition(gridPos);
        if (stepTime > GameTime) //还有时间走到下一步
        {
            //用来移动的时间差，以秒为单位
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //实际移动距离
            float distance=Vector3.Distance(transform.position, nextWorldPosition);
            //实际移动速度
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));
            if (speed <= maxSpeed) 
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > 0.05f) 
                {
                    dir = (nextWorldPosition - transform.position).normalized;//移动方向
                    Vector2 posOffset=new Vector2(dir.x*speed*Time.fixedDeltaTime, dir.y*speed*Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);//移动
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //如果时间已经到了则瞬移过去
        rb.position=nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;//移动结束
    }
    //根据行程生成Astar路径的方法
    public void BuildPath(ScheduleDetails schedule) 
    {
        movementSteps.Clear();//先清空堆栈
        currentSchedule=schedule;//当前行程
        targetScene=schedule.targetScene;
        targetGridPosition=(Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;//拿到停止时的都会片段
        this.interactable=schedule.interactable;//是否可互动
        if (schedule.targetScene == currentScene) //同场景
        {
            //生成路径
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, 
                                      schedule.targetGridPosition, movementSteps);
        }
        else if (schedule.targetScene != currentScene)//跨场景
        {
            //拿到路径列表
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);
            //循环路径列表
            if (sceneRoute != null) 
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++) 
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];
                    if (path.fromGridCell.x >= Settings.maxGridSize) //就以NPC自身为坐标
                        fromPos = (Vector2Int)currentGridPosition;
                    else
                        fromPos = path.fromGridCell;
                    if (path.gotoGridCell.x >= Settings.maxGridSize) 
                        gotoPos = schedule.targetGridPosition;
                    else gotoPos = path.gotoGridCell;
                    AStar.Instance.BuildPath(path.sceneName, fromPos, gotoPos,movementSteps);//构建路径
                }
            }
        }
        //移动
        if (movementSteps.Count > 1) 
        {
            //更新每一步对应的时间戳
            UpdateTimeOnPath();
        }
    }
    //更新每一步的时间戳
    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;//上一步
        TimeSpan currentGameTime = GameTime;//拿到当前游戏时间
        foreach (MovementStep step in movementSteps)
        {
            if(previousStep==null)
                previousStep = step;
            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;
            TimeSpan gridMovementStepTime;//网格里移动一格的时间
            if (MoveInDiagonal(step, previousStep)) //判断是横着走还是斜着走
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            else//计算移动一个格子要的时间(上面是斜着走的时间，下面是横着走的时间)
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
            //把走过一格的时间加上
            currentGameTime = currentGameTime.Add(gridMovementStepTime);//下一格的时间戳
            //循环下一步
            previousStep = step;
        }
    }
    //判断是横着走还是斜着走格子
    private bool MoveInDiagonal(MovementStep currentStep,MovementStep previousStep) 
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x)&& (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }
    //网格坐标返回世界坐标中心点
    private Vector3 GetWorldPosition(Vector3Int gridPos) 
    {
        Vector3 worldPos=grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2);
    }
    //动画的切换
    private void SwitchAnimation() 
    {
        isMoving = transform.position != GetWorldPosition(targetGridPosition);
        anim.SetBool("isMoving", isMoving);
        if (isMoving) 
        {
            anim.SetBool("Exit", true);
            anim.SetFloat("DirX", dir.x);
            anim.SetFloat("DirY", dir.y);
        }
        else
            anim.SetBool("Exit", false);
    }
    //停止动画的协程
    private IEnumerator SetStopAnimation() 
    {
        //强制面向镜头
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);
        animationBreakTime = 5f;//5s计时器
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;//切换动画片段
            anim.SetBool("EventAnimation", true);
            yield return null;
            anim.SetBool("EventAnimation", false);
        }
        else 
        {
            animOverride[stopAnimationClip] = blankAnimationClip;
            anim.SetBool("EventAnimation", false);
        }
    }
    //设置NPC激活
    private void SetActiveInScene() 
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);//开启影子
    }
    //设置NPC失活
    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);//关闭影子
    }
    //保存数据
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        //保存目的位置
        saveData.characterPosDict.Add("targetGridPosition", new SerializableVector3(targetGridPosition));
        saveData.characterPosDict.Add("currentPosition", new SerializableVector3(transform.position));//保存位置
        saveData.dataSceneName = currentScene;//保存当前场景
        saveData.targetScene = this.targetScene;//保存目标场景
        if (stopAnimationClip != null) //保存NPC停下来后的动画片段
        {
            saveData.animationInstanceID=stopAnimationClip.GetInstanceID();
        }
        saveData.interactable = this.interactable;//保存是否可以互动
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("currentSeason", (int)currentSeason);
        return saveData;
    }
    //读取数据
    public void RestoreData(GameSaveData saveData)
    {
        isInitialised = true;//读取数据吧表示人物已经加载过了
        isFirstLoad = false;
        currentScene = saveData.dataSceneName;
        targetScene=saveData.targetScene;
        Vector3 pos = saveData.characterPosDict["currentPosition"].ToVector3();
        Vector3Int gridPos = (Vector3Int)saveData.characterPosDict["targetGridPosition"].ToVector2Int();
        transform.position= pos;
        targetGridPosition= gridPos;
        if (saveData.animationInstanceID != 0) 
        {
            this.stopAnimationClip=Resources.InstanceIDToObject(saveData.animationInstanceID)as AnimationClip;
        }
        this.interactable= saveData.interactable;
        this.currentSeason = (Season)saveData.timeDict["currentSeason"];
    }
}
