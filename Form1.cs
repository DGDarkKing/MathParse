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

            try
            {
                mathFunc.Expr = richTextBoxFunc.Text;
                BuildGraph();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message + "\nГрафик не может быть построен");
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








       
    }
}
