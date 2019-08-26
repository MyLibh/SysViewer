using System;
using System.Collections;
using System.Management;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SysViewer
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();

            comboBox.SelectedItem = "Win32_Processor";
        }

        private void InsertInfo(string Key, bool DontInsertNull)
        {
            listView.Items.Clear();

            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + Key);
            try
            {
                foreach (ManagementObject share in searcher.Get())
                {

                    ListViewGroup group;
                    try
                    {
                        group = listView.Groups.Add(share["Name"].ToString(), share["Name"].ToString());
                    }
                    catch
                    {
                        group = listView.Groups.Add(share.ToString(), share.ToString());
                    }

                    if (share.Properties.Count <= 0)
                    {
                        MessageBox.Show("No Information Available", "No Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        return;
                    }

                    foreach (PropertyData PC in share.Properties)
                    {

                        ListViewItem item = new ListViewItem(group);
                        if (listView.Items.Count % 2 != 0)
                            item.BackColor = Color.White;
                        else
                            item.BackColor = Color.WhiteSmoke;

                        item.Text = PC.Name;

                        if (PC.Value != null && PC.Value.ToString() != "")
                        {
                            switch (PC.Value.GetType().ToString())
                            {
                                case "System.String[]":
                                    string[] str = (string[])PC.Value;

                                    string str2 = "";
                                    foreach (string st in str)
                                        str2 += st + " ";

                                    item.SubItems.Add(str2);
                                    break;

                                case "System.UInt16[]":
                                    ushort[] shortData = (ushort[])PC.Value;

                                    string tstr2 = "";
                                    foreach (ushort st in shortData)
                                        tstr2 += st.ToString() + " ";

                                    item.SubItems.Add(tstr2);
                                    break;

                                default:
                                    item.SubItems.Add(PC.Value.ToString());
                                    break;
                            }
                        }
                        else
                        {
                            if (!DontInsertNull)
                                item.SubItems.Add("No Information available");
                            else
                                continue;
                        }

                        listView.Items.Add(item);
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("Can't get data because of the followeing error \n" + exp.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RemoveNullValue()
        {
            foreach (ListViewItem item in listView.Items)
                if (item.SubItems[1].Text == "No Information available")
                    item.Remove();
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox.Checked)
                RemoveNullValue();
            else
                InsertInfo(comboBox.SelectedItem.ToString(), false);

        }

        private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InsertInfo(comboBox.SelectedItem.ToString(), checkBox.Checked);
        }
    }
}
