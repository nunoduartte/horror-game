using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    private Button quitButton;
    // Start is called before the first frame update
    void Start()
    {
        this.quitButton = this.GetComponent<Button>();
    }

    // Update is called once per frame
    public void OnClick()
    {
        this.quitButton.interactable = false;
        Application.Quit();
    }
}
