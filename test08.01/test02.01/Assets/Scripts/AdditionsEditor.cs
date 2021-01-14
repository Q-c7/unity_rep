using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdditionsEditor : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject additionsEditor;

    public InputField c320x;
    public InputField c320y;
    public InputField c320z;

    public InputField c120x;
    public InputField c120y;
    public InputField c120z;

    public InputField c25x;
    public InputField c25y;
    public InputField c25z;

    private MainSettings settings;
    // Start is called before the first frame update
    void Start()
    {
        settings = MainMenu.LoadSettings();
        MainMenu.PropagateGlobals(settings);
        UpdateFields();

        c320x?.onValueChanged.AddListener(delegate { OnC320XChange(c320x.text); });
        c320y?.onValueChanged.AddListener(delegate { OnC320YChange(c320y.text); });
        c320z?.onValueChanged.AddListener(delegate { OnC320ZChange(c320z.text); });

        c120x?.onValueChanged.AddListener(delegate { OnC120XChange(c120x.text); });
        c120y?.onValueChanged.AddListener(delegate { OnC120YChange(c120y.text); });
        c120z?.onValueChanged.AddListener(delegate { OnC120ZChange(c120z.text); });

        c25x?.onValueChanged.AddListener(delegate { OnC25XChange(c25x.text); });
        c25y?.onValueChanged.AddListener(delegate { OnC25YChange(c25y.text); });
        c25z?.onValueChanged.AddListener(delegate { OnC25ZChange(c25z.text); });
    }

    private void UpdateFields()
    {
        if (c320x != null)
            c320x.text = Globals.crane320_x.ToString();
        if (c320y != null)
            c320y.text = Globals.crane320_y.ToString();
        if (c320z != null)
            c320z.text = Globals.crane320_z.ToString();

        if (c120x != null)
            c120x.text = Globals.crane120_x.ToString();
        if (c120y != null)
            c120y.text = Globals.crane120_y.ToString();
        if (c120z != null)
            c120z.text = Globals.crane120_z.ToString();

        if (c25x != null)
            c25x.text = Globals.crane25_x.ToString();
        if (c25y != null)
            c25y.text = Globals.crane25_y.ToString();
        if (c25z != null)
            c25z.text = Globals.crane25_z.ToString();
    }

    private void OnC25ZChange(string text)
    {
        settings.crane25_z = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC25YChange(string text)
    {
        settings.crane25_y = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC25XChange(string text)
    {
        settings.crane25_x = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC120ZChange(string text)
    {
        settings.crane120_z = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC120YChange(string text)
    {
        settings.crane120_y = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC120XChange(string text)
    {
        settings.crane120_x = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC320ZChange(string text)
    {
        settings.crane320_z = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC320YChange(string text)
    {
        settings.crane320_y = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    private void OnC320XChange(string text)
    {
        settings.crane320_x = float.Parse(MainMenu.EmptyToZero(text));
        MainMenu.SaveSettings(settings);
        MainMenu.PropagateGlobals(settings);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            additionsEditor?.SetActive(false);
        }
    }

    public void OnShowAdditionsEditor()
    {
        additionsEditor?.SetActive(true);
        mainMenu?.SetActive(false);
    }

    public void OnHideAdditionsEditor()
    {
        additionsEditor?.SetActive(false);
        mainMenu?.SetActive(true);
    }
}
