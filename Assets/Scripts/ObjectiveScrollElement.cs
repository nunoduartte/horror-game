using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveScrollElement : MonoBehaviour
{
    public TMP_Text description;
    public Sprite imageCheck;
    public Sprite imageUnCheck;
    public Image currentImage;

    public void setCurrentImage(bool isCheck)
    {
        if (isCheck)
            this.currentImage.sprite = this.imageCheck;
        else
            this.currentImage.sprite = this.imageUnCheck;
    }
}
