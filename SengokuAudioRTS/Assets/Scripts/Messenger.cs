using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SARTS
{
    public class Messenger : MonoBehaviour
    {
        [SerializeField] TMPro.TextMeshPro m_tmPro=null;
        [SerializeField, Range(0.1f,10f)] float m_spdGain=1f;
        float m_direction = 0f;

        // Start is called before the first frame update
        void Start()
        {
            Destroy(gameObject, 20f);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += Vector3.right * Time.deltaTime * m_spdGain * m_direction;
        }

        public void SetResultMessage(Soldier _soldier, bool _isWin)
        {
            if (m_tmPro != null)
            {
                string mes = "";
                switch (_soldier.pieceType)
                {
                    default: mes = "足軽隊"; break;
                    case Soldier.PieceType.Knight: mes = "騎兵隊 "; break;
                    case Soldier.PieceType.Tank: mes = "鉄砲隊 "; break;
                }
                mes += _isWin ? "勝利" : "全滅";
                m_tmPro.text = "伝令！！\n" + mes+"\n" + "にございます！";
                m_tmPro.color = _isWin ? Color.yellow : Color.red;
                m_direction = (_soldier.plSide == Soldier.PlSide.Pl1) ? -1f : 1f;
            }
        }
    }
}
