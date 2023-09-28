using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
[ ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgessBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI_prefabs/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif
    public float minimum;
    public float maximum;
    public float current;
    public Image bar;
    public Color color;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }
    public void GetCurrentFill()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;

        float fillAmount = currentOffset / maximumOffset;
        bar.fillAmount = fillAmount;
        bar.color = color;
    }
}
