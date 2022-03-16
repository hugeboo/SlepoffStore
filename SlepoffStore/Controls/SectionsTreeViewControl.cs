using SlepoffStore.Core;
using SlepoffStore.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SlepoffStore.Controls
{
    public partial class SectionsTreeViewControl : UserControl, ISectionSelector
    {
        public event EventHandler<GenericEventArgs<SectionEx>> SectionSelected;
        public event EventHandler<GenericEventArgs<Category>> CategorySelected;

        public SectionsTreeViewControl()
        {
            InitializeComponent();
        }

        public void Init()
        {
            Fill();
            treeView.ExpandAll();
            if (treeView.SelectedNode == null && treeView.Nodes.Count > 0) treeView.SelectedNode = treeView.Nodes[0];
        }

        private void Fill()
        {
            IEnumerable<SectionEx> sections;
            using var repo = Program.CreateRepository();
            sections = repo.GetSectionsEx();

            treeView.Nodes.Clear();
            foreach(var section in sections)
            {
                var secNode = new TreeNode(section.Name) { Tag = section };
                treeView.Nodes.Add(secNode);
                foreach(var category in section.Categories)
                {
                    var catNode = new TreeNode(category.Name) { Tag = category };
                    secNode.Nodes.Add(catNode);
                }
            }
        }

        private void FireSelectedNodeEvent(TreeNode node)
        {
            if (node != null)
            {
                if (node.Tag is SectionEx sec)
                {
                    SectionSelected?.Invoke(this, new GenericEventArgs<SectionEx>(sec));
                }
                else if (node.Tag is Category cat)
                {
                    CategorySelected?.Invoke(this, new GenericEventArgs<Category>(cat));
                }
            }
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var ht = treeView.HitTest(e.Location);
                if (ht.Node != null) treeView.SelectedNode = ht.Node;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            FireSelectedNodeEvent(e.Node);
        }
    }

    public interface ISectionSelector
    {
        event EventHandler<GenericEventArgs<SectionEx>> SectionSelected;
        event EventHandler<GenericEventArgs<Category>> CategorySelected;
    }
}
