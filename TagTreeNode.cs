using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2json
{
    public class TagTreeNode
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public TagTreeNode Parent { get; set; }
        public List<TagTreeNode> Children { get; set; }
        public List<(string attr, string val)> Properties { get; set; }
        public TagTreeNode()
        {
            this.Parent = null;
            Properties = new List<(string attr, string val)>();
            Children = new List<TagTreeNode>();
        }
        public void Add_child(TagTreeNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}