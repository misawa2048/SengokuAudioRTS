using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SARTS
{
    public class SoldierButton : MonoBehaviour
    {
        readonly int SET_SPD = 10;
        [SerializeField] GameObject m_soldierPrefab = null;
        [SerializeField] Soldier.PieceType m_pieceType = Soldier.PieceType.Pawn;
        [SerializeField,Range(0,100000)] int m_soldierNum = 1000;
        [SerializeField] Text m_remainText = null;
        [SerializeField] string m_layerName = "LayerPl1";
        float pow;

        Soldier m_soldierScr;
        private void Awake()
        {
            m_soldierScr = null;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_remainText.text = m_soldierNum.ToString();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_soldierScr != null)
            {
                if (m_soldierNum > 0)
                {
                    if (m_soldierNum >= SET_SPD)
                    {
                        pow += (float)SET_SPD;
                        m_soldierNum -= SET_SPD;
                    }
                    else
                    {
                        pow += (float)m_soldierNum;
                        m_soldierNum = 0;
                    }
                    m_soldierScr.SetPower(pow);
                    m_remainText.text = Mathf.FloorToInt(m_soldierNum).ToString();
                }
            }
        }

        public void PointerDown()
        {
            if ((m_soldierNum>0)&&(m_soldierPrefab != null) &&(m_soldierScr == null))
            {
                GameObject go = Instantiate(m_soldierPrefab);
                go.layer = LayerMask.NameToLayer(m_layerName);
                m_soldierScr = go.GetComponent<Soldier>();
                m_soldierScr.plSide = Soldier.PlSide.Pl1;
                Vector3 pos = new Vector3(0.05f, 0.5f, -Camera.main.transform.position.z);
                int mm = go.layer;
                int nn = LayerMask.NameToLayer("LayerPl2");
                Debug.Log(mm + "," + nn);
                if (mm == nn)
                {
                    m_soldierScr.plSide = Soldier.PlSide.Pl2;
                    pos.x = 1f - pos.x;
                }
                pos = Camera.main.ViewportToWorldPoint(pos);
                go.transform.position = pos;
                m_soldierScr.pieceType = m_pieceType;
                pow = 0f;
                m_soldierScr.SetPower(pow);
                m_soldierScr.moveState = Soldier.MoveState.Wait;
            }

        }
        public void PointerUp()
        {
            if (m_soldierScr != null)
            {
                m_soldierScr.moveState = Soldier.MoveState.Move;
                m_soldierScr = null;
            }
        }
    }
}
