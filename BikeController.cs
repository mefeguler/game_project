using UnityEngine;

public class BikeController : MonoBehaviour
{
    [Header("Hareket Ayarları")]
    public float speed = 15f;
    public float turnSpeed = 60f;
    public float autoForwardSpeedOnTurn = 5f; // Sadece dönerken ne kadar ileri gidecek?

    [Header("Kamera Eğilme (Lean) Ayarları")]
    public Transform bikeCamera;      // Kamerayı buraya bağlayacağız
    public float maxLeanAngle = 15f;  // Bisiklet dönerken kamera maksimum kaç derece yatsın?
    public float leanSmooth = 5f;     // Yatmanın yumuşaklık hızı

    [HideInInspector]
    public bool isDriving = false;

    private float currentLean = 0f; // Kameranın o anki yatma açısı

    void Update()
    {
        if (isDriving)
        {
            // Klavyeden W,S (Vertical) ve A,D (Horizontal) girdilerini al
            float verticalInput = Input.GetAxis("Vertical");
            float horizontalInput = Input.GetAxis("Horizontal");

            float move = verticalInput * speed * Time.deltaTime;
            float turn = horizontalInput * turnSpeed * Time.deltaTime;

            // --- YENİ 1: Sadece Dönerken İleri Gitme ---
            // Eğer İleri/Geri (W,S) basılmıyorsa AMA Sağa/Sola (A,D) basılıyorsa
            if (Mathf.Abs(verticalInput) < 0.1f && Mathf.Abs(horizontalInput) > 0.1f)
            {
                // Bisikleti yavaşça ileri doğru it
                move = autoForwardSpeedOnTurn * Time.deltaTime;
            }

            // Bisikleti hareket ettir ve döndür
            transform.Translate(0, 0, move);
            transform.Rotate(0, turn, 0);

            // --- YENİ 2: Kamera Eğilmesi (Leaning) ---
            if (bikeCamera != null)
            {
                // Sağa dönerken (D tuşu = 1) eksi yöne yatmalı, sola dönerken (A tuşu = -1) artı yöne yatmalı
                float targetLean = horizontalInput * -maxLeanAngle;

                // Anlık açıyı, hedeflenen açıya doğru yumuşakça (Lerp) kaydır
                currentLean = Mathf.Lerp(currentLean, targetLean, Time.deltaTime * leanSmooth);

                // Kameranın sadece Z eksenini (sağa/sola yatma) değiştir, X ve Y açılarına dokunma
                Vector3 camRotation = bikeCamera.localEulerAngles;
                camRotation.z = currentLean;
                bikeCamera.localEulerAngles = camRotation;
            }
        }
        else
        {
            // --- BİSİKLETTEN İNİNCE ---
            // Biri bisikletten inmişse kamerayı yavaşça tekrar düz (0 derece) konuma getir
            if (bikeCamera != null && Mathf.Abs(currentLean) > 0.1f)
            {
                currentLean = Mathf.Lerp(currentLean, 0f, Time.deltaTime * leanSmooth);
                Vector3 camRotation = bikeCamera.localEulerAngles;
                camRotation.z = currentLean;
                bikeCamera.localEulerAngles = camRotation;
            }
        }
    }
}