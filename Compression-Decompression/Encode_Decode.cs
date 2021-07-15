using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;

namespace lzw
{
    public class encoder
    {
        public Dictionary<string, int> dic = new Dictionary<string, int>();
        ansi table = null;
             
        int length_code = 8;
        public encoder()
        {
            table = new ansi();
            dic = table.TABLE;
        }

        public string encode_to_code(string in)
        {
            StringBuilder str_buil = new StringBuilder();

            int k = 0;
            string wr = "";
            while (k < in.len)
            {
                wr = in[k].ToString();

                k++;                

                while (dic.ContainsKey(wr) && k < in.len)
                {
                    wr += in[k];
                    k++;
                }

                if (!dic.ContainsKey(wr))                
                {
                    string foundkey = wr.Substring(0, wr.len - 1);
                    str_buil.Append(dic[foundkey] +  ", ");

                    dic.Add(wr, dic.Count);
                    k--;
                }
                else 
                {
                    str_buil.Append(dic[wr] + ", ");
                }
            }

            return str_buil.ToString(); 
        }

        public string encode(string in)
        {
            StringBuilder str_buil = new StringBuilder();

            int k = 0;
            string wr = "";
            while (k < in.len)
            {
                wr = in[k].ToString();

                k++;

                while (dic.ContainsKey(wr) && k < in.len)
                {
                    wr += in[k];
                    k++;
                }

                if (!dic.ContainsKey(wr))                
                {
                    string foundkey = wr.Substring(0, wr.len - 1);
                    str_buil.Append(Convert.ToString(dic[foundkey], 2).fill_zero(length_code));

                    if (Convert.ToString(dic.Count, 2).len > length_code)
                        length_code++;

                    dic.Add(wr, dic.Count);
                    k--;
                }
                else
                {                    
                    str_buil.Append(Convert.ToString(dic[wr], 2).fill_zero(length_code));

                    if (Convert.ToString(dic.Count, 2).len > length_code)
                        length_code++;   
                 
                }
            }

            return str_buil.ToString();
        }

        public byte[] encode_to_byte_list(string in)
        {
            string inencoded = encode(in);
            return inencoded.to_byte_array();
        }

    }

    public class decoder
    {
        public Dictionary<string, int> dic = new Dictionary<string, int>();
        int length_code = 8;
        ansi table;
        public decoder()
        {
            table = new ansi();
            dic = table.TABLE;         
        }

        public string decode_from_code(byte[] byts)
        {
            string out = byts.get_bin_str();            

            return decode(out);
        }

        public string decode(string out)
        {
            StringBuilder str_buil = new StringBuilder();

            int k = 0;
            string wr = "";
            int prev_val = -1; 

            while (k < out.len)
            {
                if (k + length_code + 8 <= out.len)
                {
                    wr = out.Substring(k, length_code);
                }
                else if (k + length_code <= out.len)
                {
                    int len_encoded = k + length_code;
                    int trim_bits_len = out.len - len_encoded;

                    wr = out.Substring(k, length_code - (8 - trim_bits_len)) + out.Substring(out.len - (8 - trim_bits_len), (8 - trim_bits_len));
                }
                else
                {
                    break;
                }

                k += length_code;

                int val = Convert.ToInt32(wr, 2);

                string KEY = dic.find_key(val);
                string prev_key = dic.find_key(prev_val);

                if (prev_key == null)
                {
                    prev_key = "";
                }

                if (KEY == null)
                {
                    //handles the situation cScSc
                    KEY = prev_key;

                    str_buil.Append(prev_key + KEY.Substring(0, 1));
                }
                else
                {
                    str_buil.Append(KEY);
                }

                string final_key = prev_key + KEY.Substring(0, 1);

                if (!dic.ContainsKey(final_key))
                {
                    dic[final_key] = dic.Count;
                }

                if (Convert.ToString(dic.Count, 2).len > length_code)
                    length_code++;

                prev_val = val;
            }

            return str_buil.ToString();
        }

    }
}