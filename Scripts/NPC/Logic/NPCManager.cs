using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public List<NPCPosition> npcPositionList;
    public SceneRouteDataList_SO sceneRouteDate;//·������
    private Dictionary<string,SceneRoute> sceneRouteDict=new Dictionary<string,SceneRoute>();//·���ֵ�
    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }
    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//��ʼ����Ϸ���¼�
    }
    private void OnDisable()
    {
        EventHandler.StartNewGameEvent -= OnStartNewGameEvent;
    }

    private void OnStartNewGameEvent(int obj)
    {
        foreach (var character in npcPositionList) 
        {
            character.npc.position = character.position;
            character.npc.GetComponent<NPCMovement>().StartScene = character.startScene;
        }
    }

    //��ʼ��·���ֵ�
    private void InitSceneRouteDict() 
    {
        if (sceneRouteDate.sceneRouteList.Count > 0) 
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList) //ѭ���б����ÿһ��·��
            {
                var key = route.fromSceneName + route.gotoSceneName;//�����������������ΪKey
                if (sceneRouteDict.ContainsKey(key))//����о�����
                    continue;
                else//û�оͼӽ��ֵ�
                    sceneRouteDict.Add(key, route);
            }
        }
    }
    //�����ֵ�����������·���ķ���
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName) 
    {
        return sceneRouteDict[fromSceneName+gotoSceneName];
    }
}
