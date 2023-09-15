using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBody
{
    public string m_firstName;
    public string m_lastName;
    public BodyPart[] m_bodyPartList;
    public BodyPart m_activeBodyPart;
    public Town m_playerResidingTown;
    //float m_basePartHealth = 100;
    public int m_maxEnemyDifficulty = 12;

    public bool m_bodyInitialised = false;


    public void SetBodyPartLockedStatus(BodyPart.eType a_type, bool a_unlocked) { m_bodyPartList[(int)a_type].m_unlocked = a_unlocked; }

    public HumanBody()
    {
        SetUpBodyParts();
        string[] names = VLib.GenerateRandomPersonsName();
        m_firstName = names[0];
        m_lastName = names[1];
        m_activeBodyPart = m_bodyPartList[(int)BodyPart.eType.Hand];
        m_playerResidingTown = m_activeBodyPart.FindTownByName("Teston");
        RefreshBodyParts();
    }

    private void RefreshBodyParts()
    {
        for (int i = 0; i < m_bodyPartList.Length; i++)
        {
            m_bodyPartList[i].Refresh();
        }
    }

    private void SetUpBodyParts()
    {
        m_bodyPartList = new BodyPart[(int)BodyPart.eType.Count];
        for (int i = 0; i < (int)BodyPart.eType.Count; i++)
        {
            SetUpDefaultBodyPart(ref m_bodyPartList[i], i);
        }
    }

    public void SetUpDefaultBodyPart(ref BodyPart a_part, int a_index)
    {
        ref BodyPart part = ref a_part;
        int invaderStrength = 0;
        //BodyPart.Health health = new BodyPart.Health();
        //health.max = 0;
        bool unlocked = false;

        if (unlocked)
        {
            invaderStrength = 50;
        }
        //health.value = health.max *= m_basePartHealth;

        a_part = new BodyPart((BodyPart.eType)a_index, invaderStrength, unlocked, this);
    }
}
