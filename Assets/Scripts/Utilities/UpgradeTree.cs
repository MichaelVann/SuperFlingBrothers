using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameHandler;
using static UpgradeItem;

public class UpgradeTree
{
    internal List<UpgradeItem> m_upgradeItemList;

    // Start is called before the first frame update

    internal bool HasUpgrade(UpgradeItem.UpgradeId a_upgradeId) { return m_upgradeItemList[(int)a_upgradeId].m_owned; }

    internal UpgradeTree()
    {
        m_upgradeItemList = new List<UpgradeItem>();
        SetupUpgrades();
    }

    UpgradeItem NewUpgrade(UpgradeItem.UpgradeId a_ID, string a_name, int a_cost, int a_maxLevel, UpgradeItem a_precursorUpgrade, string a_description)
    {
        UpgradeItem upgrade = new UpgradeItem(a_ID,a_name, a_cost, a_maxLevel, a_precursorUpgrade, a_description);
        m_upgradeItemList.Add(upgrade);
        return upgrade;
    }

    void SetupUpgrades()
    {
        UpgradeItem playerVector = NewUpgrade(UpgradeItem.UpgradeId.playerVector, "Player Vector", 20, 0, null, "Shows the direction of player movement.");
          UpgradeItem enemyVector = NewUpgrade(UpgradeItem.UpgradeId.enemyVector, "Enemy Vectors", 30, 0, playerVector, "Shows the direction of all enemies movement.");
            UpgradeItem comboHits = NewUpgrade(UpgradeItem.UpgradeId.comboHits, "Combo Hits", 100, 0, enemyVector, "Adds 10% extra damage on each hit per turn.");

        UpgradeItem battleScouting = NewUpgrade(UpgradeItem.UpgradeId.battleScouting, "Battle Scouting", 50, 0, null, "Reveals the environmental effects of each battle.");
        //UpgradeItem test1 = NewUpgrade(UpgradeItem.UpgradeId.battleScouting, "Test1", 50, 0, playerVector, "Reveals the environmental effects of each battle.");
        //UpgradeItem test2 = NewUpgrade(UpgradeItem.UpgradeId.battleScouting, "Test2", 50, 0, battleScouting, "Reveals the environmental effects of each battle.");
        //UpgradeItem test3 = NewUpgrade(UpgradeItem.UpgradeId.battleScouting, "Test3", 50, 0, battleScouting, "Reveals the environmental effects of each battle.");
        //UpgradeItem test4 = NewUpgrade(UpgradeItem.UpgradeId.battleScouting, "Test4", 50, 0, test1, "Reveals the environmental effects of each battle.");
    }

    internal List<UpgradeItem> GetInitialUpgradeItems()
    {
        List<UpgradeItem> returnList = new List<UpgradeItem>();
        for (int i = 0; i < m_upgradeItemList.Count; i++)
        {
            if (m_upgradeItemList[i].m_precursorUpgrade == null)
            {
                returnList.Add(m_upgradeItemList[i]);
            }
        }
        return returnList;
    }

    void RefreshUpgrades()
    {
        for (int i = 0; i < m_upgradeItemList.Count; i++)
        {
            m_upgradeItemList[i].Refresh();
        }
    }

    internal void AttemptToBuyUpgrade(UpgradeItem a_upgrade)
    {
        float cash = GameHandler.m_staticAutoRef.GetCurrentCash();
        if (a_upgrade.m_cost <= cash)
        {
            GameHandler.m_staticAutoRef.ChangeCash(-a_upgrade.m_cost);
            a_upgrade.m_level++;
            a_upgrade.m_cost *= a_upgrade.m_costScaling;
            if (!a_upgrade.m_owned)
            {
                a_upgrade.SetOwned(true);
            }
            RefreshUpgrades();
        }
    }
}
