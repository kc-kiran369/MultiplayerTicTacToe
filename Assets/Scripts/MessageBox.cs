using TMPro;
using UnityEngine;

public class MessageBox : MonoBehaviour
{
    public static MessageBox Instance { get; private set; }
    [SerializeField] GameObject MessageBoxGB;
    [SerializeField] TMP_Text DescriptionText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        MessageBoxGB.SetActive(false);
    }

    public void Show(string text, float time)
    {
        DescriptionText.text = text;
        MessageBoxGB.SetActive(true);

        if (time > 0)
            Invoke(nameof(Hide), time);
    }

    public void Hide()
    {
        MessageBoxGB.SetActive(false);
    }
}