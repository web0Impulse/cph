using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            label1.Text = getBlockquote();
        }
        void button_OnClick(object sender, System.EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox2.ForeColor = Color.ForestGreen;
                byte[] encodingLetterBytes = Encoding.GetEncoding(1251).GetBytes(textBox1.Text);
                //byte degree = (byte)(2);
                byte degree = Convert.ToByte(textBox3.Text);
                byte bits = (byte)(8);
                FileStream fileWithEncode = new FileStream(textBox2.Text.Replace(".", "(1)."), FileMode.Create);
                fileWithEncode.Close();
                byte[] imageBytes = File.ReadAllBytes(textBox2.Text);
                if (imageBytes.Length == 0)
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\" + textBox2.Text);
                    textBox2.ForeColor = Color.Red;
                }
                else
                {
                    for (int i = 54, counter = 0; i < 54 + (encodingLetterBytes.Length) * (bits / degree); i = i + bits / degree, counter++)
                    {
                        for (int j = i, z = 0; z < bits / degree; j++, z++)
                        {

                            imageBytes[j] = (byte)(imageBytes[j] & getImageMask(degree));// очищаем последниее разряды
                            byte tempTextBits = (byte)((encodingLetterBytes[counter] & getTextMask(degree)) >> bits - degree);// берем биты из байта символа кодируемоего сообщения
                            imageBytes[j] |= tempTextBits;
                            encodingLetterBytes[counter] <<= degree;

                        }
                    }
                    File.WriteAllBytes(textBox2.Text.Replace(".", "(1)."), imageBytes);
                    toolStripStatusLabel1.Text = "Encoded complete";
                }
                //кажется работает
            }
            else if (radioButton2.Checked)
            {
                textBox2.ForeColor = Color.ForestGreen;
                byte[] byteOnDecode = File.ReadAllBytes(textBox2.Text);
                if (byteOnDecode.Length == 0)
                {
                    File.Delete(Directory.GetCurrentDirectory()+"\\"+textBox2.Text);
                    textBox2.ForeColor = Color.Red;
                }
                else
                {
                    int symbolsQuantity = Convert.ToInt16(textBox4.Text);
                    byte degree = Convert.ToByte(textBox3.Text);
                    byte bits = (byte)(8);
                    byte[] reverseOnReturn = new byte[symbolsQuantity];
                    for (int i = 54, k = 0; i < 54 + symbolsQuantity * (bits / degree); i = i + bits / degree, k++)
                    {
                        byte symbol = 0;
                        for (int j = i, z = 0; z < bits / degree; j++, z++)
                        {

                            byte workByte = byteOnDecode[j];
                            workByte &= getReverseMask(degree);
                            symbol <<= degree;
                            symbol |= workByte;

                        }
                        reverseOnReturn[k] = symbol;
                    }
                    string s = Encoding.GetEncoding(1251).GetString(reverseOnReturn);
                    textBox1.Text = s;
                }
            }// и это тоже работает(вроде)
        }
        static byte getImageMask(byte degree)
        {
            return (byte)(255 >> degree << degree);
        }
        static byte getTextMask(byte degree)
        {
            return (byte)(255 >> (byte)(8) - degree << (byte)(8) - degree);

        }
        static byte getReverseMask(byte degree)
        {
            return (byte)(255 >> 8 - degree);
        }
        public string getBlockquote()
        {
            string s = "";
            System.Random rnd = new System.Random();
            switch (rnd.Next(0, 2))
            {
                case 0:
                    s  = "Privacy and secrecy are not the same thing...";
                    break;
                case 1:
                    s = "Privacy is the ability to choose what information about yourself to open to the world...";
                    break;
                case 2:
                    s = "Encryption is the main way to protect your privacy...";
                    break;
            }
            return s;
        }
    };
}

