using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataProject
{
    class xml_cov
    {
        // Problems with  this function:
        /* This Function still cannot ignore XML versioning and comments
         * This Function also needs to be supplied XML attributes without spaces
         * Both of these problems have been fixed
         * To make this code stable it must be developed using state machines.
         */
        public TagTreeNode Parse_xml(string in_str)
        {
            TagTreeNode curr_node = null;
            for (int i = 0; i < in_str.Length; i++)
            {
                if (in_str[i] == '<')
                {
                    if ((in_str[i + 1] == '!') && (in_str[i + 2] == '-') && (in_str[i + 3] == '-'))
                    {
                        i += 6;
                        while (true)
                        {
                            if (in_str[i] == '>' && (in_str[i - 1] == '-') && in_str[i - 2] == '-')
                                break;
                            i++;
                        }
                    }
                    else if (in_str[i + 1] == '?')
                    {
                        i += 3;
                        while (true)
                        {
                            if (in_str[i] == '>' && in_str[i - 1] == '?')
                                break;
                            i++;
                        }
                    }
                    else
                    {
                        string tag = null;
                        for (i++; (i < in_str.Length) && (in_str[i] != '>'); i++)
                            tag += in_str[i];
                        if (tag[0] != '/')
                        {
                            List<string> tag_info = new List<string>();
                            string TagName = "";
                            int k = 0;
                            while (k < tag.Length && tag[k] != ' ')
                            {
                                TagName += tag[k];
                                k++;
                            }
                            k++;
                            bool IsBrktClosed = true;
                            string TagData = "";
                            while (k < tag.Length)
                            {
                                if (tag[k] == ' ' && IsBrktClosed)
                                {
                                    if (TagData != "")
                                        tag_info.Add(TagData);
                                    TagData = "";
                                }
                                else
                                {
                                    TagData += tag[k];
                                    if (tag[k] == '\"')
                                    {
                                        IsBrktClosed = !IsBrktClosed;
                                    }
                                }
                                k++;
                            }
                            if (TagData != "")
                                tag_info.Add(TagData);
                            TagData = "";
                            // new List<string>(tag.Split(' '));
                            TagTreeNode Child_node = new TagTreeNode();
                            Child_node.Parent = curr_node;
                            if (curr_node != null)
                                curr_node.Add_child(Child_node);
                            curr_node = Child_node;
                            curr_node.Name = TagName;
                            for (int j = 0; j < tag_info.Count; j++)
                            {
                                string[] key_val_pair = tag_info[j].Split('=');
                                (string key, string val) KV = (key_val_pair[0], key_val_pair[1]);
                                curr_node.Properties.Add(KV);
                            }
                        }
                        else
                        {
                            curr_node.Text = curr_node.Text.Trim();
                            if (curr_node.Parent != null)
                                curr_node = curr_node.Parent;
                        }
                    }
                }
                else
                {
                    if (curr_node != null)
                        curr_node.Text += in_str[i];
                }
            }
            return curr_node;
        }

        string Increment_by(int n_tabs)
        {
            string E = "";
            for (int i = 0; i < n_tabs; i++)
                E += "  ";
            return E;
        }


        public string GenerateJSONx(JsonTree X, int lvl = 0, bool WriteName = true)
        {
            string Addition = "";
            if (X.Children != null && X.Children.Count != 0)
                Addition = Addition.Insert(0, Increment_by(lvl));
            if (WriteName == true)
                Addition += X.Name;
            if (X.Children != null && X.Children.Count != 0)
            {
                if (WriteName == true)
                    Addition += ": ";
                if (X.Children.Count > 1)
                    Addition += "{\n";
                // Lets play with children
                for (int i = 0; i < X.Children.Count; i++)
                {
                    if (X.Children[i].Count == 1)
                    {
                        JsonTree Child = X.Children[i][0];
                        int Childlvl = (X.Children.Count > 1) ? lvl + 1 : 0;
                        Addition += GenerateJSONx(Child, Childlvl);
                    }
                    else
                    {
                        Addition += Increment_by(lvl + 1) + X.Children[i][0].Name + ": [\n";
                        for (int j = 0; j < X.Children[i].Count; j++)
                        {
                            Addition += GenerateJSONx(X.Children[i][j], lvl + 2, false);
                            if (j < X.Children[i].Count - 1)
                                Addition += ",\n";
                        }
                        Addition += "\n" + Increment_by(lvl + 1) + "]";
                    }
                    if (i < X.Children.Count - 1)
                        Addition += ",\n";
                }
                if (X.Children.Count > 1)
                    Addition += "\n" + Increment_by(lvl) + "}";
            }
            return Addition;
        }

        static void Main(string[] args)
        {
            string testcase = 
                System.IO.File.ReadAllText
                (@"D:\Ahmed Genina\Documents\3rd CSE 2nd Semester\Programming With Data Structures\proj\xml2json\xmltest2.txt");
            // Put your XML string above.
            var Main_class_inst = new Program();
            TagTreeNode N = Main_class_inst.Parse_xml(testcase);
            JsonTree X = new JsonTree(N);
            string z = Main_class_inst.GenerateJSONx(X);
        }
    }
}