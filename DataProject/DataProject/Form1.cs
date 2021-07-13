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


        struct line_handler
        {
            public int line;
            public string word;
            public bool closer_checker;

        }

        struct error_handler
        {
            public int line ,start,end;

        }

        public void error_higliteer(int higlight)
        {

            
                editor.SelectionColor = Color.Red;
                editor.Navigate(error[higlight].line);

                Range rng = new Range(editor, error[higlight].start, error[higlight].line, error[higlight].end, error[higlight].line);
                editor.Selection = rng;
            



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

          //  AllocConsole();

            Stack word = new Stack();
            Stack close = new Stack();
            Stack line_error = new Stack();



            leveling_stack.Clear(); error.Clear(); word.Clear(); close.Clear(); line_error.Clear(); stack_error.Clear();
            string zero = null;


            int line = 0, start = 0, end = 0  ;
            

            Range rng = new Range(editor, start, line,end, line);

            var error_element = new error_handler { line=0 ,start = 0,end = 20 };
            var line_element = new line_handler { line = 0 , word = null };
            //  error.Add(error_element);


        

            for (int i = 0; i < editor.LinesCount; i++)
                {
     
                checking_stack.Clear();

                string line_text = editor.GetLineText(i).Trim();

                int starter = line_text.IndexOf('<'), ender = line_text.IndexOf('>');

                for (int j = 0; j<line_text.Length; j++)

                {
                    if (line_text[j] == '<'  )
                    {
                        checking_stack.Push(line_text[j]);
                        
                        

                        if (j+1 < line_text.Length && line_text[j+1] == '!')
                        {
                            checking_stack.Pop();
                        }

                  if (j + 1 < line_text.Length && line_text[line_text.IndexOf('<') + 1] != '!'
                   && line_text[line_text.IndexOf('<') + 1] != '?'
                   && line_text[line_text.IndexOf('<') + 1] != '!')

                        {
                            if (line_text.IndexOf('>') -1> 0)
                            {
                                bool closer_check = false;
                                short closer = 0; if (line_text[line_text.IndexOf('<',starter) + 1] == '/') { closer = 1;  }
                                  
                                string s = line_text.Substring(starter+1+closer    ,   ender-1-starter-closer);

                                starter = line_text.IndexOf('<',starter+1);
                                ender = line_text.IndexOf('>', ender + 1);

                                if (s.Contains(" ")==true) { s = s.Substring(0, s.IndexOf(' '));}
                                 
                                    leveling_stack.Push(s.Trim());

                                if (s[0] == '!') { leveling_stack.Pop(); }


                                if (line_text[line_text.IndexOf('>') - 1] == '/')
                                {
                                    leveling_stack.Pop();
                                }



                                 zero = leveling_stack.Peek().ToString();
                                
                                if(closer == 1) { closer_check = true; }
                                
                                line_element = new line_handler { word = zero,line=i+1 , closer_checker= closer_check };
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
                        {
                            checking_stack.Pop();

                        }

                        else if (checking_stack.Count == 0 && j - 1 > 0 && line_text[j - 1] != '-')
                        {
                            checking_stack.Push(line_text[j]);
                        }

                        else if (j == 0)
                        {
                            checking_stack.Push(line_text[j]);
                        }                         
                    }
                    

                }
               
                if (line_text.Length > 2 && line_text[0] != '<'  && line_text[line_text.Length-2] !='-')  { checking_stack.Push(line_text[0]);}

                else if (line_text.Length > 2 && line_text[line_text.Length - 1] != '>' && line_text[1] != '!' ){ checking_stack.Push(line_text[0]); }


                if (checking_stack.Count != 0)
                {
                    error_element = new error_handler { line = i, start = 0, end = editor.GetLineText(i).Length };
                    error.Add(error_element);   
                    
                }

            }



            
            

            if (stack_error.Count >= 1)
            {
                
            

                for (int i = 0; i < stack_error.Count; i++)
                {
                    if ( i>0 && word.Peek().ToString() == stack_error[i].word && Convert.ToBoolean(close.Peek()) != stack_error[i].closer_checker )
                    {
                        word.Pop();
                        close.Pop();
                    }

                    
                    else if ( i > 0 && word.Peek().ToString() != stack_error[i].word &&  stack_error[i].closer_checker == true)
                    {
                        word.Pop();
                        close.Pop();
                        line_error.Push(stack_error[i].line);
                    }
                    else if (i > 0 && word.Peek().ToString() == stack_error[i].word && stack_error[i].closer_checker == false)
                    {
                       
                        line_error.Push(stack_error[i].line);
                    }



                    else {

                        word.Push((stack_error[i].word));
                        close.Push(stack_error[i].closer_checker);

                    }

                }
            }


           foreach (object obj in line_error)
            {

               // Console.WriteLine(obj.ToString());

                //Console.WriteLine((int)obj);

                error_element = new error_handler { line =(int)obj-1, start = 0, end = editor.GetLineText((int)obj-1).Length };
                error.Add(error_element);
            }



            label1.Text = "Error Num : " + error.Count.ToString();



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
    }


    
}
