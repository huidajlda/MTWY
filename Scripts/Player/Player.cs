using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
public class Player : MonoBehaviour,ISaveable
{
    private Rigidbody2D rb;//玩家身上的刚体组件
    public float speed;//移动速度
    private float inputX;//键盘X轴方向的移动输入
    private float inputY;//键盘Y轴方向的移动输入
    private Vector2 movementInput;//真实移动方向
    private Animator[] animators;//获取身上各个部分的动画控制器
    private bool isMoving;//是否移动
    private bool inputDisable;//玩家是否可以操作
    //动画切换工具
    private float mouseX;
    private float mouseY;
    private bool useTool;

    public string GUID => GetComponent<DataGUID>().guid;//存储标识

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();//获取组件
        animators=GetComponentsInChildren<Animator>();//拿到子物体身上的所有动画控制器组件
        inputDisable = true;
    }
    //注册接口都放到Start里面
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();//添加进接口列表
    }
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;
        EventHandler.MovePosition += OnMovePosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;//鼠标点击触发的事件
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;//注册切换游戏状态的事件
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
        EventHandler.EndGameEvent += OnEndGameEvent;//结束游戏的事件
    }

    private void OnMouseClickedEvent(Vector3 mouseWolrdPos, ItemDetails details)
    {
        //执行动画
        //判断当前鼠标使用的物品是工具
        if (details.itemType != ItemType.Seed && details.itemType != ItemType.Commodity && details.itemType != ItemType.Furniture) 
        {
            mouseX=mouseWolrdPos.x-transform.position.x;
            mouseY=mouseWolrdPos.y-(transform.position.y+0.85f);
            //优先执行哪个方向的动画,让blendtree知道执行哪个方向的动画(因为没有斜方向的动画)
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else mouseX = 0;
            //执行动画的协程(这样可以让动画执行过程中，地面就产生变化)
            StartCoroutine(UseToolRoutine(mouseWolrdPos, details));
        }
        else//种子商品家具直接执行事件
            EventHandler.CallExecuteActionAfterAnimation(mouseWolrdPos, details);//实际执行的事件
    }
    //动画的协程
    private IEnumerator UseToolRoutine(Vector3 mouseWolrdPos, ItemDetails details) 
    {
        useTool = true;
        inputDisable = true;//播放使用工具的动画时，玩家不能移动
        yield return null;//确保上面的执行完了
        foreach (var anim in animators) //循环身体每个部位的动画控制器
        {
            anim.SetTrigger("useTool");//触发使用工具的动画
            //根据使用工具的方向转动人物的方向
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        //等待一段时间
        yield return new WaitForSeconds(0.45f);//这个时间是动画差不多播放到使用工具碰到地面的时间
        //使用工具产生效果的时间
        EventHandler.CallExecuteActionAfterAnimation(mouseWolrdPos, details);//实际执行的事件
        yield return new WaitForSeconds(0.25f);//等待执行完成
        //一次使用结束，玩家可以移动
        useTool = false;
        inputDisable=false;
    }
    private void OnDisable()
    {
        EventHandler.BeforeSceneUnloadEvent -= OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneUnloadEvent -= OnAfterSceneUnloadEvent;
        EventHandler.MovePosition -= OnMovePosition;
        EventHandler.UpdateGameStateEvent -= OnUpdateGameStateEvent;
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
        EventHandler.EndGameEvent -= OnEndGameEvent;
    }
    //结束游戏调用的方法
    private void OnEndGameEvent()
    {
        inputDisable = true;//停止玩家输入
    }

    //开始新游戏调用的方法
    private void OnStartNewGameEvent(int obj)
    {
        inputDisable = false;
        transform.position = Settings.PlayerStartPos;
    }

    //切换游戏状态的事件方法
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState) 
        {
            case GameState.GamePlay://正常游戏
                inputDisable = false;//玩家可以操作
                break;
            case GameState.Pause://暂停（玩家不能移动）
                inputDisable = true;//玩家不能进行输入
                break;
        }
    }

    //切换场景前的方法
    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;//在切换场景时不能进行操作
    }
    //切换场景后的方法
    private void OnAfterSceneUnloadEvent()
    {
        inputDisable=false;
    }
    //切换场景后移动到指定位置
    private void OnMovePosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void Update()
    {
        if(inputDisable==false)//false玩家可以操作
            PlayerInput();//获取玩家输入方向
        else
            isMoving = false;//移动动画停止
        SwitchAnimation();

    }
    private void FixedUpdate()
    {
        if(!inputDisable)
            Movement();//因为用的刚体来移动,所以在FixedUpdate调用比较好
    }
    //玩家的输入方向
    private void PlayerInput() 
    {
        inputX = Input.GetAxisRaw("Horizontal");//水平方向的输入
        inputY = Input.GetAxisRaw("Vertical");//垂直方向的输入
        if (inputX != 0 && inputY != 0)//说明是斜方走，但是斜线比较长乘于速度后会走的比较快 
        {
            //所以对斜线时的大小进行限制，当然可以不限制
            inputX = inputX * 0.7f;//一份之根号二约等于0.7
            inputY = inputY * 0.7f;
        }
        //持续按住左Shift切换为走路
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput =new Vector2 (inputX, inputY);//真正的移动方向
        isMoving = movementInput != Vector2.zero;//是否移动赋值
    }
    //玩家根据方向来移动
    private void Movement() 
    {
        //用刚体来移动，需要2D跳跃时一般用AddForce给物体添加一个力的方法
        //俯视角的移动选着MovePosition，移动其坐标的方法（现有的坐标+移动的坐标）
        rb.MovePosition(rb.position+movementInput*speed*Time.deltaTime);
    }
    //切换动画的函数方法
    private void SwitchAnimation() 
    {
        foreach (var anim in animators) 
        {
            anim.SetBool("isMoving", isMoving);
            anim.SetFloat("mouseX", mouseX);
            anim.SetFloat("mouseY", mouseY);
            if (isMoving) 
            {
                anim.SetFloat("InputX", inputX);
                anim.SetFloat("InputY", inputY);
            }
        }
    }
    //保存数据
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData=new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));//保存人物坐标
        return saveData;
    }
    //读取数据
    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[this.name].ToVector3();//取出数据
        transform.position = targetPosition;
    }
}
