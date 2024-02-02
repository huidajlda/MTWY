using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneNameAttribute))]//Ϊ�ĸ����Խ��л���
public class SceneNameDrawer : PropertyDrawer//�����ڼ�����е�����
{
    int sceneIndex = -1;//�������
    GUIContent[] sceneNames;
    readonly string[] scenePathSplit = { "/", ".uinty" };
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (EditorBuildSettings.scenes.Length == 0) return;
        if (sceneIndex == -1)
            GetSceneNameArray(property);
        int oldIndex = sceneIndex;
        sceneIndex=EditorGUI.Popup(position, label, sceneIndex, sceneNames);
        if(oldIndex!=sceneIndex)
            property.stringValue = sceneNames[sceneIndex].text;
    }
    //��ó���������(����ǵ�����)
    private void GetSceneNameArray(SerializedProperty property) 
    {
        var scenes=EditorBuildSettings.scenes;
        //��ʼ��
        sceneNames=new GUIContent[scenes.Length];
        for (int i = 0; i < sceneNames.Length; i++) 
        {
            string path = scenes[i].path;//����·��
            //Debug.Log(path);
            string[] splitPath=path.Split(scenePathSplit,System.StringSplitOptions.RemoveEmptyEntries);
            //Debug.Log(splitPath[2]);
            string[] name = splitPath[2].Split(".unity", System.StringSplitOptions.RemoveEmptyEntries);
            //Debug.Log(name[0]);
            string sceneName = "";
            if (name.Length > 0)
            {
                sceneName = name[name.Length-1];
            }
            else 
            {
                sceneName = "(Deleted Scene)";
            }
            sceneNames[i]=new GUIContent(sceneName);
        }
        if (sceneNames.Length == 0) 
        {
            sceneNames = new[] { new GUIContent("Check Your Build Settings") };
        }
        if (!string.IsNullOrEmpty(property.stringValue))
        {
            bool nameFound = false;
            for (int i = 0; i < sceneNames.Length; i++) 
            {
                if (sceneNames[i].text == property.stringValue) 
                {
                    sceneIndex = i;
                    nameFound = true;
                    break;
                }
            }
            if (nameFound == false)
                sceneIndex = 0;
        }
        else 
        {
            sceneIndex = 0;
        }
        property.stringValue = sceneNames[sceneIndex].text;
    }
}
#endif
