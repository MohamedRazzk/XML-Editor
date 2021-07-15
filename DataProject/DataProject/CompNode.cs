using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProject
{
    public class CompNode
    {

        public char Symbol { get; set; }
        public int Frequency { get; set; }
        public CompNode Right { get; set; }
        public CompNode Left { get; set; }

        public List<bool> Traverse(char symbol, List<bool> data)
        { 
            if (Right == null && Left == null)
            {
                if (symbol.Equals(this.Symbol)) { return data;}
                else{return null;}
            }

            else
            {
                List<bool> left = null;
                List<bool> right = null;

                if (Left != null)
                {
                    
                    List<bool> left_branch = new List<bool>();
                    left_branch.AddRange(data);
                    left_branch.Add(false);
                    left = Left.Traverse(symbol, left_branch);
                }

                if (Right != null)
                {
                    List<bool> right_branch = new List<bool>();
                    right_branch.AddRange(data);
                    right_branch.Add(true);
                    right = Right.Traverse(symbol, right_branch);
                }

                if (left != null)
                {
                    return left;
                }
                else
                {
                    return right;
                }
            }
        }
    }
}
