using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
//[RequireComponent(typeof(Animator))]
public class SpartanControl : NetworkBehaviour {

    #region SerializeField Area

    [SerializeField] Text lifeLabel;
    [SerializeField] Text lifePrefabs;
    [SerializeField] RectTransform lifePos;

    [SerializeField] float m_GroundCheckDistance = 0.1f;
    [SerializeField] float rotationSpeed = 100.0F;
    [SerializeField] GameObject weaponIndex0;
    [SerializeField] GameObject weaponIndex1;
    [SerializeField] GameObject weaponIndex2;
    [SerializeField] GameObject weaponIndex3;
    [SerializeField] GameObject weaponIndex4;
    #endregion

    #region Variable
    float m_OrigGroundCheckDistance;
        Rigidbody rb;
        //[SerializeField] Animator m_Animator;
        private Animation anim;
        CapsuleCollider m_Capsule;
    bool holdWeapon1 = false;
    bool holdWeapon2 = false;
    int holdIndex1 = 0;
    int holdIndex2 = 0;
    #endregion

    #region SyncVar
    int countGuard = 3; 
    int countBadge = 0;
    [SyncVar(hook = "OnIsResist")] public bool isResist = false;
    [SyncVar(hook = "OnChangeLife")] public float hitpoint = 100.0f;
    [SyncVar(hook = "OnChageWeapon")] int holdingIndex = 0;
    [SyncVar(hook = "OnChangeSpeed")] float speed = 16;

    #endregion

    #region SyncVarAnimation
    [SyncVar(hook = "OnAttck")] bool stateAttack = false;
    [SyncVar(hook = "OnIdle")] bool stateIdle = true;
    [SyncVar(hook = "OnRun")] bool stateRun = false;
    [SyncVar(hook = "OnResist")] bool stateResist = false;
    [SyncVar(hook = "OnDie")] bool stateDie = false;

    #endregion

    // Use this for initialization
    void Start () {
        if (isLocalPlayer)
        {
            this.transform.GetChild(0).GetComponent<Camera>().enabled = true;
        }
        else
        {
            this.transform.GetChild(0).GetComponent<Camera>().enabled = false;
        }

        anim = gameObject.GetComponent<Animation>();
        rb = transform.GetComponent<Rigidbody>();
       // m_Animator = GetComponent<Animator>();
        m_Capsule = GetComponent<CapsuleCollider>();

        m_OrigGroundCheckDistance = m_GroundCheckDistance;

        GameObject panel = GameObject.FindGameObjectWithTag("HitPointPanel");

        if (isLocalPlayer)
        {
            lifeLabel = Instantiate(lifePrefabs, Vector3.zero, Quaternion.identity) as Text;
            lifeLabel.transform.SetParent(panel.transform);

            Vector3 lifeLabelPos = lifePos.position;
            lifeLabel.transform.position = lifeLabelPos;

            lifeLabel.text = hitpoint.ToString();
        }
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if (hitpoint <= 0)
            {
                stateDie = true;
                stateIdle = false;
                CmdDie(stateDie);
                CmdIdle(stateIdle);
            }
            else if (Input.GetMouseButton(0) && !Input.GetKey(KeyCode.W))
            {
                stateAttack = true;
                stateIdle = false;
                CmdAttack(stateAttack);
                CmdIdle(stateIdle);

            }
            else if (Input.GetMouseButton(1) && countGuard > 0 && !Input.GetKey(KeyCode.W))
            {
                Debug.Log(countGuard);
                //countGuard--;
                isResist = true;
                stateResist = true;
                stateIdle = false;
                CmdResist(stateResist);
                CmdIdle(stateIdle);
                CmdIsResist(isResist);

            }
            else if (Input.GetKey(KeyCode.W))
            {
                stateRun = true;
                stateIdle = false;
                CmdRun(stateRun);
                CmdIdle(stateIdle);
            }
            else
            {
                stateAttack = false;
                stateRun = false;
                stateResist = false;
                stateDie = false;
                stateIdle = true;
                isResist = false;

                CmdAttack(stateAttack);
                CmdRun(stateRun);
                CmdResist(stateResist);
                CmdDie(stateDie);
                CmdIsResist(isResist);
                CmdIdle(stateIdle);

            }

        }
        else
        {
            return;
        }
       // m_Animator.ResetTrigger("Attack");
        
       
    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            #region movement
            if (hitpoint >= 0)
            {
                float translation = Input.GetAxis("Vertical") * speed;
                
                translation *= Time.deltaTime;
                

                // Rigidbody Forward
                if (translation >= 0)
                {
                    rb.MovePosition(rb.position + this.transform.forward * translation);
                }
                else
                {
                    rb.MovePosition(rb.position + this.transform.forward * 0);
                }



                //Rigidbody Turn 
                if (Input.GetAxis("Vertical") > 0.4f || Input.GetAxis("Vertical") < -0.4f)
                {
                    float rotation = Input.GetAxis("Horizontal") * rotationSpeed;
                    rotation *= Time.deltaTime;
                    Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
                    rb.MoveRotation(rb.rotation * turn);
                }
                else
                {
                    float rotation = Input.GetAxis("Horizontal") * rotationSpeed*0;
                    rotation *= Time.deltaTime;
                    Quaternion turn = Quaternion.Euler(0f, rotation, 0f);
                    rb.MoveRotation(rb.rotation * turn);
                }
            }

            #endregion

            if (Input.GetKeyDown(KeyCode.Alpha1) && holdWeapon1)
            {
                // WeaponIndex : 1 = Axe , 2 = Katana , 3 = LongSword , 4 = Claymore
                if (holdIndex1 == 1)
                {
                    holdingIndex = holdIndex1;
                    speed = 8;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                }
                else if (holdIndex1 == 2)
                {
                    holdingIndex = holdIndex1;
                    speed = 15;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                }
                else if (holdIndex1 == 3)
                {
                    holdingIndex = holdIndex1;
                    speed = 7;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                    //weaponIndex3.SetActive(true);
                }
                else if (holdIndex1 == 4)
                {
                    holdingIndex = holdIndex1;
                    speed = 12;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                    //weaponIndex4.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && holdWeapon2)
            {
                // WeaponIndex : 1 = Axe , 2 = Katana , 3 = LongSword , 4 = Claymore
                if (holdIndex2 == 1)
                {
                    holdingIndex = holdIndex2;
                    speed = 8;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                    //weaponIndex1.SetActive(true);
                }
                else if (holdIndex2 == 2)
                {
                    holdingIndex = holdIndex2;
                    speed = 15;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                    //weaponIndex2.SetActive(true);
                }
                else if (holdIndex2 == 3)
                {
                    holdingIndex = holdIndex2;
                    speed = 7;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                    //weaponIndex3.SetActive(true);
                }
                else if (holdIndex2 == 4)
                {
                    holdingIndex = holdIndex2;
                    speed = 12;
                    CmdChageWeapon(holdingIndex);
                    CmdChangeSpeed(speed);
                    //weaponIndex4.SetActive(true);
                }
            }
        }
        /*float Horizontal = Input.GetAxis("Horizontal");
        float Vertical = Input.GetAxis("Vertical");

        //Check should cause sliding.
        if (Horizontal != 0 || Vertical != 0)
        {
            Vector3 movement = new Vector3(Horizontal, 0, Vertical) * speed;
            rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
        }
                 */

    }

    private void OnTriggerStay(Collider other)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Enter");
            if (other.gameObject.CompareTag("pickAxe") && Input.GetKey(KeyCode.F))
            {
                Debug.Log("pick up axe");
                if (!holdWeapon1)
                {
                    Debug.Log("Axe to slot1");
                    holdWeapon1 = true;
                    holdIndex1 = 1;
                }
                else if (!holdWeapon2 && holdIndex1 != 1)
                {
                    Debug.Log("Axe to slot2");
                    holdWeapon2 = true;
                    holdIndex2 = 1;
                }
            }
            else if (other.gameObject.CompareTag("pickKatana") && Input.GetKey(KeyCode.F))
            {
                Debug.Log("pick up katana");
                if (!holdWeapon1)
                {
                    Debug.Log("katana to slot1");
                    holdWeapon1 = true;
                    holdIndex1 = 2;
                }
                else if (!holdWeapon2 && holdIndex1 != 2)
                {
                    Debug.Log("katana to slot2");
                    holdWeapon2 = true;
                    holdIndex2 = 2;
                }
            }
            else if (other.gameObject.CompareTag("pickLongSword") && Input.GetKey(KeyCode.F))
            {
                Debug.Log("pick up LongSword");
                if (!holdWeapon1)
                {
                    Debug.Log("LongSword to slot1");
                    holdWeapon1 = true;
                    holdIndex1 = 3;
                }
                else if (!holdWeapon2 && holdIndex1 != 3)
                {
                    Debug.Log("LongSword to slot2");
                    holdWeapon2 = true;
                    holdIndex2 = 3;
                }
            }
            else if (other.gameObject.CompareTag("pickClaymore") && Input.GetKey(KeyCode.F))
            {
                Debug.Log("pick up Claymore");
                if (!holdWeapon1)
                {
                    Debug.Log("Claymore to slot1");
                    holdWeapon1 = true;
                    holdIndex1 = 4;
                }
                else if (!holdWeapon2 && holdIndex1 != 4)
                {
                    Debug.Log("Claymore to slot2");
                    holdWeapon2 = true;
                    holdIndex2 = 4;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.gameObject.CompareTag("knife") && !isResist)
            {
                Debug.Log("knife");
                float damge = 15.0f;
                hitpoint -= damge;
                Debug.Log(hitpoint);
                CmdChangeLife(hitpoint);
            }
            else if (other.gameObject.CompareTag("doubleAxe") && !isResist)
            {
                Debug.Log("doubleAxe");
                float damge = 40.0f;
                hitpoint -= damge;
                Debug.Log(hitpoint);
                CmdChangeLife(hitpoint);
            }
            else if (other.gameObject.CompareTag("katana") && !isResist)
            {
                Debug.Log("Katana Hit");
                float damge = 20.0f;
                hitpoint -= damge;
                Debug.Log(hitpoint);
                CmdChangeLife(hitpoint);
            }
            else if (other.gameObject.CompareTag("longSword") && !isResist)
            {
                Debug.Log("longSword");
                float damge = 50.0f;
                hitpoint -= damge;
                Debug.Log(hitpoint);
                CmdChangeLife(hitpoint);
            }
            else if (other.gameObject.CompareTag("claymore") && !isResist)
            {
                Debug.Log("Claymore Hit");
                float damge = 25.0f;
                hitpoint -= damge;
                Debug.Log(hitpoint);
                CmdChangeLife(hitpoint);
            }
        }
    }

    #region Cmd
    [Command]
    public void CmdChangeLife(float newHitPoint)
    {
        hitpoint = newHitPoint;
        lifeLabel.text = hitpoint.ToString();
    }

    [Command]
    public void CmdIsResist(bool newValue)
    {
        isResist = newValue;
    }

    [Command]
    public void CmdChageWeapon(int indexWeapon)
    {
        holdingIndex = indexWeapon;
        if (holdingIndex==1)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(true);
            weaponIndex2.SetActive(false);
            weaponIndex3.SetActive(false);
            weaponIndex4.SetActive(false);
        }
        else if (holdingIndex == 2)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(false);
            weaponIndex2.SetActive(true);
            weaponIndex3.SetActive(false);
            weaponIndex4.SetActive(false);
        }
        else if (holdingIndex == 3)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(false);
            weaponIndex2.SetActive(false);
            weaponIndex3.SetActive(true);
            weaponIndex4.SetActive(false);
        }
        else if (holdingIndex == 4)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(false);
            weaponIndex2.SetActive(false);
            weaponIndex3.SetActive(false);
            weaponIndex4.SetActive(true);
        }

    }

    [Command]
    public void CmdChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    


    void OnChangeLife(float n)
    {
        hitpoint = n;
        lifeLabel.text = hitpoint.ToString();
    }

    void OnIsResist(bool n)
    {
        isResist = n;
    }

    void OnChageWeapon(int indexWeapon)
    {
        holdingIndex = indexWeapon;
        if (holdingIndex == 1)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(true);
            weaponIndex2.SetActive(false);
            weaponIndex3.SetActive(false);
            weaponIndex4.SetActive(false);
        }
        else if (holdingIndex == 2)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(false);
            weaponIndex2.SetActive(true);
            weaponIndex3.SetActive(false);
            weaponIndex4.SetActive(false);
        }
        else if (holdingIndex == 3)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(false);
            weaponIndex2.SetActive(false);
            weaponIndex3.SetActive(true);
            weaponIndex4.SetActive(false);
        }
        else if (holdingIndex == 4)
        {
            weaponIndex0.SetActive(false);
            weaponIndex1.SetActive(false);
            weaponIndex2.SetActive(false);
            weaponIndex3.SetActive(false);
            weaponIndex4.SetActive(true);
        }

    }

    void OnChangeSpeed(float nSpeed)
    {
        speed = nSpeed;
    }

    #endregion

    #region AnimationCommand
    [Command]
    public void CmdAttack(bool newStateAttck)
    {
        stateAttack = newStateAttck;
        if (stateAttack)
        {
            anim.Play("attack");
        }
       
    }

    [Command]
    public void CmdRun(bool newStateRun)
    {
        stateRun = newStateRun;
        if (stateRun)
        {
            anim.Play("run");
        }

    }

    [Command]
    public void CmdResist(bool newStateResist)
    {
        stateResist = newStateResist;
        if (stateResist)
        {
            Debug.Log("Start");
            anim.Play("resist");
            //countGuard--;
            //StartCoroutine(playAnimResist());
            //anim.Play("resist");
        }

    }

    [Command]
    public void CmdDie(bool newStateDie)
    {
        stateDie = newStateDie;
        if (stateDie)
        {
            anim.Play("die");
        }

    }

    [Command]
    public void CmdIdle(bool newStateIdle)
    {
        stateIdle = newStateIdle;
        if (stateIdle)
        {
            anim.Play("idlebattle");
        }

    }
        
    void OnAttck(bool newStateAnimation)
    {
        stateAttack = newStateAnimation;
        if (stateAttack)
        {
            anim.Play("attack");
        }
    }

    void OnRun(bool newStateAnimation)
    {
        stateRun = newStateAnimation;
        if (stateRun)
        {
            anim.Play("run");
        }
    }

    void OnResist(bool newStateAnimation)
    {
        stateResist = newStateAnimation;
        if (stateResist)
        {
          anim.Play("resist");
        }
    }

    void OnDie(bool newStateAnimation)
    {
        stateDie = newStateAnimation;
        if (stateDie)
        {
            anim.Play("die");
        }
    }

    void OnIdle(bool newStateAnimation)
    {
        stateIdle = newStateAnimation;
        if (stateIdle)
        {
            anim.Play("idlebattle");
        }
    }

    
    #endregion
       /*
    IEnumerator playAnimResist()
    {
        
        anim.Play("resist");
        anim.isPlaying
        Debug.Log("wait");
        yield return new WaitForSeconds(1);
        Debug.Log("finish");

        
        stateResist = false;
        CmdResist(stateResist);
    }  */

}
