using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Transition;
namespace MFarm.Save 
{
    public class DataSlot//��������
    {
        //����Ľ�����(string,����GUID)
        public Dictionary<string, GameSaveData> dataDict=new Dictionary<string, GameSaveData>();
        #region ����UI��ʾ��������
        public string DataTime//ʱ��
        {
            get
            {
                var key = TimeManager.Instance.GUID;
                if (dataDict.ContainsKey(key)) 
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "��/" 
                        + (Season)timeData.timeDict["gameSeason"] + "/" 
                        + timeData.timeDict["gameMonth"] + "��/" 
                        + timeData.timeDict["gameDay"] + "��/";
                }
                else return string.Empty;
            }
        }
        public string DataScene //��������
        {
            get 
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key)) 
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        "00.Start"=>"����",
                        "01.Field"=>"ũ��",
                        "02.Home"=>"Сľ��",
                        "03.Staff"=>"�г�",
                        _=>string.Empty,
                    };
                }
                else { return string.Empty; }
            }
        }
        #endregion
    }
}
