using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using System.Runtime.InteropServices;

namespace DataProject
{
    
    
    
    public partial class Form1 : Form
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

      




        Stack checking_stack = new Stack();
        Stack leveling_stack = new Stack();
       

        private string FileName = string.Empty;

        int est = 0;

        List<error_handler> error = new List<error_handler>();
        List<line_handler> stack_error = new List<line_handler>();
        List<level_line_fixer> linefixlevel = new List<level_line_fixer>();


        struct line_handler
        {
            public int line;
            public string word;
            public bool closer_checker;

        }

        struct error_handler
        {
            public int line ,start,end;
            public bool type;
            public string fixer;

        }

        struct level_line_fixer
        {
            public int line;
            public string word , fixe;

        }

        public void error_higliteer(int higlight)
        {
            //AllocConsole();
           // Console.WriteLine(error[higlight].type);
            
                editor.SelectionColor = Color.Red;
                editor.Navigate(error[higlight].line);


            Range rng = new Range(editor, error[higlight].start, error[higlight].line, error[higlight].end, error[higlight].line);
                editor.Selection = rng;


            if (error[est].type == false)
            {
                label2.Text = "CL:" + (error[est].line+1)  ;
                label3.Text = "ET : Syntax";
            }
            else
            {
                label2.Text = "CL :" + (error[est].line+1) ;
                label3.Text = "ET : Level";
            }



        }


        public void parsing ()
        {


            leveling_stack.Clear(); error.Clear(); stack_error.Clear();
            string zero = null;


            int line = 0, start = 0, end = 0;


            Range rng = new Range(editor, start, line, end, line);

            var error_element = new error_handler { line = 0, start = 0, end = 20, type = false };
            var line_element = new line_handler { line = 0, word = null };

            //  error.Add(error_element);


            for (int i = 0; i < editor.LinesCount; i++)
            {

                checking_stack.Clear();

                string line_text = editor.GetLineText(i).Trim();

                int starter = line_text.IndexOf('<'), ender = line_text.IndexOf('>');

                for (int j = 0; j < line_text.Length; j++)

                {
                    if (line_text[j] == '<')
                    {
                        checking_stack.Push(line_text[j]);



                        if (j + 1 < line_text.Length && line_text[j + 1] == '!')
                        {
                            checking_stack.Pop();
                        }

                        if (j + 1 < line_text.Length && line_text[line_text.IndexOf('<') + 1] != '!'
                         && line_text[line_text.IndexOf('<') + 1] != '?'
                         && line_text[line_text.IndexOf('<') + 1] != '!')

                        {
                            if (line_text.IndexOf('>') - 1 > 0)
                            {
                                bool closer_check = false;
                                short closer = 0; if (line_text[line_text.IndexOf('<', starter) + 1] == '/') { closer = 1; }

                                string s = line_text.Substring(starter + 1 + closer, ender - 1 - starter - closer);

                                starter = line_text.IndexOf('<', starter + 1);
                                ender = line_text.IndexOf('>', ender + 1);

                                if (s.Contains(" ") == true) { s = s.Substring(0, s.IndexOf(' ')); }

                                leveling_stack.Push(s.Trim());

                                if (s[0] == '!') { leveling_stack.Pop(); }


                                if (line_text[line_text.IndexOf('>') - 1] == '/')
                                {
                                    leveling_stack.Pop();
                                }



                                zero = leveling_stack.Peek().ToString();

                                if (closer == 1) { closer_check = true; }

                                line_element = new line_handler { word = zero, line = i + 1, closer_checker = closer_check };
                                stack_error.Add(line_element);



                                if (line_text[line_text.IndexOf('>') - 1] == '/' || s[0] == '!')
                                {
                                    stack_error.RemoveAt(stack_error.Count - 1);
                                }


                            }

                        }

                    }

                    else if (line_text[j] == '>')
                    {
                        if (checking_stack.Count != 0 && checking_stack.Peek().ToString() == "<" && j - 1 > 0 && line_text[j - 1] != '-')
                        { checking_stack.Pop(); }

                        else if (checking_stack.Count == 0 && j - 1 > 0 && line_text[j - 1] != '-')
                        { checking_stack.Push(line_text[j]); }

                        else if (j == 0)
                        { checking_stack.Push(line_text[j]); }

                    }


                }

                if (line_text.Length > 2 && line_text[0] != '<' && line_text[line_text.Length - 2] != '-') { checking_stack.Push(line_text[0]); }

                else if (line_text.Length > 2 && line_text[line_text.Length - 1] != '>' && line_text[1] != '!') { checking_stack.Push(line_text[0]); }


                if (checking_stack.Count != 0)
                {
                    int zero_start = 0;
                    foreach (char x in editor.GetLineText(i))
                    { if (x != ' ') { zero_start = editor.GetLineText(i).IndexOf(x); break; } }

                    error_element = new error_handler { line = i, start = zero_start, end = editor.GetLineText(i).Length, type = false };
                    error.Add(error_element);

                }

            }

        }


        public void error_detector()
        {
            parsing();

            Stack word = new Stack();
            Stack close = new Stack();
            Stack line_error = new Stack();
            


           word.Clear(); close.Clear(); line_error.Clear(); stack_error.Clear();

            var error_element = new error_handler { line = 0, start = 0, end = 20, type = false };
            var level_elemnt = new level_line_fixer { line = 0, word = null, fixe = null };










     
            AllocConsole();
            Console.Clear();
            
            for (int i = 0; i < stack_error.Count; i++)
            {
                Console.WriteLine(stack_error[i].line + "   " + stack_error[i].closer_checker + "   " + stack_error[i].word);
            } 









            for (int i = 0; i < stack_error.Count; i++)

            {


                if (word.Count == 0)
                {
                    word.Push(stack_error[i].word);
                    close.Push(stack_error[i].closer_checker);
                    line_error.Push(stack_error[i].line);

                }

                else if (stack_error[i].word == word.Peek().ToString() && stack_error[i].closer_checker == true && Convert.ToBoolean(close.Peek()) == false)
                {
                    word.Pop(); close.Pop(); line_error.Pop();

                }


                else if (stack_error[i].word != word.Peek().ToString() && stack_error[i].closer_checker == true && Convert.ToBoolean(close.Peek()) == false)
                {

                    level_elemnt = new level_line_fixer { line = stack_error[i].line, word = stack_error[i].word, fixe = word.Peek().ToString() };
                    linefixlevel.Add(level_elemnt);
                    word.Pop(); close.Pop(); line_error.Pop();
                }
                else
                {
                    word.Push(stack_error[i].word);
                    close.Push(stack_error[i].closer_checker);
                    line_error.Push(stack_error[i].line);
                }



            }






            for (int i = 0;  i < linefixlevel.Count; i++)
           {
               Console.WriteLine(linefixlevel[i].line + " ---  " + linefixlevel[i].word + " ---  " + linefixlevel[i].fixe);
                
            }



            foreach (object obj in line_error)
            {

                // Console.WriteLine(obj.ToString());

                //Console.WriteLine((int)obj);
                int zero_start = 0;
                foreach (char x in editor.GetLineText((int)obj - 1))
                { if (x != ' ') { zero_start = editor.GetLineText((int)obj - 1).IndexOf(x); break; } }

                error_element = new error_handler { line = (int)obj - 1, start = zero_start, end = editor.GetLineText((int)obj - 1).Length, type = true };
                error.Add(error_element);
            }


            error = error.OrderBy(sel => sel.line).ToList();
            label1.Text = "Total Errors : " + error.Count.ToString();

            if (error.Count > 0)
            {
                error_higliteer(0);
            }


        }
        

        public void syntax_fixer()
        {
            string line_err = editor.SelectedText.Trim();
           // Console.WriteLine(line_err);

            if (line_err.IndexOf('<')!= 0 && line_err[line_err.IndexOf('<')-1] !=' ')
            {

               

                if (line_err.IndexOf('>') != 0 && line_err[line_err.IndexOf('>') - 1] == '-')
                { }

                else
                {
                    line_err = line_err.Substring(line_err.IndexOf('<'), line_err.Length - line_err.IndexOf('<'));
                    editor.SelectedText = line_err;


                 //   Console.WriteLine(line_err);
                  
                }



            }
                  
            if (line_err.IndexOf('<') == 0 && line_err[line_err.IndexOf('<') + 1] == '<')
            {
                
                line_err = line_err.Substring(line_err.IndexOf('<')+1, line_err.Length - line_err.IndexOf('<')-1);
                // Console.WriteLine(line_err);
                editor.SelectedText = line_err;
            }


         

            if (line_err.LastIndexOf('>') == line_err.Length-1 && line_err[line_err.LastIndexOf('>') -1 ] == '>')
            {

                line_err = line_err.Substring(0, line_err.LastIndexOf('>') );
                //   Console.WriteLine(line_err);
                editor.SelectedText = line_err;

            }




            if (line_err.LastIndexOf('>') !=line_err.Length-1 && line_err[1] != '!' )
            {
               
                    line_err = line_err.Substring(0, line_err.LastIndexOf('>')+1);

                //   Console.WriteLine(line_err);
                editor.SelectedText = line_err;

            }






        }


        public void level_fixer()
        {
            string line_err = editor.SelectedText;
            //Console.WriteLine(line_err);

        }

        public void puttify ()
        {
            parsing();
            AllocConsole();
            List<int> level = new List<int>();

            level.Add(0);
            int level_num = 0;

            for (int i = 2; i < stack_error.Count; i+=2)
            {
               if (stack_error[i].closer_checker == false && stack_error[i-1].closer_checker == false )

                {
                  //  Console.WriteLine(stack_error[i].line);
                  ///  Console.WriteLine(stack_error[i-1].line);
                   // Console.WriteLine("true true");


                    level.Add(level_num + 1);
                    level.Add(level_num + 2);
                    level_num += 2;



                }

                else if (stack_error[i].closer_checker == true && stack_error[i - 1].closer_checker == false)

                {
                   // Console.WriteLine(stack_error[i].line);
                   // Console.WriteLine(stack_error[i - 1].line);

                    level.Add(level_num + 1);
                   // Console.WriteLine("fasle true");


                }

                else if (stack_error[i].closer_checker == false && stack_error[i - 1].closer_checker == true)
                {
                   // Console.WriteLine(stack_error[i].line);
                  //  Console.WriteLine(stack_error[i - 1].line);

                    level.Add(level_num);
                    level.Add(level_num);
                  //  Console.WriteLine("true fasle");
                }


               else if (stack_error[i].closer_checker == true && stack_error[i - 1].closer_checker == true)
                {
                  //  Console.WriteLine(stack_error[i].line);
                  //  Console.WriteLine(stack_error[i - 1].line);
//
                    level.Add(level_num + 1);
                    level.Add(level_num + 2);
                    level_num -= 2;
                  //  Console.WriteLine("fase fasle");

                }


            }


           // Console.WriteLine(level.Count);

           for(int i = 0; i<level.Count; i++)
           {
                Console.WriteLine(level[i]);
            }


           for (int i = stack_error[0].line; i<level.Count;i++)
            {
                editor.Selection = new Range(editor, i);

                
                editor.SelectedText = string.Concat(Enumerable.Repeat("  ", level[i])) + editor.SelectedText;

            }
            editor.Navigate(0);



          

            /*
             * 
             *   count = TOT.Count(f => f == '<');
                count_x = TOT.Count(f => f == '/');
            editor.Selection = new Range(editor, i);

            var zed = editor.Text;
            zed.GetType();
           */

            Console.WriteLine("finish");

           // for (int i = 0; i < stack_error.Count; i++)
           //{
          //   Console.WriteLine(stack_error[i].line + "   " + stack_error[i].closer_checker + "   " + stack_error[i].word);
          // }

        }

        public void ugly()
        {
  
            for (int i = 0; i < editor.LinesCount; i++)
            {
                editor.Selection = new Range(editor, i);

                editor.SelectedText = editor.SelectedText.Trim();

            }
            editor.Navigate(0);

    }



        public void minify()
        {
            string mini = "";
            for (int i = 0; i < editor.LinesCount; i++)
            {

                mini += editor.GetLineText(i).Trim();

            }
            editor.Text = mini;

        }

        public Form1()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editor.TextLength != 0)
            {
                saveToolStripMenuItem_Click(sender, e);
            }
            this.FileName = string.Empty;
            editor.Clear();

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "XML File(.XML)|*.xml|All Files (*.*)|*.*";
            ofd.Title = "Open a Xml File ";
            if (ofd.ShowDialog() == DialogResult.OK)
            {

                editor.Text = File.ReadAllText(ofd.FileName);
               // System.IO.StreamReader sr = new System.IO.StreamReader(ofd.FileName);
             //   editor.RichTextBox.Text = sr.ReadToEnd();
//sr.Close();

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.FileName))
            {
                // call SaveAs 
                saveAsToolStripMenuItem_Click_1(sender, e);
            }
            else
            {
                // we already have the filename. we overwrite that file.
                System.IO.StreamWriter writer = new System.IO.StreamWriter(this.FileName);
                writer.Write(editor.Text);
                writer.Close();
            }
        }
        private void undoToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            editor.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.Cut();
        }

        private void copyToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            editor.Copy();
        }

        private void pasteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            editor.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editor.SelectAll();
        }

        private void saveAsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog saving = new SaveFileDialog();

            saving.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            saving.Filter = "Text Files (.XML)|*.XML|All Files (*.*)|*.*";
            saving.Title = "Save As";
            saving.FileName = "Untitled";

            if (saving.ShowDialog() == DialogResult.OK)
            {
                // save the new FileName in our variable
                this.FileName = saving.FileName;
                System.IO.StreamWriter writing = new System.IO.StreamWriter(saving.FileName);
                writing.Write(editor.Text);
                writing.Close();
            }
        }

        private void teamToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("----------- CSE323 - XML Editor 2021 ----------" + Environment.NewLine + Environment.NewLine +

               "Mohamed Fathi Mohamed Razzk - 16X0103" + Environment.NewLine +
               "Ahmed Mostafa Mostafa Ganina - 16E0030" + Environment.NewLine +
               "Haidy Amr Ahmed AbdElhakeem - 1701635", "Project Team"
               );
        }

        private void customizeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FontDialog dlg = new FontDialog();


            if (dlg.ShowDialog() == DialogResult.OK)
            {

                editor.Font = dlg.Font;

            }
        }

        private void numberedRTB1_Load(object sender, EventArgs e)
        {
           
        }

        private void redToolStripMenuItem_Click(object sender, EventArgs e)
        {
            error_detector();
        }

        private void editor_Load(object sender, EventArgs e)
        {
          


        }

        private void editor_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void errorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (error.Count != 0)
            {
                editor.SelectionColor = Color.Red;

                Range rng = new Range(editor, error[est].start, error[est].line, error[est].end, error[est].line);
                editor.Selection = rng;
                est = (est + 1) % error.Count;

            }
            else
            {
                MessageBox.Show("There is No errors");
            }
            




        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            redToolStripMenuItem_Click( sender,  e);

        }

        private void button2_Click(object sender, EventArgs e)
        {


            if (error.Count != 0)
            {
                est = (est + 1) % error.Count;
                error_higliteer(est);
            }
            else
            {
                MessageBox.Show("There is No errors");
            }

           




        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            if (error.Count != 0)
            {
                est = Math.Abs((est - 1) % error.Count);
                error_higliteer(est);
               

            }
            else
            {
                MessageBox.Show("There is No errors");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AllocConsole();

            if (error[est].type == false)
            {
                syntax_fixer();
            }
            else
            {
                level_fixer();
            }


            //Console.WriteLine(error[est].line+1);



        }

        private void button5_Click(object sender, EventArgs e)
        {
            puttify();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            minify();

        }

        private void uglyFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ugly();
        }
    }


    
}
