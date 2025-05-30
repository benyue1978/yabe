/**************************************************************************
*                           MIT License
* 
* Copyright (C) 2025 Frederic Chaxel <fchaxel@free.fr>
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
using System.Drawing;
using System.Windows.Forms;

namespace Yabe
{
    class DummyUserDeviceView { } // A dummy class required to avoid opening an empty Form with the designer  

    // All TreeNode are under a Root "Device Class View"
    // Tag is a BacnetDevice or for Folder a List<int> with deviceId of all affected devices
    // DCVNodeNotAffected is the Folder Node for devices with unknow destination
    public partial class YabeMainDialog
    {
        TreeNode DCVNodeNotAffected; // The Tree Node for NotAffected Devices
        TreeNode CreateDeviceClassView()
        {

            String Descr = Properties.Settings.Default.DeviceClassStructure;

            if (Descr == null) Descr = "";

            // Root Node
            TreeNode tnDeviceClass = new TreeNode("Device Class View");
            tnDeviceClass.Tag = -2;    // For the Node sorter, Network View got the value -1

            String[] Folders = Descr.Split(';');
            List<TreeNode> AddedTn = new List<TreeNode>(); // More simple to do in a List than in a TreeNode.Nodes

            foreach (string Folder in Folders)
            {
                string[] elements = Folder.Split(new char[] { '(', ')', ',' });

                if (string.IsNullOrWhiteSpace(elements[0]))
                    continue;

                TreeNode tnFolder = new TreeNode(elements[0].Trim(), 11, 11); // with a folder icon
                tnFolder.Name = tnFolder.Text; // Used for TreeNode Ordering by alphabatical order
                List<int> DeviceIds = new List<int>();
                tnFolder.Tag = DeviceIds;

                for (int i = 1; i < elements.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(elements[i]))
                        if (Int32.TryParse(elements[i], out int id) == true)  // It's a DeviceId
                            DeviceIds.Add(id);
                        else // It's a reference to a previous group
                        {
                            int idx = AddedTn.FindIndex(o => o.Text.ToLower() == elements[i].Trim().ToLower());
                            if (idx != -1) // maybe an error
                            {
                                tnFolder.Nodes.Add(AddedTn[idx]);
                                AddedTn.Remove(AddedTn[idx]);
                            }
                        }
                }

                AddedTn.Add(tnFolder);
            }

            if (AddedTn.Count == 0)
            {
                TreeNode tn = new TreeNode("New Folder", 11, 11);
                tn.Tag = new List<int>(); // Empty List
                tn.Name = tn.Text;
                AddedTn.Add(tn);
            }

            // Node for not affecte devices is "hidden" if it's name blank
            if (!string.IsNullOrEmpty(Properties.Settings.Default.NotAffectedFolderName))
            {
                DCVNodeNotAffected = new TreeNode(Properties.Settings.Default.NotAffectedFolderName, 11, 11);
                DCVNodeNotAffected.Tag = new List<int>(); // Empty List
                DCVNodeNotAffected.Name = new string((char)255, 1); // For ordering, always last folder
                AddedTn.Add(DCVNodeNotAffected);
            }

            foreach (TreeNode tn in AddedTn)
                tnDeviceClass.Nodes.Add(tn);

            m_DeviceTree.Nodes.Add(tnDeviceClass);
            return tnDeviceClass;

        }
        // By default device are added first in the Network view, then cloned to be add in this view
        bool AddToDeviceClassView(TreeNode deviceNode, TreeNode CurrentNode = null, bool IsAffected = false)
        {
            if ((DeviceClassViewTreeNode == null) || (deviceNode == null)) return false;

            if (CurrentNode == null) CurrentNode = DeviceClassViewTreeNode;

            // Find and add at all right places
            foreach (TreeNode tn in CurrentNode.Nodes)
                if ((tn.Tag is List<int>)) // Folder node ?
                    if (AddToDeviceClassView(deviceNode, tn, IsAffected) == true) // recursive call
                        IsAffected = true;

            if (CurrentNode.Tag is List<int> DeviceIds)
            {
                if (DeviceIds.Contains((int)(deviceNode.Tag as BACnetDevice).deviceId))
                {
                    CurrentNode.Nodes.Add((TreeNode)deviceNode.Clone());
                    IsAffected = true;
                }
            }

            // Not added, put it in the folder for all not affected devices
            if ((IsAffected == false) && (CurrentNode == DeviceClassViewTreeNode) && (DCVNodeNotAffected != null))
                DCVNodeNotAffected.Nodes.Add((TreeNode)deviceNode.Clone());

            return IsAffected;
        }

        #region "DeviceClassViewModification"

        private void RewriteDeviceClassStructureSettings(TreeNode tnParent, ref String DeviceClassStructure)
        {
            foreach (TreeNode tn in tnParent.Nodes)
            {
                if ((tn.Tag is List<int> listdevices) && (tn != DCVNodeNotAffected))
                {
                    RewriteDeviceClassStructureSettings(tn, ref DeviceClassStructure); // recursive call to childs

                    DeviceClassStructure += tn.Text + "(";

                    String Sep = "";
                    foreach (int devid in listdevices) // The list of deviceId allowed
                    {
                        DeviceClassStructure += Sep + devid;
                        Sep = ",";
                    }
                    foreach (TreeNode tn2 in tn.Nodes) // The list of Folder inside the current Folder
                    {
                        if (tn2.Tag is List<int>)
                            DeviceClassStructure += Sep + tn2.Text;
                        Sep = ",";
                    }
                    DeviceClassStructure += ");";

                }
            }
        }
        private void MoveNode(TreeNode SourceNode, TreeNode DestinationNode)
        {

            if (SourceNode.Tag is BACnetDevice dev) // Remove the device identifier for the OrignalDevice list
            {
                List<int> d = SourceNode.Parent.Tag as List<int>;

                if (d == null) return; // should never occurs

                d.Remove((int)dev.deviceId);

                if (DestinationNode != null)
                {
                    d = DestinationNode.Tag as List<int>;
                    if (!d.Exists(o => o == (int)dev.deviceId))
                        d.Add((int)dev.deviceId);
                }
            }

            m_DeviceTree.Nodes.Remove(SourceNode);
            DestinationNode?.Nodes.Add(SourceNode);

            // Rewrite the Setting parameter
            String DeviceClassStructure = "";
            RewriteDeviceClassStructureSettings(DeviceClassViewTreeNode, ref DeviceClassStructure);
            Properties.Settings.Default.DeviceClassStructure = DeviceClassStructure;

        }
        private void m_DeviceTree_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void m_DeviceTree_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode SourceNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            Point DropXY = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
            TreeNode DestinationNode = ((TreeView)sender).GetNodeAt(DropXY);

            if ((DestinationNode==null)||(SourceNode==null)) return;

            // Nothing inside a Device, but assume the target is the parent Folder
            if (DestinationNode.Tag is BACnetDevice)
                DestinationNode = DestinationNode.Parent;

            if (SourceNode == DestinationNode) return;

            // No Device at the top level
            if ((SourceNode.Tag is BACnetDevice) && (DestinationNode == DeviceClassViewTreeNode)) return;

            // No Folder inside the Unaffected Devices Folder
            if ((SourceNode.Tag is List<int>) && (DestinationNode == DCVNodeNotAffected)) return;

            // Not possible too put a parent inside it's child node
            TreeNode tnParent = DestinationNode.Parent;
            while (tnParent != null)
            {
                if (tnParent == SourceNode) return;
                tnParent = tnParent.Parent;
            }

            MoveNode(SourceNode, DestinationNode);

            m_DeviceTree.ExpandAll();

        }

        private void m_DeviceTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode node = e.Item as TreeNode;

            if (node == DCVNodeNotAffected) return;

            while (node.Parent != null) // only allow a child TreeNode of DeviceClassViewTreeNode
                node = node.Parent;
            if (node == DeviceClassViewTreeNode)
                m_DeviceTree.DoDragDrop(e.Item, DragDropEffects.Move); // A BacnetDevice Node
        }
        private void DCVViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            void RewriteSetting()
            {
                String DeviceClassStructure = "";
                RewriteDeviceClassStructureSettings(DeviceClassViewTreeNode, ref DeviceClassStructure);
                Properties.Settings.Default.DeviceClassStructure = DeviceClassStructure;
            }

            TreeNode Node = m_DeviceTree.SelectedNode;

            if (sender == InserttoolStripMenuItem)
                if ((Node == DeviceClassViewTreeNode) || (Node?.Parent == DeviceClassViewTreeNode))
                {
                    var Input =
                    new GenericInputBox<TextBox>("Device View Class", "New Folder name",
                        (o) =>
                        {
                            // adjustment to the generic control
                        }, 1, true);
                    DialogResult res = Input.ShowDialog();

                    if (res == DialogResult.OK)
                    {
                        TreeNode tn = new TreeNode(Input.genericInput.Text, 11, 11);
                        tn.Tag = new List<int>();
                        tn.Name = tn.Text;
                        Node.Nodes.Add(tn);
                        m_DeviceTree.ExpandAll();
                        RewriteSetting();
                    }
                    return;
                }

            if (!(Node.Tag is List<int>)) return;

            if ((sender == DeletetoolStripMenuItem) && (Node != DCVNodeNotAffected))
            {
                if (MessageBox.Show("Sure to delete " + Node.Text, "Folder delete", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    m_DeviceTree.Nodes.Remove(m_DeviceTree.SelectedNode);
                    m_DeviceTree.ExpandAll();
                    RewriteSetting();
                }
            }

            if (sender == RenameStripMenuItem)
            {

                var Input =
                new GenericInputBox<TextBox>("Device View Class", "New Folder name",
                    (o) =>
                    {
                        o.Text = Node.Text;
                        // adjustment to the generic control
                    }, 1, true);
                DialogResult res = Input.ShowDialog();

                if (res == DialogResult.OK)
                {
                    Node.Text = Node.Name = Input.genericInput.Text;
                    m_DeviceTree.ExpandAll();

                    if (Node == DCVNodeNotAffected)
                        Properties.Settings.Default.NotAffectedFolderName = Input.genericInput.Text;

                    RewriteSetting();
                }
            }

        }

        private void UnaffectDeviceToolStripMenuItem2_Click(object sender, EventArgs e)
        {

            TreeNode Node = m_DeviceTree.SelectedNode;
            if (!(Node.Tag is BACnetDevice) || !(Node.Parent.Tag is List<int>)) return;

            MoveNode(Node, DCVNodeNotAffected);

            m_DeviceTree.ExpandAll();

        }

        private void duplicateDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // It's a problem to duplicate a node in the same Folder, when one is move the deviceId is
            // remove from the accepted list. Don't want to check, so forbideen.
            if (DCVNodeNotAffected == null) return;

            TreeNode Node = m_DeviceTree.SelectedNode;
            if (!(Node.Tag is BACnetDevice) || !(Node.Parent.Tag is List<int>)) return;

            TreeNode newNode = (TreeNode)Node.Clone();
            DCVNodeNotAffected.Nodes.Add(newNode);

            m_DeviceTree.ExpandAll();
        }

        #endregion

    }
}