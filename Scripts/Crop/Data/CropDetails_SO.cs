using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="CropDataList_SO",menuName ="Crop/CropDataList")]
public class CropDetails_SO : ScriptableObject
{
    public List<CropDetails> cropDetailsList;//种子生长信息列表
}
