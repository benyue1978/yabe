﻿/**************************************************************************
*                           MIT License
* 
* Copyright (C) 2015 Frederic Chaxel <fchaxel@free.fr>
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to
* the following conditions:
*
* The above copyright notice and this permission notice shall be included
* in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.BACnet;
using System.Windows.Forms;

namespace Yabe
{
    public partial class AlarmSummary : Form
    {
        BACnetDevice device;


        IList<BacnetGetEventInformationData> Alarms = new List<BacnetGetEventInformationData>();

        public AlarmSummary(ImageList img_List, BACnetDevice device)
        {
            InitializeComponent();
            this.Text = "Active Alarms on Device Id " + device.deviceId.ToString();
            this.device = device;

            TAlarmList.ImageList = img_List;

            Application.UseWaitCursor = true;

            Application.DoEvents();
        }

        private void AlarmSummary_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F4)
            {
                Alarms.Clear();
                AlarmSummary_Shown(null, null);

            }
        }

        private void AlarmSummary_Shown(object sender, EventArgs e)
        {
            // get the Alarm summary
            // Addentum 135-2012av-1 : Deprecate Execution of GetAlarmSummary, GetEVentInformation instead
            // -> parameter 2 in the method call
            bool Ret;
            Ret = device.channel.GetAlarmSummaryOrEventRequest(device.BacAdr, true, ref Alarms); // try GetEVentInformation

            if (!Ret)
                Ret = device.channel.GetAlarmSummaryOrEventRequest(device.BacAdr, false, ref Alarms); // try GetAlarmSummary

            if (Ret)
            {
                LblInfo.Visible = false;
                FillTreeNode();
                AckText.Enabled = AckBt.Enabled = true;
            }
            else
                LblInfo.Text = "Service not available on this device";

            Application.UseWaitCursor = false;
            Cursor.Current = Cursors.Default; // sometimes required, else not back since a click !

        }
        private static string GetEventStateNiceName(String name)
        {
            name = name.Substring(12);
            name = name.Replace('_', ' ');
            name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
            return name;
        }
        private static string GetEventEnableNiceName(String name)
        {
            name = name.Substring(13);
            name = name.Replace('_', ' ');
            name = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
            return name;
        }

        private void FillTreeNode()
        {
            int icon;

            TAlarmList.Nodes.Clear();

            TAlarmList.BeginUpdate();

            // Only one network read request to get the object name
            int _retries = device.channel.Retries;
            device.channel.Retries = 1;

            // fill the Treenode
            foreach (BacnetGetEventInformationData alarm in Alarms)
            {
                if ((alarm.acknowledgedTransitions.ToString() != "111") || (alarm.eventState != BacnetEventNotificationData.BacnetEventStates.EVENT_STATE_NORMAL))
                {
                    TreeNode currentTn;

                    String nameStr = device.GetObjectName(alarm.objectIdentifier);

                    if (nameStr == null)
                    {
                        // Get the property Name, network activity, time consuming
                        IList<BacnetValue> name;
                        bool retcode = device.channel.ReadPropertyRequest(device.BacAdr, alarm.objectIdentifier, BacnetPropertyIds.PROP_OBJECT_NAME, out name);

                        if (retcode)
                        {
                            nameStr = name[0].Value.ToString();
                        }
                    }

                    icon = YabeMainDialog.GetIconNum(alarm.objectIdentifier.type);
                    if (nameStr != null)
                    {
                        if (!Properties.Settings.Default.DisplayIdWithName)
                            currentTn = new TreeNode(nameStr, icon, icon);
                        else
                            currentTn = new TreeNode(nameStr + " (" + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(alarm.objectIdentifier.ToString()) + ")", icon, icon);

                        currentTn.ToolTipText = alarm.objectIdentifier.ToString();
                    }
                    else
                        currentTn = new TreeNode(alarm.objectIdentifier.ToString(), icon, icon);

                    currentTn.Tag = alarm;
                    TAlarmList.Nodes.Add(currentTn);

                    if (Properties.Settings.Default.ShowDescriptionWhenUseful)
                    {
                        String Descr = "";
                        try
                        {
                            // Get the Description, network activity, time consuming
                            IList<BacnetValue> name;
                            bool retcode = device.channel.ReadPropertyRequest(device.BacAdr, alarm.objectIdentifier, BacnetPropertyIds.PROP_DESCRIPTION, out name);

                            if (retcode)
                                Descr = name[0].Value.ToString();
                        }
                        catch { }

                        currentTn.Nodes.Add(new TreeNode("Description : " + Descr, Int32.MaxValue, Int32.MaxValue));
                    }

                    icon = Int32.MaxValue; // out bound
                    currentTn.Nodes.Add(new TreeNode("Alarm state : " + GetEventStateNiceName(alarm.eventState.ToString()), icon, icon));


                    TreeNode tn2 = new TreeNode("Ack Required :", icon, icon);
                    bool ackrequired = false;
                    for (int i = 0; i < 3; i++)
                    {
                        if (alarm.acknowledgedTransitions.ToString()[i] == '0')
                        {
                            BacnetEventNotificationData.BacnetEventEnable bee = (BacnetEventNotificationData.BacnetEventEnable)(1 << i);
                            String text = GetEventEnableNiceName(bee.ToString()) + " since " + alarm.eventTimeStamps[i].Time.ToString();
                            tn2.Nodes.Add(new TreeNode(text, icon, icon));
                            ackrequired = true;
                        }
                    }

                    if (!ackrequired) tn2 = new TreeNode("No Ack Required, already done", icon, icon);
                    currentTn.Nodes.Add(tn2);
                }
            }

            // set back the request retries number
            device.channel.Retries = _retries;

            TAlarmList.EndUpdate();

            TAlarmList.ExpandAll();

            if (TAlarmList.Nodes.Count == 0)
            {
                LblInfo.Visible = true;
                LblInfo.Text = "Empty event list ... all is OK";
            }
        }

        private bool AcqAlarm(BacnetGetEventInformationData alarm)
        {
            bool SomeChanges = false;
            for (int i = 0; i < 3; i++) // 3 transitions maybe to be ack To_Normal, To_OfNormal, To_Fault
            {
                if (alarm.acknowledgedTransitions.ToString()[i] == '0') // Transition to be ack, 1 means ok/already done
                {
                    BacnetGenericTime bgt;

                    if (alarm.eventTimeStamps != null)
                        bgt = alarm.eventTimeStamps[i];
                    else // Deprecate Execution of GetAlarmSummary
                    {
                        // Read the event time stamp, we do not have it 
                        IList<BacnetValue> values;
                        if (device.channel.ReadPropertyRequest(device.BacAdr, alarm.objectIdentifier, BacnetPropertyIds.PROP_EVENT_TIME_STAMPS, out values, 0, (uint)i) == false)
                        {
                            Trace.TraceWarning("Error reading PROP_EVENT_TIME_STAMPS");
                            return false;
                        }
                        String s1 = ((BacnetValue[])(values[0].Value))[0].ToString(); // Date & 00:00:00 for Hour
                        String s2 = ((BacnetValue[])(values[0].Value))[1].ToString(); // 00:00:00 & Time
                        DateTime dt = Convert.ToDateTime(s1.Split(' ')[0] + " " + s2.Split(' ')[1]);
                        bgt = new BacnetGenericTime(dt, BacnetTimestampTags.TIME_STAMP_DATETIME);
                    }

                    // something to clarify : BacnetEventStates or BacnetEventEnable !!!
                    BacnetEventNotificationData.BacnetEventStates eventstate = (BacnetEventNotificationData.BacnetEventStates)(2 - i);

                    if (device.channel.AlarmAcknowledgement(device.BacAdr, alarm.objectIdentifier, eventstate, AckText.Text, bgt,
                                new BacnetGenericTime(DateTime.Now, BacnetTimestampTags.TIME_STAMP_DATETIME)) == true)
                    {
                        alarm.acknowledgedTransitions.SetBit((byte)i, true);
                        SomeChanges = true;
                    }
                }
            }

            return SomeChanges;
        }

        private void AckBt_Click(object sender, EventArgs e)
        {
            List<TreeNode> listacq = new List<TreeNode>();

            foreach (TreeNode t in TAlarmList.SelectedNodes)
            {
                TreeNode t2 = t;
                while (t2.Parent != null) t2 = t2.Parent;  // go to the root element
                if (!listacq.Exists((o) => o == t2))
                    listacq.Add(t2);
            }

            foreach (TreeNode t in listacq)
            {
                BacnetGetEventInformationData alarm = (BacnetGetEventInformationData)t.Tag; // the alam content
                AcqAlarm(alarm);
            }

            FillTreeNode();
        }

        // Used if the Read without retries has fail
        private void TAlarmList_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode tn = e.Node;
            while (tn.Parent != null) tn = tn.Parent;

            if (tn.ToolTipText == "")
            {
                BacnetGetEventInformationData alarm = (BacnetGetEventInformationData)tn.Tag;
                IList<BacnetValue> name;

                device.channel.ReadPropertyRequest(device.BacAdr, alarm.objectIdentifier, BacnetPropertyIds.PROP_OBJECT_NAME, out name);

                tn.ToolTipText = tn.Text;

                if (Properties.Settings.Default.DisplayIdWithName)
                    tn.Text = name[0].Value.ToString() + " (" + System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(tn.ToolTipText) + ")";
                else
                    tn.Text = name[0].Value.ToString();

            }

        }

    }
}
