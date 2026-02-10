using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EmblemUI : MonoBehaviour
{
    public TMP_Text NameText;
    public TMP_Text EmblemText;
    public Image EmblemImg;
    public Sprite NarrationImage;
    public List<EmblemData> textData;
    [System.Serializable]
    public class EmblemData
    {
        [TextArea(1, 3)]
        public string Name;

        [TextArea(2, 5)]
        public string Emblem;

        public Sprite EmblemSprite;
    }

    public void Update()
    {
        if(NameText == null || EmblemText == null || EmblemImg == null)
        {
            return;
        }

        string currentName = NameText.text.Trim();

        if (string.IsNullOrEmpty(currentName))
        {
            EmblemText.text = "";
            EmblemImg.sprite = NarrationImage;
            return;
        }

        foreach (var data in textData)
        {
            if(data.Name == currentName)
            {
                EmblemText.text = data.Emblem;
                EmblemImg.sprite = data.EmblemSprite;
                return;
            }
        }

        EmblemText.text = "";
        EmblemImg.sprite = null;
    }
}
