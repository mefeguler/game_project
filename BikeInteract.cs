using UnityEngine;

public class BikeInteract : MonoBehaviour
{
    [Header("Gerekli Bağlantılar")]
    public GameObject player;         // FPS Karakterimiz
    public GameObject bikeCamera;     // Bisikletin arkasındaki kamera
    public BikeController bikeScript; // Sürme kodumuz

    private bool isNearBike = false;  // Bisikletin yanına geldik mi?

    void Update()
    {
        // Alanın içindeysek ve klavyeden 'E' tuşuna basarsak
        if (isNearBike && Input.GetKeyDown(KeyCode.E))
        {
            if (bikeScript.isDriving == false)
            {
                GetOnBike(); // Bisiklete bin
            }
            else
            {
                GetOffBike(); // Bisikletten in
            }
        }
    }

    // Karakter Trigger (Görünmez alan) içine GİRDİĞİNDE
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player) isNearBike = true;
    }

    // Karakter Trigger dışına ÇIKTIĞINDA
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player) isNearBike = false;
    }

    void GetOnBike()
    {
        bikeScript.isDriving = true;   // Sürme kodunu aktif et
        player.SetActive(false);       // Yürüyen karakteri tamamen gizle!
        bikeCamera.SetActive(true);    // Bisiklet kamerasını aç
    }

    void GetOffBike()
    {
        bikeScript.isDriving = false;  // Sürme kodunu durdur

        // Karakteri bisikletin biraz sol tarafına ışınla (iç içe sıkışmasınlar)
        player.transform.position = transform.position + transform.right * -2f + Vector3.up;

        player.SetActive(true);        // Yürüyen karakteri geri getir!
        bikeCamera.SetActive(false);   // Bisiklet kamerasını kapat
    }
}