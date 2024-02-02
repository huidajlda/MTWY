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
    public ScheduleDetails_SO scheduleData;//�г��б�
    private SortedSet<ScheduleDetails> scheduleSet;//�������г��б�
    private ScheduleDetails currentSchedule;//��ǰ���г�
    //��ʱ�������Ϣ
    [SerializeField]private string currentScene;//��ǰ����
    private string targetScene;//Ŀ�곡��
    private Vector3Int currentGridPosition;//��ǰ����λ��
    private Vector3Int targetGridPosition;//Ŀ����������
    private Vector3Int nextGridPosition;
    private Vector3 nextWorldPosition;
    public string StartScene { set => currentScene = value; }
    [Header("�ƶ�����")]
    public float normalSpeed= 2f;//�ƶ��ٶ�
    private float minSpeed = 1;//��С�ƶ��ٶ�
    private float maxSpeed = 3;//����ƶ��ٶ�
    private Vector2 dir;//����
    public bool isMoving;//�Ƿ��ƶ�(����)
    private bool npcMove;
    private Rigidbody2D rb;//�������
    private SpriteRenderer spriteRenderer;//������Ⱦ��
    private BoxCollider2D coll;//������
    private Animator anim;//����������
    private Grid grid;//����
    private Stack<MovementStep> movementSteps;
    private Coroutine npcMoveRoutine;//npc��Э��
    private bool isInitialised;//�Ƿ��Ѿ����ع������NPC
    private bool sceneLoaded;
    public bool interactable;//�Ƿ���Ի���
    public bool isFirstLoad;//�ǲ��ǵ�һ�μ�������
    private Season currentSeason;//��Ϸ�ļ���
    //������ʱ��
    private float animationBreakTime;
    private bool canPlayStopAnimation;
    private AnimationClip stopAnimationClip;//ֹͣ����Ƭ��
    public AnimationClip blankAnimationClip;
    private AnimatorOverrideController animOverride;
    private TimeSpan GameTime=>TimeManager.Instance.GameTime;//��ȡ��Ϸʱ��

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
        scheduleSet = new SortedSet<ScheduleDetails>();//��ʼ��
        foreach (var schedule in scheduleData.scheduleList) 
        {
            scheduleSet.Add(schedule);
        }
    }
    private void OnEnable()
    {
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;//��ӽ����س�������¼�
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;//��������ǰNPC�����ƶ�
        EventHandler.GameMinuteEvent += OnGameMinuteEvent;//ʱ�̵ķ����¼�
        EventHandler.EndGameEvent += OnEndGameEvent;//������Ϸ���¼�
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ��Ϸ���¼�
    }
    private void OnDisable()
    {
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.GameMinuteEvent -= OnGameMinuteEvent;//ʱ�̵ķ����¼�
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
        saveable.RegisterSaveable();//��ӽ��ӿ��б�
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
        foreach (var schedule in scheduleSet) //�ҵ�����ʱ����г�
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
            BuildPath(matchSchedule);//����·��
    }
    private void OnBeforeSceneUnloadEvent()
    {
        sceneLoaded =false;
    }
    //���س������¼����õķ���
    private void OnAfterSceneUnloadEvent()
    {
        grid=FindObjectOfType<Grid>();//��ȡ����
        CheckVisiable();
        if (!isInitialised) 
        {
            InitNPC();//NPC��ʼ��
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
    //�Ƿ�ɼ�
    private void CheckVisiable() 
    {
        if(currentScene==SceneManager.GetActiveScene().name)
            SetActiveInScene();
        else
            SetInactiveInScene();
    }
    //��ʼ��NPC
    private void InitNPC()
    {
        targetScene = currentScene;//Ŀ�곡�����ڵ�ǰ�ĳ���
        //�����ڵ�ǰ������������ĵ�
        currentGridPosition=grid.WorldToCell(transform.position);
        transform.position = new Vector3(currentGridPosition.x + Settings.gridCellSize / 2f,
                                        currentGridPosition.y + Settings.gridCellSize / 2f, 0);
        targetGridPosition = currentGridPosition;
    }
    //�ƶ��ķ���
    private void Movement() 
    {
        if (!npcMove) 
        {
            if (movementSteps.Count > 0)
            {
                MovementStep step = movementSteps.Pop();//�ó���һ��
                currentScene = step.sceneName;
                CheckVisiable();//�Ƿ��ڸó�����ʾNPC
                nextGridPosition = (Vector3Int)step.gridCoordinate;//�õ���һ������������
                TimeSpan stepTime = new TimeSpan(step.hour, step.minute, step.second);//�õ��ⲽ��ʱ���
                MoveToGridPosition(nextGridPosition, stepTime);//�ƶ�
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
    //�ƶ���Э��
    private IEnumerator MoveRoutine(Vector3Int gridPos, TimeSpan stepTime)
    {
        npcMove = true;//npc�����ƶ�
        nextWorldPosition=GetWorldPosition(gridPos);
        if (stepTime > GameTime) //����ʱ���ߵ���һ��
        {
            //�����ƶ���ʱ������Ϊ��λ
            float timeToMove = (float)(stepTime.TotalSeconds - GameTime.TotalSeconds);
            //ʵ���ƶ�����
            float distance=Vector3.Distance(transform.position, nextWorldPosition);
            //ʵ���ƶ��ٶ�
            float speed = Mathf.Max(minSpeed, (distance / timeToMove / Settings.secondThreshold));
            if (speed <= maxSpeed) 
            {
                while (Vector3.Distance(transform.position, nextWorldPosition) > 0.05f) 
                {
                    dir = (nextWorldPosition - transform.position).normalized;//�ƶ�����
                    Vector2 posOffset=new Vector2(dir.x*speed*Time.fixedDeltaTime, dir.y*speed*Time.fixedDeltaTime);
                    rb.MovePosition(rb.position + posOffset);//�ƶ�
                    yield return new WaitForFixedUpdate();
                }
            }
        }
        //���ʱ���Ѿ�������˲�ƹ�ȥ
        rb.position=nextWorldPosition;
        currentGridPosition = gridPos;
        nextGridPosition = currentGridPosition;
        npcMove = false;//�ƶ�����
    }
    //�����г�����Astar·���ķ���
    public void BuildPath(ScheduleDetails schedule) 
    {
        movementSteps.Clear();//����ն�ջ
        currentSchedule=schedule;//��ǰ�г�
        targetScene=schedule.targetScene;
        targetGridPosition=(Vector3Int)schedule.targetGridPosition;
        stopAnimationClip = schedule.clipAtStop;//�õ�ֹͣʱ�Ķ���Ƭ��
        this.interactable=schedule.interactable;//�Ƿ�ɻ���
        if (schedule.targetScene == currentScene) //ͬ����
        {
            //����·��
            AStar.Instance.BuildPath(schedule.targetScene, (Vector2Int)currentGridPosition, 
                                      schedule.targetGridPosition, movementSteps);
        }
        else if (schedule.targetScene != currentScene)//�糡��
        {
            //�õ�·���б�
            SceneRoute sceneRoute = NPCManager.Instance.GetSceneRoute(currentScene, schedule.targetScene);
            //ѭ��·���б�
            if (sceneRoute != null) 
            {
                for (int i = 0; i < sceneRoute.scenePathList.Count; i++) 
                {
                    Vector2Int fromPos, gotoPos;
                    ScenePath path = sceneRoute.scenePathList[i];
                    if (path.fromGridCell.x >= Settings.maxGridSize) //����NPC����Ϊ����
                        fromPos = (Vector2Int)currentGridPosition;
                    else
                        fromPos = path.fromGridCell;
                    if (path.gotoGridCell.x >= Settings.maxGridSize) 
                        gotoPos = schedule.targetGridPosition;
                    else gotoPos = path.gotoGridCell;
                    AStar.Instance.BuildPath(path.sceneName, fromPos, gotoPos,movementSteps);//����·��
                }
            }
        }
        //�ƶ�
        if (movementSteps.Count > 1) 
        {
            //����ÿһ����Ӧ��ʱ���
            UpdateTimeOnPath();
        }
    }
    //����ÿһ����ʱ���
    private void UpdateTimeOnPath()
    {
        MovementStep previousStep = null;//��һ��
        TimeSpan currentGameTime = GameTime;//�õ���ǰ��Ϸʱ��
        foreach (MovementStep step in movementSteps)
        {
            if(previousStep==null)
                previousStep = step;
            step.hour = currentGameTime.Hours;
            step.minute = currentGameTime.Minutes;
            step.second = currentGameTime.Seconds;
            TimeSpan gridMovementStepTime;//�������ƶ�һ���ʱ��
            if (MoveInDiagonal(step, previousStep)) //�ж��Ǻ����߻���б����
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellDiagonalSize / normalSpeed / Settings.secondThreshold));
            else//�����ƶ�һ������Ҫ��ʱ��(������б���ߵ�ʱ�䣬�����Ǻ����ߵ�ʱ��)
                gridMovementStepTime = new TimeSpan(0, 0, (int)(Settings.gridCellSize / normalSpeed / Settings.secondThreshold));
            //���߹�һ���ʱ�����
            currentGameTime = currentGameTime.Add(gridMovementStepTime);//��һ���ʱ���
            //ѭ����һ��
            previousStep = step;
        }
    }
    //�ж��Ǻ����߻���б���߸���
    private bool MoveInDiagonal(MovementStep currentStep,MovementStep previousStep) 
    {
        return (currentStep.gridCoordinate.x != previousStep.gridCoordinate.x)&& (currentStep.gridCoordinate.y != previousStep.gridCoordinate.y);
    }
    //�������귵�������������ĵ�
    private Vector3 GetWorldPosition(Vector3Int gridPos) 
    {
        Vector3 worldPos=grid.CellToWorld(gridPos);
        return new Vector3(worldPos.x + Settings.gridCellSize / 2f, worldPos.y + Settings.gridCellSize / 2);
    }
    //�������л�
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
    //ֹͣ������Э��
    private IEnumerator SetStopAnimation() 
    {
        //ǿ������ͷ
        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", -1);
        animationBreakTime = 5f;//5s��ʱ��
        if (stopAnimationClip != null)
        {
            animOverride[blankAnimationClip] = stopAnimationClip;//�л�����Ƭ��
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
    //����NPC����
    private void SetActiveInScene() 
    {
        spriteRenderer.enabled = true;
        coll.enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);//����Ӱ��
    }
    //����NPCʧ��
    private void SetInactiveInScene()
    {
        spriteRenderer.enabled = false;
        coll.enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);//�ر�Ӱ��
    }
    //��������
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData = new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        //����Ŀ��λ��
        saveData.characterPosDict.Add("targetGridPosition", new SerializableVector3(targetGridPosition));
        saveData.characterPosDict.Add("currentPosition", new SerializableVector3(transform.position));//����λ��
        saveData.dataSceneName = currentScene;//���浱ǰ����
        saveData.targetScene = this.targetScene;//����Ŀ�곡��
        if (stopAnimationClip != null) //����NPCͣ������Ķ���Ƭ��
        {
            saveData.animationInstanceID=stopAnimationClip.GetInstanceID();
        }
        saveData.interactable = this.interactable;//�����Ƿ���Ի���
        saveData.timeDict = new Dictionary<string, int>();
        saveData.timeDict.Add("currentSeason", (int)currentSeason);
        return saveData;
    }
    //��ȡ����
    public void RestoreData(GameSaveData saveData)
    {
        isInitialised = true;//��ȡ���ݰɱ�ʾ�����Ѿ����ع���
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
