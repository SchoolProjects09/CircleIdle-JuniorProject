using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//https://www.youtube.com/watch?v=HXFoUGw7eKk

[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;
    public LayoutElement layoutElement;

    public void SetText(string content, string header = "")
    {
        if (string.IsNullOrEmpty(header))
            headerField.gameObject.SetActive(false);
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }
        contentField.text = content;

        int headederLenght = headerField.text.Length;
        int contentLenght = contentField.text.Length;
        Debug.Log(System.Math.Max(headerField.preferredWidth, contentField.preferredWidth));
        //layoutElement.enabled = System.Math.Max(headerField.preferredWidth, contentField.preferredWidth) >= layoutElement.preferredWidth;
    }
    private void Update()
    {
        Vector2 position = Input.mousePosition;
        transform.position = position;

    }
}
 