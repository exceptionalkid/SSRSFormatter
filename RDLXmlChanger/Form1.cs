using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RDLXmlChanger
{
    public partial class Form1 : Form
    {
        IEnumerable<string> Files;

        string Pattern1A = string.Empty;
        string Pattern1B = string.Empty;

        string Pattern2A = string.Empty;
        string Pattern2B = string.Empty;

        string Pattern3A = string.Empty;
        string Pattern3B = string.Empty;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
                Files = Directory.EnumerateFiles(textBox1.Text, "*.rdl");
                label1.Text = Files.ToList().Count.ToString() + " RDL files found!";
                progressBar1.Maximum = Files.ToList().Count;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            if (Files == null) return;
            string DestinationPath = string.Empty;
           
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                DestinationPath = folderBrowserDialog2.SelectedPath;
            }

            Pattern1A = CreatePattern(nudFrom.Value) + ";"; //0.0000;
            Pattern1B = CreatePattern(nudTo.Value) + ";";

            Pattern2A = CreatePattern(nudFrom.Value) + ")"; //0.0000)
            Pattern2B = CreatePattern(nudTo.Value) + ")";

            Pattern3A = CreatePattern(nudFrom.Value);  //0.0000
            Pattern3B = CreatePattern(nudTo.Value);

            ChangeXML(DestinationPath);  
        }
        string CreatePattern(decimal Number)
        {
            StringBuilder sbDecimal = new StringBuilder();
            sbDecimal.Append("0.");
            for (int i = 1; i <= Number; i++)
                sbDecimal.Append("0");

            return sbDecimal.ToString();

        }
        void ChangeXML(string DestinationPath)
        {
            decimal ToDecimal = nudTo.Value;
            if (!string.IsNullOrEmpty(DestinationPath))
            {
                try
                {
                    int C = 0;
                    foreach (string s in Files)
                    {
                        try
                        {
                            C += 1;
                            progressBar1.Value = C;

                            XmlDocument xDo = new XmlDocument();
                            xDo.Load(s);
                            XmlNodeList l = xDo.GetElementsByTagName("Format");
                            foreach (XmlNode N in l)
                            {
                                if (N.InnerXml.StartsWith("#"))
                                {
                                    int LastNum = 0;
                                    if (int.TryParse(N.InnerXml.Substring(N.InnerXml.Length - 1), out LastNum))
                                    {
                                        N.InnerXml = N.InnerXml.Replace(Pattern3A, Pattern3B);
                                    }
                                    else
                                        N.InnerXml = N.InnerXml.Replace(Pattern1A, Pattern1B).Replace(Pattern2A, Pattern2B);
                                }
                                else if (N.InnerXml.StartsWith("0."))
                                {
                                    try
                                    {
                                        Convert.ToDouble(N.InnerXml);
                                        int inDxDot = N.InnerXml.IndexOf('.');
                                        if (inDxDot >= 0)
                                        {
                                            if (N.InnerXml.Substring(inDxDot + 1).Length != ToDecimal)
                                            {
                                                N.InnerXml = N.InnerXml.Replace(Pattern3A, Pattern3B);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        N.InnerXml = N.InnerXml.Replace(Pattern1A, Pattern1B).Replace(Pattern2A, Pattern2B);
                                    }
                                }
                            }
                            xDo.Save(Path.Combine(DestinationPath, Path.GetFileName(s)));

                            listBox2.Items.Add(Path.GetFileName(s) + "  -  successfully changed");
                            listBox2.SelectedIndex += 1;
                        }
                        catch  
                        {
                            listBox1.Items.Add("Exception - " + s);
                            listBox1.SelectedIndex += 1;
                        }
                    }
                    MessageBox.Show("Completed!", "Format Changer", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception Ex)
                {
                    MessageBox.Show(Ex.Message, "Format Changer", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
                
    }
}
