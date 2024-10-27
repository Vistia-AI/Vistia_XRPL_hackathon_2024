using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VerifyCodeHandler : MonoBehaviour
{
    [SerializeField] private List<TMP_InputField> verifyCodeList = new List<TMP_InputField>(6);
    private int currentIndex = 0;
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            int index = i;
            verifyCodeList[i].onSelect.AddListener((input) => Select(input, index));
            verifyCodeList[i].onValueChanged.AddListener(MoveCursor);
        }
    }

    private void Select(string arg0, int index)
    {
        if (currentIndex != index)
        {
            StartCoroutine(Wait());
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        //EventSystem.current.SetSelectedGameObject(null);
        verifyCodeList[currentIndex].Select();
    }
    private void MoveCursor(string input)
    {
        if (input.Length > 0 && currentIndex < 5)
        {
            currentIndex += 1;
        }
        else if (input.Length == 0 && currentIndex > 0)
        {
            currentIndex -= 1;
        }
        verifyCodeList[currentIndex].Select();
    }
}
