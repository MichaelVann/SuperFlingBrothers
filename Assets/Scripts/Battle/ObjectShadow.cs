using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class ObjectShadow : MonoBehaviour
{
    public void Awake()
    {
    }

    public void Update()
    {
        float eulerAnglesForShadow = transform.eulerAngles.z + GameHandler.BATTLE_ShadowAngle;
        float x = Mathf.Sin(eulerAnglesForShadow * Mathf.PI / 180f) / transform.parent.transform.localScale.x;
        float y = Mathf.Cos(eulerAnglesForShadow * Mathf.PI / 180f) / transform.parent.transform.localScale.y;
        transform.localPosition = new Vector3(x, y) * BattleManager.m_shadowDistance;
    }
}
