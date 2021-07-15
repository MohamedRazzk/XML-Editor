using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xml2json
{
    public class JsonTree
    {
        public string Name { get; set; }
        public List<List<JsonTree>> Children { get; set; }
        private TagTreeNode Prepare_for_Json(TagTreeNode N)
        {
            TagTreeNode M = N;
            M.Name = "\"" + M.Name + "\"";
            if (!String.IsNullOrEmpty(M.Text))
                M.Properties.Add(("#text", ("\"" + M.Text + "\"")));
            for (int i = 0; i < M.Properties.Count; i++)
            {
                var x = M.Properties[i];
                x.attr = "\"" + x.attr + "\"";
                M.Properties[i] = x;
            }
            M.Text = null;
            //if (M.Children != null)
            //{
            //    for (int i = 0; i < Children.Count; i++)
            //    {
            //        M.Children[i] = Prepare_for_Json(M.Children[i]);
            //    }
            //}
            return M;
        }
        private JsonTree(string Value)
        {
            this.Name = Value;
            this.Children = null;
        }
        private JsonTree(string init_name, string init_value)
        {
            this.Name = init_name;
            this.Children = new List<List<JsonTree>>();
            List<JsonTree> Child = new List<JsonTree>();
            Child.Add(new JsonTree(init_value));
            this.Children.Add(Child);
        }
        public JsonTree(TagTreeNode N)
        {
            TagTreeNode M = Prepare_for_Json(N);
            this.Name = M.Name;
            this.Children = new List<List<JsonTree>>();
            if (M.Properties.Count == 1)
            {
                List<JsonTree> Child = new List<JsonTree>();
                Child.Add(new JsonTree(M.Properties[0].val));
                this.Children.Add(Child);
            }
            else
            {
                for (int i = 0; i < M.Properties.Count; i++)
                {
                    List<JsonTree> Child = new List<JsonTree>();
                    Child.Add(new JsonTree(M.Properties[i].attr, M.Properties[i].val));
                    this.Children.Add(Child);
                }
            }
            if (M.Children != null && M.Children.Count != 0)
            {
                while (M.Children.Count > 0)
                {
                    var Same_name_props = new List<TagTreeNode>();
                    Same_name_props.Add(M.Children[0]);
                    M.Children.RemoveAt(0);
                    for (int i = 0; i < M.Children.Count; i++)
                    {
                        if (M.Children[i].Name == Same_name_props[0].Name)
                        {
                            Same_name_props.Add(M.Children[i]);
                            M.Children.RemoveAt(i);
                            i--;
                        }
                    }
                    // Now we have all same name children
                    List<JsonTree> SameNameChild = new List<JsonTree>();
                    for (int i = 0; i < Same_name_props.Count; i++)
                    {
                        SameNameChild.Add(new JsonTree(Same_name_props[i]));
                    }
                    this.Children.Add(SameNameChild);
                }
            }
        }
    }
}
