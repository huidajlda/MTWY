using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MFarm.Transition;
namespace MFarm.Save 
{
    public class DataSlot//进度条类
    {
        //保存的进度条(string,就是GUID)
        public Dictionary<string, GameSaveData> dataDict=new Dictionary<string, GameSaveData>();
        #region 用来UI显示进度详情
        public string DataTime//时间
        {
            get
            {
                var key = TimeManager.Instance.GUID;
                if (dataDict.ContainsKey(key)) 
                {
                    var timeData = dataDict[key];
                    return timeData.timeDict["gameYear"] + "年/" 
                        + (Season)timeData.timeDict["gameSeason"] + "/" 
                        + timeData.timeDict["gameMonth"] + "月/" 
                        + timeData.timeDict["gameDay"] + "日/";
                }
                else return string.Empty;
            }
        }
        public string DataScene //场景名称
        {
            get 
            {
                var key = TransitionManager.Instance.GUID;
                if (dataDict.ContainsKey(key)) 
                {
                    var transitionData = dataDict[key];
                    return transitionData.dataSceneName switch
                    {
                        "00.Start"=>"海边",
                        "01.Field"=>"农场",
                        "02.Home"=>"小木屋",
                        "03.Staff"=>"市场",
                        _=>string.Empty,
                    };
                }
                else { return string.Empty; }
            }
        }
        #endregion
    }
}
