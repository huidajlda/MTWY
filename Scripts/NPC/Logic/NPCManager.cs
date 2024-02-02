using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : Singleton<NPCManager>
{
    public List<NPCPosition> npcPositionList;
    public SceneRouteDataList_SO sceneRouteDate;//路径数据
    private Dictionary<string,SceneRoute> sceneRouteDict=new Dictionary<string,SceneRoute>();//路径字典
    protected override void Awake()
    {
        base.Awake();
        InitSceneRouteDict();
    }
    private void OnEnable()
    {
        EventHandler.StartNewGameEvent += OnStartNewGameEvent;//开始新游戏的事件
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

    //初始化路径字典
    private void InitSceneRouteDict() 
    {
        if (sceneRouteDate.sceneRouteList.Count > 0) 
        {
            foreach (SceneRoute route in sceneRouteDate.sceneRouteList) //循环列表里的每一条路径
            {
                var key = route.fromSceneName + route.gotoSceneName;//两个场景名称相加作为Key
                if (sceneRouteDict.ContainsKey(key))//如果有就跳过
                    continue;
                else//没有就加进字典
                    sceneRouteDict.Add(key, route);
            }
        }
    }
    //查找字典两个场景中路径的方法
    public SceneRoute GetSceneRoute(string fromSceneName, string gotoSceneName) 
    {
        return sceneRouteDict[fromSceneName+gotoSceneName];
    }
}
