using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    /// <summary>
    /// �̵�Ÿ��
    /// </summary>
    MoveType.TYPE m_moveType = MoveType.TYPE.Idle;
    /// <summary>
    /// ���� Ÿ��
    /// </summary>
    DirType.TYPE m_dirType = DirType.TYPE.Left;
    /// <summary>
    /// �ε� �ε���
    /// </summary>
    public int m_index = 0;

    /// <summary>
    /// �̵� �ӵ�
    /// </summary>
    public float m_speed = 0.0f;

    /// <summary>
    /// �̵����� �ʴ� ����
    /// </summary>
    [Range(0.0f, 0.5f)]
    public float m_notMoveSenc = 0.0f;

    /// <summary>
    /// �⺻ ���� �̸�
    /// </summary>
    public string m_weaponName = string.Empty;

    /// <summary>
    /// �ҷ��� ������Ʈ
    /// </summary>
    GameObject m_viewObj = null;

    /// <summary>
    /// ���� ������Ʈ
    /// </summary>
    GameObject m_weaponObj = null;

    /// <summary>
    /// �Ѿ˹߻� Ʈ������
    /// </summary>
    List<Transform> m_shotBaseT = new List<Transform>();

    /// <summary>
    /// ���� ���� ��ġ Ʈ������
    /// </summary>
    Transform m_weaponBaseT = null;

    /// <summary>
    /// �ִϸ�����
    /// </summary>
    Animator m_animator = null;

    /// <summary>
    /// ���� ������
    /// </summary>
    WeaponData m_wData = null;

    bool m_atkFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        StartSetting();
    }

    /// <summary>
    /// �⺻ ����
    /// </summary>
    void StartSetting ()
    {
        m_weaponBaseT = transform.Find("WeaponBase");
        LoadCharacter(m_index);
        
        LoadWeapon(m_weaponName);
    }

    /// <summary>
    /// ĳ���� �ҷ�����
    /// </summary>
    /// <param name="argIndex">ĳ���� �ε���</param>
    void LoadCharacter (int argIndex)
    {
        if (m_viewObj != null) Destroy(m_viewObj);

        GameObject _tmpObj = GManager.Instance.LoadResources(LoadType.TYPE.Character, argIndex);
        m_viewObj = GManager.Instance.CreateObj(_tmpObj, transform);
        m_animator = m_viewObj.GetComponent<Animator>();
    }

    /// <summary>
    /// ���� �ҷ�����
    /// </summary>
    /// <param name="argName">���� �̸�</param>
    void LoadWeapon (string argName)
    {
        if (m_weaponObj != null)
        {
            m_wData = null;
            m_shotBaseT.Clear();
            Destroy(m_weaponObj);
        }

        m_wData = GManager.Instance.GetWeaponData(argName);
        if (m_wData == null) return;
        m_weaponObj = GManager.Instance.CreateObj(m_wData.m_obj, m_weaponBaseT);
        Weapon _weaponBaseSc = m_weaponObj.GetComponent<Weapon>();
        for (int i = 0; i < _weaponBaseSc.m_sBaseT.Length; i++)
        {
            m_shotBaseT.Add(_weaponBaseSc.m_sBaseT[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        MoveController();
        AttackController();
    }

    /// <summary>
    /// �̵� ��Ʈ�ѷ�
    /// </summary>
    void MoveController ()
    {
        Vector2 _input = Vector2.zero;
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(_input.x) <= m_notMoveSenc && Mathf.Abs(_input.y) <= m_notMoveSenc)
        {
            m_moveType = MoveType.TYPE.Idle;
            _input = Vector2.zero;

        }
        else
        {
            m_moveType = MoveType.TYPE.Walk;
        }

        if (Mathf.Abs(_input.x) >= m_notMoveSenc) m_dirType = _input.x < 0 ? DirType.TYPE.Left : DirType.TYPE.Right; //�ִϸ��̼����� ���� ��ȯ
        

        m_animator.SetFloat("Dir", (float)m_dirType);
        m_animator.SetFloat("Move", (float)m_moveType);

        transform.Translate(m_speed * Time.deltaTime * _input.normalized);
    }
    
    /// <summary>
    /// ������Ʈ�ѷ�
    /// </summary>
    void AttackController()
    {
        if (m_atkFlag) return;
        if(Input.GetAxis("Fire1") > 0.0f)
        {
            m_atkFlag = true;
            for (int i = 0; i < m_shotBaseT.Count; i++)
            {
                Quaternion _rot = m_shotBaseT[i].localRotation;
                m_shotBaseT[i].Rotate(0.0f, 0.0f, Random.Range(-m_wData.m_sencitivity, m_wData.m_sencitivity));
               GameObject _bulletObj= GManager.Instance.CreateObj(m_wData.m_bulletObj, m_shotBaseT[i], false);
                _bulletObj.GetComponent<bullet>().BulletTimeSetting(m_wData.m_atkLength);
                m_shotBaseT[i].localRotation = _rot;
            }
            Invoke("ClearAtk", m_wData.m_reAtkTime);
        }
    }

    /// <summary>
    /// ���� �ʱ�ȭ
    /// </summary>
    void ClearAtk()
    {
        m_atkFlag = false;
    }
}
