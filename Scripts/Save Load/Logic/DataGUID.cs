using UnityEngine;
[ExecuteAlways]//一直运行的特性
//这个脚本挂载在所有需要存储的代码上
public class DataGUID : MonoBehaviour
{
    public string guid;//标识储存数据
    private void Awake()
    {
        if (guid == string.Empty) 
        {
            guid=System.Guid.NewGuid().ToString();//生成GUID
        }
    }
}
