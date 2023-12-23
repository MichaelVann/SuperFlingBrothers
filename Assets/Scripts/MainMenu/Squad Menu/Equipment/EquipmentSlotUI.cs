using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour
{
    public TextMeshProUGUI m_nameText;
    public TextMeshProUGUI m_abilityTextRef;
    public TextMeshProUGUI m_affixesTextRef;
    public ArmorSegment m_armorSegment;
    [SerializeField] GameObject m_portraitEmptyText;
    public TextMeshProUGUI m_itemValueTextRef;
    public TextMeshProUGUI m_healthText;
    [SerializeField] internal GameObject m_selectedOutline;

    int m_index = -1;
    Equipment m_equipmentRef;


    // Start is called before the first frame update
    void Start()
    {

    }

    internal void Init(int a_index, Equipment a_equipment)
    {
        m_index = a_index;
        m_equipmentRef = a_equipment;
    }

    void RotateArmorSegment()
    {
        float angle = 0f;

        switch (m_index)
        {
            case 0:
                angle = 45f;
                break;
            case 1:
                angle = -45f;
                break;
            case 2:
                angle = 45 + 90f;
                    break;
            case 3:
                angle = -45f - 90f;
                break;
            case -1:
                angle = 0f;
                break;
        }
        
        m_armorSegment.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public void Refresh()
    {
        bool valid = m_equipmentRef != null;

        if (valid)
        {
            m_armorSegment.AssignEquipment(m_equipmentRef);
            RotateArmorSegment();

            m_abilityTextRef.text = m_equipmentRef.m_activeAbility.GetName();
            m_itemValueTextRef.text = "" + m_equipmentRef.GetSellValue();
            m_healthText.color = VLib.RatioToColorRGB(m_equipmentRef.m_health / m_equipmentRef.m_maxHealth);

            m_nameText.text = m_equipmentRef.m_rarity.name;
            if (m_equipmentRef.m_name != "")
            {
                m_nameText.text += "\"" + m_equipmentRef.m_name + "\"";
            }
            m_nameText.color = m_equipmentRef.m_rarity.color;

            if (m_equipmentRef.m_activeAbility.m_affixes.Count > 0)
            {
                m_affixesTextRef.text = m_equipmentRef.m_activeAbility.GetAffixNames();
                m_affixesTextRef.color = Color.white;
            }
            else
            {
                m_affixesTextRef.text = "No affixes";
                m_affixesTextRef.color = new Color(0.8f, 0.8f, 0.8f);
            }
        }
        else
        {
            m_nameText.text = "Empty";
            m_abilityTextRef.text = "Empty";
            m_itemValueTextRef.text = "0";
            m_affixesTextRef.text = "Equip an equipment to see it's stats.";
            m_affixesTextRef.color = new Color(0.8f, 0.8f, 0.8f);
        }
        m_healthText.text = valid ? m_equipmentRef.m_health.ToString("f1") + "/" + m_equipmentRef.m_maxHealth.ToString("f1") : "";
        
        m_armorSegment.gameObject.SetActive(valid);
        m_portraitEmptyText.SetActive(!valid);
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
