using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public TimeManager timeManager;

    public float moveSpeed, gravityModifier, jumpPower, runSpeed = 12.0f;
    public CharacterController charCon;

    private Vector3 moveInput;

    public Transform camTransform;
    public float mouseSensitivity;
    public bool invertX, invertY;

    private bool canJump, canDoubleJump;
    public Transform groundCheckPoint;
    public LayerMask whatIsGround;

    public Animator animator;

    public Transform firePoint;

    public Gun activeGun;
    public List<Gun> allGuns = new List<Gun>();
    public int currentGun;
    public List<Gun> unlockableGuns = new List<Gun>();

    public Transform aimPoint, gunHolder;
    public Vector3 gunStartPost;
    public float aimSpeed = 2f;

    public GameObject muzzleFlash;

    public AudioSource footstepFast, footstepSlow;

    private float bounceAmount;
    private bool bounce;

    public float maxViewAngle =60f;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        currentGun--;
        SwitchGun();
        gunStartPost = gunHolder.localPosition;
    }

    void Update()
    {
        if (!UIController.instance.pauseScreen.activeInHierarchy && !GameManager.instance.levelEnding)
        {
            // moveInput.x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; 
            // moveInput.z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            // store y velocity
            float yStore = moveInput.y;

            Vector3 vertMove = transform.forward * Input.GetAxis("Vertical");
            Vector3 horiMove = transform.right * Input.GetAxis("Horizontal");

            moveInput = horiMove + vertMove;
            moveInput.Normalize();
            
            if (Input.GetKey(KeyCode.Backspace))
                timeManager.DoSlowmotion();

            if (Input.GetKey(KeyCode.LeftShift) && !canDoubleJump && canJump)
                moveInput = moveInput * runSpeed;
            else
                moveInput = moveInput * moveSpeed;

            moveInput.y = yStore;

            moveInput.y += Physics.gravity.y * gravityModifier * Time.deltaTime;

            canJump = false;

            if (charCon.isGrounded)
            {
                moveInput.y = Physics.gravity.y * gravityModifier * Time.deltaTime;
                canJump = true;
            }
            //????
            //canJump = Physics.OverlapSphere(groundCheckPoint.position, 0.10f, whatIsGround).Length > 0;

            if (canJump)
            {
                canDoubleJump = false;
                
            }
            // Handle Jumping

            // Double Jump not working
            if (Input.GetKeyDown(KeyCode.Space) && canJump)
            {
                moveInput.y = jumpPower;
                canDoubleJump = true;
                AudioManager.instance.PlaySFX(8);
            }
            else if (canDoubleJump && Input.GetKeyDown(KeyCode.Space))
            {
                moveInput.y = jumpPower;
                canDoubleJump = false;
                AudioManager.instance.PlaySFX(8);
            }

            if (bounce)
            {
                bounce = false;
                moveInput.y = bounceAmount;

                canDoubleJump = true;
            }

            charCon.Move(moveInput * Time.deltaTime);

            // control camera rotation
            Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y") * mouseSensitivity);

            if (invertX)
                mouseInput.x = -mouseInput.x;

            if (invertY)
                mouseInput.y = -mouseInput.y;

            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);
            camTransform.rotation = Quaternion.Euler(camTransform.rotation.eulerAngles + new Vector3(-mouseInput.y, 0f, 0f));
            
            if(camTransform.rotation.eulerAngles.x > maxViewAngle && camTransform.rotation.eulerAngles.x<180)
            {
                camTransform.rotation = Quaternion.Euler(maxViewAngle, camTransform.rotation.eulerAngles.y, camTransform.rotation.eulerAngles.z);
            }
            else if(camTransform.rotation.eulerAngles.x > 180f && camTransform.rotation.eulerAngles.x<360f - maxViewAngle)
            {
                camTransform.rotation = Quaternion.Euler(-maxViewAngle, camTransform.rotation.eulerAngles.y, camTransform.rotation.eulerAngles.z);
            }

            muzzleFlash.SetActive(false);

            //handle shooting
            //one shot/ first shot
            if (Input.GetMouseButtonDown(0) && activeGun.fireCounter <= 0)
            {
                RaycastHit hit;
                if (Physics.Raycast(camTransform.position, camTransform.forward, out hit, 5000f)) 
                {
                    firePoint.LookAt(hit.point);
                }
                else
                {
                    firePoint.LookAt(camTransform.position + (camTransform.forward * 30f));
                }


                FireShot();
            }


            //autofire
            if (Input.GetMouseButton(0) && activeGun.canAutoFire)
            {
                if (activeGun.fireCounter <= 0)
                {
                    FireShot();
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SwitchGun();
                Debug.Log("switched");
            }


            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("switched");
                CameraController.instace.ZoomIn(activeGun.zoomAmount);
            }

            if (Input.GetMouseButton(1))
            {
                gunHolder.position = Vector3.MoveTowards(gunHolder.position, aimPoint.position, aimSpeed * Time.deltaTime);
            }
            else
            {
                gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, gunStartPost, aimSpeed * Time.deltaTime);
            }

            if (Input.GetMouseButtonUp(1))
            {
                CameraController.instace.ZoomOut();
            }

            animator.SetFloat("moveSpeed", moveInput.magnitude);
            animator.SetBool("onGround", canJump);

        }
    }

    public void FireShot()
    {
        if (activeGun.currentAmmo > 0)
        {
            activeGun.currentAmmo--;
            Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);
            activeGun.fireCounter = activeGun.fireRate;
            UIController.instance.ammoText.text = "AMMMO: " + activeGun.currentAmmo;
            muzzleFlash.SetActive(true);
        }
    }

    public void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);
        currentGun++;

        if (currentGun >= allGuns.Count)
            currentGun = 0;
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);
        UIController.instance.ammoText.text = "AMMMO: " + activeGun.currentAmmo;

        firePoint.position = activeGun.firepoint.position;
    }

    public void AddGun(string gunToAdd)
    {
        bool gunUnlocked = false;

        for (int i = 0; i < unlockableGuns.Count; i++)
        {
            if(unlockableGuns[i].gunName == gunToAdd)
            {
                gunUnlocked = true;
                allGuns.Add(unlockableGuns[i]);
                unlockableGuns.RemoveAt(i);
                break;
            }
        }

        if (gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            SwitchGun();
        }
    }

    public void Bounce(float bounceForce)
    {
        bounceAmount = bounceForce;
        bounce = true;
    }

}
