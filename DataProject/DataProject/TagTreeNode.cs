using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProject
{
    public class property
    {
        public string attr;
        public string val;

    }
    public class TagTreeNode
    {
       
        public string Name { get; set; }
        public string Text { get; set; }
        public TagTreeNode Parent { get; set; }
        public List<TagTreeNode> Children { get; set; }
        public List<property> Properties { get; set; }
        public TagTreeNode()
        {
            this.Parent = null;
            Properties = new List<property>();
            Children = new List<TagTreeNode>();
        }
        public void Add_child(TagTreeNode child)
        {
            child.Parent = this;
            Children.Add(child);
        }
    }
}