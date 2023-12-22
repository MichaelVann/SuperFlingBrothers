using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDigest : MonoBehaviour
{
    public Text m_levelText;
    public Text m_nameText;
    public TextMeshProUGUI m_affixText;
    public EquipmentPortrait m_portraitRef;
    public TextMeshProUGUI m_abilityNameRef;
    public Text m_itemValueTextRef;
    public EquipmentInventoryHandler m_equipmentInventoryHandler;
    public EquipmentInteractButton m_interactButtonRef;
    [SerializeField] GameObject m_toOverviewButtonRef;

    GameHandler m_gameHandlerRef;

    Equipment m_equipmentRef;

    public void SetEquipmentRef(Equipment a_equipment) { m_equipmentRef = a_equipment; }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void Refresh()
    {
        bool linkedToSquadScreen = false;
        if (m_gameHandlerRef == null)
        {
            m_gameHandlerRef = FindObjectOfType<GameHandler>();
        }
        if (m_equipmentInventoryHandler == null)
        {
            m_equipmentInventoryHandler = FindObjectOfType<EquipmentInventoryHandler>();
        }
        if (m_equipmentInventoryHandler != null)
        {
            linkedToSquadScreen = true;
        }

        if (linkedToSquadScreen)
        {
            m_interactButtonRef.Init(m_gameHandlerRef, m_equipmentInventoryHandler.m_squadOverviewHandlerRef, m_equipmentRef);
            m_interactButtonRef.SetEquipButtonStatus();
            m_interactButtonRef.gameObject.SetActive(true);
            m_toOverviewButtonRef.SetActive(false);
        }
        else
        {
            m_interactButtonRef.gameObject.SetActive(false);
            m_toOverviewButtonRef.SetActive(true);
        }
        bool valid = m_equipmentRef != null;


        if (m_equipmentRef != null)
        {
            m_portraitRef.SetEquipmentRef(m_equipmentRef);

            m_abilityNameRef.text = m_equipmentRef.m_activeAbility.GetName();
            m_itemValueTextRef.text = "" + m_equipmentRef.GetSellValue();
        }
        else
        {
            m_itemValueTextRef.text = "0";
        }

        m_levelText.text = valid ? "Level: " + m_equipmentRef.m_level : "";
        m_nameText.text = valid ? m_equipmentRef.m_name : "";
        m_nameText.color = valid ? m_equipmentRef.m_rarity.color : Color.white;

        m_portraitRef.gameObject.SetActive(valid);
        m_affixText.text = "";
        m_affixText.text += m_equipmentRef.m_activeAbility.GetAbilityDescription() + '\n';
    }

    public void Close()
    {
        if (m_equipmentInventoryHandler)
        {
            m_equipmentInventoryHandler.SetEquipmentDigestStatus(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InteractButtonPressed()
    {
        if (m_equipmentRef.IsBroken())
        {
            m_gameHandlerRef.AttemptToRepairEquipment(m_equipmentRef);
        }
        else
        {
            m_equipmentInventoryHandler.SetEquipStatus(m_equipmentRef);
        }
        Refresh();
        Close();
    }

    public void ToOverview()
    {
        FindObjectOfType<SquadScreenHandler>().OpenSquadOverview();
        Close();
    }

    // Update is called once per frame
    void Update()
    {
        //Refresh();
    }
}
