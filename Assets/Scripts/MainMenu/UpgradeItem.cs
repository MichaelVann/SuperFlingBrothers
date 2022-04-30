using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class UpgradeItem
{
    public string m_name;
    public string m_description;

    public float m_cost;
    public bool m_owned = false;
    public int m_level = 1;

    public void SetName(string a_string) { m_name = a_string; }
    public void SetDescription(string a_string) { m_description = a_string; }
    public void SetCost(float a_cost) { m_cost = a_cost; }

    public void SetOwned(bool a_value) { m_owned = a_value; }

    public void SetLevel(int a_value) { m_level = a_value; }

    public UpgradeItem()
    {

    }

    public void Update()
    {

    }
}
