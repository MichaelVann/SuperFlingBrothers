using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDigest : MonoBehaviour
{
    public TextMeshProUGUI m_affixText;
    public TextMeshProUGUI m_abilityNameRef;
    public EquipmentInventoryHandler m_equipmentInventoryHandlerRef;
    public EquipmentInteractButton m_interactButtonRef;
    [SerializeField] GameObject m_toOverviewButtonRef;
    [SerializeField] EquipmentSlotUI m_slotUIRef;

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
        if (m_equipmentInventoryHandlerRef == null)
        {
            m_equipmentInventoryHandlerRef = FindObjectOfType<EquipmentInventoryHandler>();
        }
        if (m_equipmentInventoryHandlerRef != null)
        {
            linkedToSquadScreen = true;
        }

        if (linkedToSquadScreen)
        {
            m_interactButtonRef.Init(m_gameHandlerRef, m_equipmentInventoryHandlerRef, m_equipmentRef);
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
            m_slotUIRef.Init(-1, m_equipmentRef);
        }
        m_slotUIRef.Refresh();

        m_affixText.text = "";
        m_affixText.text += m_equipmentRef.m_activeAbility.GetAbilityDescription() + '\n';
    }

    public void Close()
    {
        if (m_equipmentInventoryHandlerRef)
        {
            m_equipmentInventoryHandlerRef.SetEquipmentDigestStatus(false);
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
            m_equipmentInventoryHandlerRef.SetEquipStatus(m_equipmentRef);
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
