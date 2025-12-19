using BepInEx.Logging;
using Common.CommonUI;
using ExposureUnnoticed2.ObjectUI.SystemMenu;
using HadakaCoat.ObjectsUi.Common.Button;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Runtime;
using Il2CppSystem.Runtime.InteropServices;
using System.Linq;
using TMPro;
using UnityEngine;

namespace FlanDev.SFM.UI;

public class NativeOverwrite : MonoBehaviour
{
    public float Scale;
    public GameObject? gameGui;
    public GameObject? myButton;
    internal ManualLogSource Log = new(nameof(MinimalTest));

    public void Awake()
    {
        ClassInjector.RegisterTypeInIl2Cpp<ButtonGroupManager.IButtonView>();
    }

    public void Update()
    {
        if (SystemMenuView.Instance == null)
        {

        }
        else
        {
            myButton ??= EnsureMainButton(SystemMenuView.Instance.buttonGroupManager);


            return;

            var textMeshProUGUIs = SystemMenuView.Instance.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (var textMeshProUGUI in textMeshProUGUIs)
            {
                textMeshProUGUI.text = "textMeshProUGUIX";
            }


            var buttonView = SystemMenuView.Instance.buttonGroupManager.buttons[1].GameObject().GetComponent<ButtonView>();
            buttonView.SetText("buttonViewX");
        }

    }

    private static GameObject EnsureMainButton(ButtonGroupManager manager)
    {
        var button = Instantiate(manager.buttons[0].GameObject()).GetComponent<ButtonGroupManager.IButtonView>();
        button.GameObject().GetComponentInChildren<TextMeshProUGUI>().text = "My Button";
        manager.ReSearchButton(true, false);
    }
}
