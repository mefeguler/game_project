using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // Farenin hassasiyeti
    public Transform playerBody; // Karakterin bedeni
    float xRotation = 0f;

    void Start()
    {
        // Oyuna başlayınca fare imlecini ekranın ortasına kilitle ve gizle
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // Farenin X (sağ-sol) ve Y (yukarı-aşağı) hareketlerini al
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Yukarı/aşağı bakma açısını hesapla ve sınırla (boynu kırılmasın diye)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Kamerayı yukarı/aşağı döndür
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Karakterin tüm bedenini sağa/sola döndür
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
