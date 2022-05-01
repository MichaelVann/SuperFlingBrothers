using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class UpgradeItem
{
    public string m_name;
    public string m_description;

    public float m_cost;
    public float m_costScaling = 3f;
    public bool m_owned = false;
    public bool m_hasLevels = false;
    public int m_level = 0;
    public int m_maxLevel = 10;

    public void SetName(string a_string) { m_name = a_string; }
    public void SetDescription(string a_string) { m_description = a_string; }
    public void SetCost(float a_cost) { m_cost = a_cost; }
    public void SetCostScaling(float a_cost) { m_costScaling = a_cost; }

    public void SetOwned(bool a_value) { m_owned = a_value; }
    public void SetHasLevels(bool a_value) { m_hasLevels = a_value; }

    public void SetLevel(int a_value) { m_level = a_value; }
    public void SetMaxLevel(int a_value) { m_maxLevel = a_value; }

    public UpgradeItem()
    {

    }

    public void Update()
    {

    }
}
