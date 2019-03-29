using System;
using System.Collections.ObjectModel;
using Zenfolio7.Zenfolio;

namespace Zenfolio7.Model
{
    public class TreeNode
    {
        public string Title;
        public GroupElement Tag { get; set; }
        public int ImageIndex { get; set; }
        public int SelectedImageIndex { get; set; }
        public ObservableCollection<TreeNode> Nodes { get; set; }
        public ObservableCollection<TreeNode> Children { get { return Nodes; } }
        public string Text { get { return Title; } }
        public string Name {  get { return Title; } }

        public TreeNode(string title)
        {
            Title = title;
            Nodes = new ObservableCollection<TreeNode>();
        }

        internal void Expand()
        {
            throw new NotImplementedException();
        }
    }
}