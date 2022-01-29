using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathParse
{
    // sin(x^(x-8))+2
    public partial class Form1 : Form
    {
        MathFuncParser mathFunc;
        double a, b, x;
        const double h = 0.01;

        public Form1()
        {
            InitializeComponent();

            a = double.Parse(textBoxFrom.Text);
            b = double.Parse(textBoxTo.Text);
            mathFunc = new MathFuncParser("sin(x)");
            chartFuncGraph.Series[1].ToolTip = "(X = #VALX, Y = #VALY)";

            BuildGraph();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            richTextBoxFunc.Text = richTextBoxFunc.Text.Replace(" ", "");
            richTextBoxFunc.Text = richTextBoxFunc.Text.ToLower();

            if (CheckFunc())
            {
                mathFunc.Expr = richTextBoxFunc.Text;
                BuildGraph();
            }
            else
            {
                MessageBox.Show("График не может быть построен");
            }
        }

        


        void BuildGraph()
        {
            chartFuncGraph.Series[0].Points.Clear();
            chartFuncGraph.Series[1].Points.Clear();

            x = a;
            double yNew, yOld;

            A:
            try
            {
                yOld = mathFunc.Calc(x);

            }
            catch (Exception)
            {
                x += h;
                goto A; 
            }

            do
            {
                try
                {
                    chartFuncGraph.Series[0].Points.AddXY(x, yOld);
                    x += h;
                    yNew = mathFunc.Calc(x);


                    yOld = yNew;
                }
                catch (Exception)
                {
                    x += h;
                }


            } while (x <= b);
        }








        private bool CheckFunc()
        {
            int countOpenBracket = 0, countCloseBracket = 0;
            for (int i = 0; i < richTextBoxFunc.Text.Length; i++)
            {
                i = richTextBoxFunc.Text.IndexOf('(', i);
                if (i != -1)
                    countOpenBracket++;
                else
                    break;
            }

            for (int i = 0; i < richTextBoxFunc.Text.Length; i++)
            {
                i = richTextBoxFunc.Text.IndexOf(')', i);
                if (i != -1)
                    countCloseBracket++;
                else
                    break;
            }

            if(countCloseBracket!=countOpenBracket)
            {
                MessageBox.Show("Не хватает скобок");
                return false;
            }



            foreach (var item in MathFuncParser.baseFuncs)
            {
                int i = richTextBoxFunc.Text.IndexOf(item);
                while(i!=-1)
                {
                    if (richTextBoxFunc.Text[i + item.Length] != '(')
                    {
                        MessageBox.Show("Нет открывающейся скобоки после " + item);
                        return false;
                    }

                    i = richTextBoxFunc.Text.IndexOf(item, i+1);
                }
            }



            return true;
        }
    }
}
