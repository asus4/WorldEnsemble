using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class OpenSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;

    private void OnEnable()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonTap);
    }

    private void OnDisable()
    {
        var button = GetComponent<Button>();
        button.onClick.RemoveListener(OnButtonTap);
    }

    private void OnButtonTap()
    {
        SceneManager.LoadScene(sceneName);
    }
}
