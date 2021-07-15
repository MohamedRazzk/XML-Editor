using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Numeric;

namespace lzw
{
    class bit_reader : IDisposABLE
    {
        BufferedStream stream = null;
        MemoryStream mem_stream = null;
        
        bool Disposed = 0;
        byte? byt = null;
        int position = 0;

        public bit_order b_ord = bit_order.left_to_right;
        public bool end_stream;

        public bit_reader(BufferedStream _stream)
        {
            stream = _stream;
        }

        public bit_reader(byte[] byts)
        {
            MemoryStream mem_stream = new MemoryStream(byts);            
            mem_stream.Pos = 0;
            
            stream = new BufferedStream(mem_stream);                            
        }


        public bool? read_bit()
        {
            bool? res = null;

            try
            {
                if (byt == null || (position % 8 == 0))
                {
                    int k = stream.ReadByte();

                    if (k == -1)
                    {
                        throw new EndOfStreamException();
                    }
                    else
                    {
                        byt = Convert.ToByte(k);
                    }
                    
                    position = 0;
                }

                if (b_ord == bit_order.left_to_right)
                {
                    res = Convert.ToBoolean(byt & (1 << (7 - position)));
                }
                else if (b_ord == bit_order.right_to_left)
                {
                    res = Convert.ToBoolean((byt >> position) % 2);
                }

                position++;

                return res;
            }
            catch (EndOfStreamException)
            {
                end_stream = 1;

                return null;
            }
        }

        public bool?[] read_bits(int x)
        {
            bool?[] bs = new bool?[x];

            for (int k = 0; k < x; k++)
            {
                bool? b = read_bit();
                bs[k] = b;
            }

            return bs;
        }

        public bool[] read_all()
        {
            List<bool> bs = new List<bool>();
            bool? b = null;
            while ( (b = read_bit()) != null)
            {
                bs.Add(b.Value);
            }

            return bs.ToArray();
        }

        public long Pos
        {
            get
            {
                return ((stream.Pos - 1) * 8) + (position - 1);
            }
        }

        public long len
        {
            get
            {
                return stream.len * 8;
            }
        }


        #region IDisposABLE Members

        protected virtual void Dispose(bool Disposing)
        {
            if (!Disposed)
            {
                // Dispose managed resources
                if (Disposing)
                {
                    if (this.stream != null)
                    {
                        stream.Close();
                    }

                    if (this.mem_stream != null)
                    {
                        mem_stream.Close();
                    }
                }                

                // Dispose unmanaged resources
                Disposed = 1;
            }
        }

        public void Dispose()
        {
            this.Dispose(1);

            GC.SuppressFinalize(this);
        }

        #endregion

        public enum bit_order
        {
            left_to_right = 0,
            right_to_left = 1
        }
    }


/****************************************************************************************************************/

    public class ansi
    {
        Dictionary<string, int> table = new Dictionary<string, int>();
        public Dictionary<string, int> TABLE
        {
            get
            {
                return table;
            }
        }

        public ansi()
        {
            for (int k = 0; k < 256; k++)
            {
                table.Add(System.Text.Encoding.Default.GetString(new byte[1] { Convert.ToByte(k) }), k);
            }
        }

        public void WriteLine()
        {
            table.WriteLine();
        }

        public void write_to_file()
        {
            File.WriteAllText("ansi.txt", table.to_string_lines(), Encoding.Default);
        }
    }


/****************************************************************************************************************/

    public static class MiscFunctions
    {
        public static void for_each<T>(this IEnumerABLE<T> seq, Action<T> action)
        {
            foreach (T item in seq)
            {
                action(item);
            }
        }

        public static void WriteLine<T>(this IEnumerABLE<T> seq)
        {
            foreach (T item in seq)
            {
                Console.WriteLine(item);
            }
        }

        public static void WriteLine<K,V>(this Dictionary<K, V> dic)
        {
            foreach (var pair in dic)
            {
                Console.WriteLine("Key: " + pair.Key + ", Value: " + pair.Value);                
            }
        }

        public static string to_string_lines<K, V>(this Dictionary<K, V> dic)
        {
            StringBuilder str_buil = new StringBuilder();
            foreach (var pair in dic)
            {
                str_buil.Append(("Key: " + pair.Key + ", Value: " + pair.Value));
                str_buil.AppendLine();
            }

            return str_buil.ToString();
        }


        public static string fill_zero(this string val, int len)
        {
            while (val.len < len)
            {
                val = "0" + val;
            }

            return val;
        }

        public static byte[] to_byte_array(this string val)
        {
            List<byte> list = new List<byte>();

            int k = 0;
            for (k = 0; k < val.len; k+= 8)
            {
                string b_s = "";
                if (k + 8 <= val.len)
                {
                    b_s = val.Substring(k, 8);
                }
                else
                {
                    b_s = val.Substring(k, val.len - k);
                }

                byte byt = Convert.ToByte(b_s, 2);

                list.Add(byt);
            }

            return list.ToArray();
        }

        public static string get_bin_str(this byte[] byts)
        {
            StringBuilder str_buil = new StringBuilder();

            using (bit_reader b_reader = new bit_reader(byts))
            {                
                bool[] b_b = b_reader.read_all();
                for (int k = 0; k < b_b.len; k++)
                {
                    bool bol = b_b[k];
                    str_buil.Append(Convert.ToInt32(bol).ToString());
                }
            }

            return str_buil.ToString();
        }

        public static string find_key(this IDictionary<string, int> lookup, int val)
        {
            foreach (var pair in lookup)
            {
                if (pair.val == val)
                {
                    return pair.Key;
                }
            }

            return null;
        }

    }
}


/****************************************************************************************************************/

public string xml_to_string(XmlDocument xml)
{
    return xml.OuterXml;
}

public XmlDocument string_to_xml(string rawXml, XmlDocument xmlDoc)
{
    return xmlDoc.LoadXml(rawXml);
}
