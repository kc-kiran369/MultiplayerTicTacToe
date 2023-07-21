using UnityEngine;
using UnityEngine.UI;

public class OffsetAnimator : MonoBehaviour
{
    public Vector2 Axis;
    public float Speed = 10.0f;
    private Image _image;

    private void Start()
    {
        _image = GetComponent<Image>();
        Axis = new(1, 0);
    }
    private void Update()
    {
        _image.material.mainTextureOffset += Time.deltaTime * Speed * Axis;
    }
}
