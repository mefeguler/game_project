using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Camera playerCamera; // YENİ: Kameramızı buraya bağlayacağız

    // Hız ve İvme Ayarları
    public float baseSpeed = 10f;
    private float currentSpeed;

    public float gravity = -40f;
    public float jumpHeight = 1.2f;

    // Ayak Sensörü
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    // Eğilme (Crouch) Ayarları
    public float normalHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchSpeed = 4f;

    // Bunny Hop Ayarları
    public float maxBunnySpeed = 18f;
    public float speedBoostPerJump = 1.5f;

    // --- YENİ: KAMERA EFEKTLERİ (FOV ve Head Bob) ---
    public float normalFOV = 60f;
    public float maxFOV = 85f; // Hızlandıkça görüş açısı genişleyecek
    public float fovSpeed = 5f;

    public float bobFrequency = 1.5f; // Adım atma hızı
    public float bobAmplitude = 0.15f; // Kafanın ne kadar sallanacağı
    private float bobTimer;

    // Kamera yüksekliği (Eğilme hissiyatı için)
    private float defaultCameraY = 0.6f;
    private float crouchCameraY = 0f;

    void Start()
    {
        currentSpeed = baseSpeed;
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = normalFOV;
        }
    }

    void Update()
    {
        // Yerde miyiz kontrolü
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;

            if (currentSpeed > baseSpeed && !Input.GetButton("Jump"))
            {
                currentSpeed -= Time.deltaTime * 15f;
            }
        }

        // --- EĞİLME (CROUCH) KISMI ---
        float targetCameraY = defaultCameraY; // Hedeflenen kamera yüksekliği

        if (Input.GetKey(KeyCode.LeftControl))
        {
            controller.height = crouchHeight;
            currentSpeed = crouchSpeed;
            targetCameraY = crouchCameraY; // Eğilince kamera aşağı insin
        }
        else
        {
            controller.height = normalHeight;
            if (currentSpeed < baseSpeed) currentSpeed = baseSpeed;
        }

        // --- YÜRÜME KISMI ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // --- ZIPLAMA VE BUNNY HOP KISMI ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            if (currentSpeed < maxBunnySpeed)
            {
                currentSpeed += speedBoostPerJump;
            }
        }

        // --- YERÇEKİMİ ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // ==========================================
        // --- YENİ: KAMERA EFEKTLERİ KISMI ---
        // ==========================================
        if (playerCamera != null)
        {
            // 1. FOV (Görüş Açısı) Değişimi: Hızlandıkça rüzgarı hisset!
            float speedRatio = Mathf.Clamp01((currentSpeed - baseSpeed) / (maxBunnySpeed - baseSpeed));
            float targetFOV = Mathf.Lerp(normalFOV, maxFOV, speedRatio);
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);

            // 2. Head Bobbing (Kafa Sallanması)
            bool isMoving = (x != 0 || z != 0); // Karakter yürüyor mu?

            // Yerdeyken, yürürken ve eğilmiyorken kafa sallasın
            if (isGrounded && isMoving && !Input.GetKey(KeyCode.LeftControl))
            {
                // Yürüdükçe zamanı hızımızla çarparak ritim oluşturuyoruz (Sinüs dalgası)
                bobTimer += Time.deltaTime * currentSpeed * bobFrequency;

                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    targetCameraY + Mathf.Sin(bobTimer) * bobAmplitude,
                    playerCamera.transform.localPosition.z
                );
            }
            else
            {
                // Durduğumuzda veya havadayken kafa sallanması dursun, kamera yumuşakça sabitlensin
                bobTimer = 0f;
                playerCamera.transform.localPosition = new Vector3(
                    playerCamera.transform.localPosition.x,
                    Mathf.Lerp(playerCamera.transform.localPosition.y, targetCameraY, Time.deltaTime * 10f),
                    playerCamera.transform.localPosition.z
                );
            }
        }
    }
}