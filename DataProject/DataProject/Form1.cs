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
using Microsoft.VisualBasic;
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


        public void shifter (int line, int sapce)
        {
            editor.Selection = new Range(editor, line - 1);

            for (int mu = 0; mu < sapce; mu++)
            {
                editor.IncreaseIndent();
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
            


           word.Clear(); close.Clear(); line_error.Clear(); linefixlevel.Clear();

            var error_element = new error_handler { line = 0, start = 0, end = 20, type = false };
            var level_elemnt = new level_line_fixer { line = 0, word = null, fixe = null };



            AllocConsole();
            Console.Clear();


            Console.WriteLine(stack_error.Count);
            for (int i = 0; i < stack_error.Count; i++)
            {
                Console.WriteLine(stack_error[i].line + "   " + stack_error[i].closer_checker + "   " + stack_error[i].word);
            }



            Console.WriteLine("we are here ");


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

            

            foreach (level_line_fixer obj in linefixlevel)
            {
                
                
                int zero_start = 0;
                foreach (char x in editor.GetLineText(obj.line - 1))
                { if (x != ' ') { zero_start = editor.GetLineText(obj.line - 1).IndexOf(x); break; } }

                error_element = new error_handler { line = obj.line - 1, start = zero_start, end = editor.GetLineText(obj.line - 1).Length, type = true };
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
            int selectedline = editor.Selection.FromLine+1;
            level_line_fixer fix_word = linefixlevel.FirstOrDefault(x => (x.line) == selectedline);
            editor.SelectedText = line_err.Replace(fix_word.word, fix_word.fixe); ;
          

        }

        public void puttify (int treespacing)
        {
            parsing();

            int spacecap = 0;
            for (int z =1; z <=editor.LinesCount; z++)

            {

                var newList = stack_error.FindAll(s => s.line == z);
                int capper = 0;


                for (int i = 0; i < newList.Count; i++)
                {
                    if (newList[i].closer_checker == false) { capper++; }
                    else { capper--; }
                }


                editor.Selection = new Range(editor, z-1);


                if (capper == 1)
                {
                    shifter(z, spacecap* treespacing);
                    spacecap++;
                }


                else if(capper == -1 )
                {
                    spacecap--;
                    shifter(z, spacecap* treespacing);
                }

  
                else if (capper == 0)
                {
                    shifter(z, spacecap* treespacing);
                }

            }

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

        private static DialogResult treedaialog(ref int input)
        {
            System.Drawing.Size size = new System.Drawing.Size(250, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.MaximizeBox = false;
            inputBox.Text = "Tree Spacing View ";

            System.Windows.Forms.NumericUpDown numerical= new NumericUpDown();
            numerical.Size = new System.Drawing.Size(size.Width - 10, 23);
            numerical.Location = new System.Drawing.Point(5, 5);
            numerical.Value = input;
            inputBox.Controls.Add(numerical);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input =(int)numerical.Value;
            return result;
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
         
            error_detector();

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
            label1.Text = "Total Errors : " + error.Count.ToString();
            label2.Text = "CL : $";
            label3.Text = "ET : Null";
            error.RemoveAt(est);
            button2_Click(sender, e);
            





            //Console.WriteLine(error[est].line+1);



        }

        private void button5_Click(object sender, EventArgs e)
        {
            puttify(1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            minify();

        }

        private void uglyFormatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ugly();
        }

        private void horToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int tree_space = 0;
            treedaialog(ref tree_space);

            puttify(tree_space);
        }
    }


    
}
