using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private Transform crosshairTransform;

    private void Start()
    {
        crosshairTransform = transform.Find("Crosshair");
    }

    void Update()
    {
        crosshairTransform.position = Input.mousePosition;
    }
}
