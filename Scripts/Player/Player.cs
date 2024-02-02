using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Save;
public class Player : MonoBehaviour,ISaveable
{
    private Rigidbody2D rb;//������ϵĸ������
    public float speed;//�ƶ��ٶ�
    private float inputX;//����X�᷽����ƶ�����
    private float inputY;//����Y�᷽����ƶ�����
    private Vector2 movementInput;//��ʵ�ƶ�����
    private Animator[] animators;//��ȡ���ϸ������ֵĶ���������
    private bool isMoving;//�Ƿ��ƶ�
    private bool inputDisable;//����Ƿ���Բ���
    //�����л�����
    private float mouseX;
    private float mouseY;
    private bool useTool;

    public string GUID => GetComponent<DataGUID>().guid;//�洢��ʶ

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();//��ȡ���
        animators=GetComponentsInChildren<Animator>();//�õ����������ϵ����ж������������
        inputDisable = true;
    }
    //ע��ӿڶ��ŵ�Start����
    private void Start()
    {
        ISaveable saveable = this;
        saveable.RegisterSaveable();//��ӽ��ӿ��б�
    }
    private void OnEnable()
    {
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneUnloadEvent += OnAfterSceneUnloadEvent;
        EventHandler.MovePosition += OnMovePosition;
        EventHandler.MouseClickedEvent += OnMouseClickedEvent;//������������¼�
        EventHandler.UpdateGameStateEvent += OnUpdateGameStateEvent;//ע���л���Ϸ״̬���¼�
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
        EventHandler.EndGameEvent += OnEndGameEvent;//������Ϸ���¼�
    }

    private void OnMouseClickedEvent(Vector3 mouseWolrdPos, ItemDetails details)
    {
        //ִ�ж���
        //�жϵ�ǰ���ʹ�õ���Ʒ�ǹ���
        if (details.itemType != ItemType.Seed && details.itemType != ItemType.Commodity && details.itemType != ItemType.Furniture) 
        {
            mouseX=mouseWolrdPos.x-transform.position.x;
            mouseY=mouseWolrdPos.y-(transform.position.y+0.85f);
            //����ִ���ĸ�����Ķ���,��blendtree֪��ִ���ĸ�����Ķ���(��Ϊû��б����Ķ���)
            if (Mathf.Abs(mouseX) > Mathf.Abs(mouseY))
                mouseY = 0;
            else mouseX = 0;
            //ִ�ж�����Э��(���������ö���ִ�й����У�����Ͳ����仯)
            StartCoroutine(UseToolRoutine(mouseWolrdPos, details));
        }
        else//������Ʒ�Ҿ�ֱ��ִ���¼�
            EventHandler.CallExecuteActionAfterAnimation(mouseWolrdPos, details);//ʵ��ִ�е��¼�
    }
    //������Э��
    private IEnumerator UseToolRoutine(Vector3 mouseWolrdPos, ItemDetails details) 
    {
        useTool = true;
        inputDisable = true;//����ʹ�ù��ߵĶ���ʱ����Ҳ����ƶ�
        yield return null;//ȷ�������ִ������
        foreach (var anim in animators) //ѭ������ÿ����λ�Ķ���������
        {
            anim.SetTrigger("useTool");//����ʹ�ù��ߵĶ���
            //����ʹ�ù��ߵķ���ת������ķ���
            anim.SetFloat("InputX", mouseX);
            anim.SetFloat("InputY", mouseY);
        }
        //�ȴ�һ��ʱ��
        yield return new WaitForSeconds(0.45f);//���ʱ���Ƕ�����ಥ�ŵ�ʹ�ù������������ʱ��
        //ʹ�ù��߲���Ч����ʱ��
        EventHandler.CallExecuteActionAfterAnimation(mouseWolrdPos, details);//ʵ��ִ�е��¼�
        yield return new WaitForSeconds(0.25f);//�ȴ�ִ�����
        //һ��ʹ�ý�������ҿ����ƶ�
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
    //������Ϸ���õķ���
    private void OnEndGameEvent()
    {
        inputDisable = true;//ֹͣ�������
    }

    //��ʼ����Ϸ���õķ���
    private void OnStartNewGameEvent(int obj)
    {
        inputDisable = false;
        transform.position = Settings.PlayerStartPos;
    }

    //�л���Ϸ״̬���¼�����
    private void OnUpdateGameStateEvent(GameState gameState)
    {
        switch (gameState) 
        {
            case GameState.GamePlay://������Ϸ
                inputDisable = false;//��ҿ��Բ���
                break;
            case GameState.Pause://��ͣ����Ҳ����ƶ���
                inputDisable = true;//��Ҳ��ܽ�������
                break;
        }
    }

    //�л�����ǰ�ķ���
    private void OnBeforeSceneUnloadEvent()
    {
        inputDisable = true;//���л�����ʱ���ܽ��в���
    }
    //�л�������ķ���
    private void OnAfterSceneUnloadEvent()
    {
        inputDisable=false;
    }
    //�л��������ƶ���ָ��λ��
    private void OnMovePosition(Vector3 targetPosition)
    {
        transform.position = targetPosition;
    }

    private void Update()
    {
        if(inputDisable==false)//false��ҿ��Բ���
            PlayerInput();//��ȡ������뷽��
        else
            isMoving = false;//�ƶ�����ֹͣ
        SwitchAnimation();

    }
    private void FixedUpdate()
    {
        if(!inputDisable)
            Movement();//��Ϊ�õĸ������ƶ�,������FixedUpdate���ñȽϺ�
    }
    //��ҵ����뷽��
    private void PlayerInput() 
    {
        inputX = Input.GetAxisRaw("Horizontal");//ˮƽ���������
        inputY = Input.GetAxisRaw("Vertical");//��ֱ���������
        if (inputX != 0 && inputY != 0)//˵����б���ߣ�����б�߱Ƚϳ������ٶȺ���ߵıȽϿ� 
        {
            //���Զ�б��ʱ�Ĵ�С�������ƣ���Ȼ���Բ�����
            inputX = inputX * 0.7f;//һ��֮���Ŷ�Լ����0.7
            inputY = inputY * 0.7f;
        }
        //������ס��Shift�л�Ϊ��·
        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputX = inputX * 0.5f;
            inputY = inputY * 0.5f;
        }
        movementInput =new Vector2 (inputX, inputY);//�������ƶ�����
        isMoving = movementInput != Vector2.zero;//�Ƿ��ƶ���ֵ
    }
    //��Ҹ��ݷ������ƶ�
    private void Movement() 
    {
        //�ø������ƶ�����Ҫ2D��Ծʱһ����AddForce���������һ�����ķ���
        //���ӽǵ��ƶ�ѡ��MovePosition���ƶ�������ķ��������е�����+�ƶ������꣩
        rb.MovePosition(rb.position+movementInput*speed*Time.deltaTime);
    }
    //�л������ĺ�������
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
    //��������
    public GameSaveData GenerateSaveData()
    {
        GameSaveData saveData=new GameSaveData();
        saveData.characterPosDict = new Dictionary<string, SerializableVector3>();
        saveData.characterPosDict.Add(this.name, new SerializableVector3(transform.position));//������������
        return saveData;
    }
    //��ȡ����
    public void RestoreData(GameSaveData saveData)
    {
        var targetPosition = saveData.characterPosDict[this.name].ToVector3();//ȡ������
        transform.position = targetPosition;
    }
}
