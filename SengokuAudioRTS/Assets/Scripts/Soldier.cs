using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SARTS
{
    [ExecuteInEditMode]
    public class Soldier : MonoBehaviour
    {
        public enum PieceType
        {
            Pawn=0,
            Knight,
            Tank,
        }
        public enum MoveState
        {
            Wait=0,
            Move,
            Finish
        }
        public enum PlSide
        {
            Pl1 = 0,
            Pl2,
        }
        readonly float HEIGHT_GAIN = 0.001f;
        public PieceType pieceType;
        public MoveState moveState;
        public PlSide plSide;
        [SerializeField] GameObject m_messangerPrefab = null;
        [SerializeField, Range(0f, 100000f)] float m_power=1000f;
        [SerializeField] Transform m_meshTr = null;
        [SerializeField] TMPro.TMP_Text m_text = null;
        [SerializeField, Range(0.01f,1f)] float m_moveSpdGain = 0.1f;
        float m_moveSpd;


        private void Awake()
        {
            m_power = 0f;
        }

        // Start is called before the first frame update
        void Start()
        {
            Color col;
            switch (pieceType)
            {
                case PieceType.Knight: col = Color.yellow; break;
                case PieceType.Tank: col = Color.green; break;
                default: col = Color.white; break;
            }
            switch (pieceType)
            {
                case PieceType.Knight: m_moveSpd = 3f; break;
                case PieceType.Tank: m_moveSpd = 1f; break;
                default: m_moveSpd = 2f; break;
            }
            if(plSide== PlSide.Pl2)
            {
                m_moveSpd *= -1f;
            }

#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                m_meshTr.GetComponent<MeshRenderer>().sharedMaterial.color = col;
            }
            else
            {
                m_meshTr.GetComponent<MeshRenderer>().material.color = col;
            }
#else
            m_meshTr.GetComponent<MeshRenderer>().material.color = col;
#endif
        }

        // Update is called once per frame
        void Update()
        {
            if (m_meshTr != null)
            {
                m_meshTr.transform.localScale = new Vector3(1f, m_power * HEIGHT_GAIN, 1f);
                m_meshTr.transform.localPosition = new Vector3(0f, m_power * HEIGHT_GAIN * 0.5f, 0f);
                m_text.text = Mathf.CeilToInt(m_power).ToString();
            }
            if(moveState== MoveState.Move)
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.MovePosition(transform.position + Time.deltaTime * m_moveSpd * m_moveSpdGain * Vector3.right);
                //transform.position += Time.deltaTime * m_moveSpd * m_moveSpdGain * Vector3.right;

            }
#if UNITY_EDITOR
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Color col;
                switch (pieceType)
                {
                    case PieceType.Knight: col = Color.yellow; break;
                    case PieceType.Tank: col = Color.green; break;
                    default: col = Color.white; break;
                }
                m_meshTr.GetComponent<MeshRenderer>().sharedMaterial.color = col;
            }
#endif
        }

        private void OnCollisionStay(Collision collision)
        {
            Soldier enemyScr = collision.gameObject.GetComponent<Soldier>();
            if (enemyScr != null)
            {
                float myRate = 0.5f;
                if((pieceType == PieceType.Pawn) && (enemyScr.pieceType == PieceType.Knight)){
                    myRate = 0.35f;
                }
                if ((pieceType == PieceType.Pawn) && (enemyScr.pieceType == PieceType.Tank))
                {
                    myRate = 0.2f;
                }
                if ((pieceType == PieceType.Knight) && (enemyScr.pieceType == PieceType.Pawn))
                {
                    myRate = 0.65f;
                }
                if ((pieceType == PieceType.Knight) && (enemyScr.pieceType == PieceType.Tank))
                {
                    myRate = 0.4f;
                }
                if ((pieceType == PieceType.Tank) && (enemyScr.pieceType == PieceType.Pawn))
                {
                    myRate = 0.8f;
                }
                if ((pieceType == PieceType.Tank) && (enemyScr.pieceType == PieceType.Knight))
                {
                    myRate = 0.6f;
                }

                float ret;
                ret = AddPower(-10f* (1f - myRate));
                int result = 0;
                if (ret <= 0f)
                {
                    result |= 1; // pl全滅
                    Destroy(gameObject);
                }
                ret = enemyScr.AddPower(-10f*myRate);
                if (ret <= 0f)
                {
                    result |= 2; // em全滅
                }

                if (result != 0)
                {
                    GameObject emGo = collision.gameObject;
                    Soldier emSoldier = emGo.GetComponent<Soldier>();

                    GameObject plMesGo = Instantiate(m_messangerPrefab, transform.position, transform.rotation);
                    Messenger plMessenger = plMesGo.GetComponent<Messenger>();
                    GameObject emMesGo = Instantiate(m_messangerPrefab, emGo.transform.position, emGo.transform.rotation);
                    Messenger emMessenger = emMesGo.GetComponent<Messenger>();

                    if (result == 1)
                    {
                        plMessenger.SetResultMessage(this, false);
                        emMessenger.SetResultMessage(emSoldier, true);
                    }
                    if (result == 2)
                    {
                        plMessenger.SetResultMessage(this, true);
                        emMessenger.SetResultMessage(emSoldier, false);
                    }
                    if (result == 3)
                    {
                        plMessenger.SetResultMessage(this, false);
                        emMessenger.SetResultMessage(emSoldier, false);
                    }
                }

                if ((result & 1) !=0)
                {
                    Destroy(gameObject);
                }
                if ((result & 2) != 0)
                {
                    Destroy(collision.gameObject);
                }

            }
        }

        public void SetPower(float _power)
        {
            m_power = _power;
        }
        public float AddPower(float _power)
        {
            m_power = Mathf.Max(m_power + _power,0f);
            return m_power;
        }

    }
}
