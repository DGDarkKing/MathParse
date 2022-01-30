using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MathParse
{
    class MathFuncParser
    {
        public static readonly string[] baseFuncs = new string[] { "log", "sin", "cos", "tg", "ctg" };
        static readonly string[] mathConsts = new string[] { "e", "pi" };
        public static readonly char[] mathOpers = new char[] { '+', '-', '*', '/', '^' };



        Node root;
        string expr;
        public string Expr
        {
            get
            {
                return expr;
            }

            set
            {
                SetExpr(value);
            }
        }




        public MathFuncParser(string mathExpression)
        {
            Expr = mathExpression;
        }



       

        //
        public void SetExpr(string expression)
        {
            CheckFunc(expression);

            expr = expression;
            List<string> tokenList = new List<string>();
            tokenSplit(tokenList);

            List<Node> nodeList = new List<Node>();

            int brackeOpenOuter = tokenList.IndexOf("(");
            int brackeOpenInner = tokenList.IndexOf("(", brackeOpenOuter + 1);
            int brackeCloseOuter = tokenList.IndexOf(")");
            while (tokenList.Contains("("))
            {
                if (brackeOpenInner != -1 && brackeOpenInner < brackeCloseOuter)
                {
                    brackeOpenOuter = brackeOpenInner;
                    brackeOpenInner = tokenList.IndexOf("(", brackeOpenOuter + 1);
                }
                else
                {

                    disassemble(tokenList, brackeOpenOuter + 1, brackeCloseOuter - 1, nodeList);

                    bool isFunc = false;

                    foreach (var item in baseFuncs)
                    {
                        if (tokenList[brackeOpenOuter - 1] == item)
                        {
                            Node nodeF = new NodeFunc(item);

                            int nodeInd = int.Parse(tokenList[brackeOpenOuter + 1].Substring(1));
                            nodeF.children[0] = nodeList[nodeInd];
                            nodeF.children[0].parent = nodeF;

                            nodeList.RemoveAt(nodeInd);
                            nodeList.Add(nodeF);

                            tokenList.RemoveAt(brackeOpenOuter - 1);
                            tokenList.RemoveAt(brackeOpenOuter - 1);
                            tokenList.RemoveAt(brackeOpenOuter - 1);
                            tokenList.RemoveAt(brackeOpenOuter - 1);

                            tokenList.Insert(brackeOpenOuter - 1, "N" + (nodeList.Count - 1).ToString());


                            isFunc = true;
                            break;
                        }
                    }


                    if (!isFunc)
                    {
                        tokenList.RemoveAt(brackeOpenOuter + 2);
                        tokenList.RemoveAt(brackeOpenOuter);

                    }

                    brackeOpenOuter = tokenList.IndexOf("(");
                    brackeOpenInner = tokenList.IndexOf("(", brackeOpenOuter + 1);
                    brackeCloseOuter = tokenList.IndexOf(")");
                }
            }


            disassemble(tokenList, 0, tokenList.Count - 1, nodeList);


            root = nodeList[0];

        }


        //
        private void disassemble(List<string> tokenList, int startInd, int endInd, List<Node> nodeList)
        {
            List<string> exprForDisassem = new List<string>();
            for (int i = startInd; i < endInd+1; i++)
            {
                exprForDisassem.Add(tokenList[i]);
            }


            while(exprForDisassem.Count!=1)
            {
                Node oper = new NodeOper("123");
                oper.parent = null;
                int ind;
                if (exprForDisassem.Contains("^"))
                {
                    oper.Val = "^";
                    ind = exprForDisassem.IndexOf("^");
                }
                else if (exprForDisassem.Contains("*") || exprForDisassem.Contains("/"))
                {
                    int i = exprForDisassem.IndexOf("*");
                    i =     i == -1 ? exprForDisassem.Count : i;

                    int j = exprForDisassem.IndexOf("/");
                    j =     j == -1 ? exprForDisassem.Count : j;


                    ind = i < j ? i : j;
                    oper.Val = i < j ? "*" : "/";
                }
                else if(exprForDisassem.Contains("+") || exprForDisassem.Contains("-"))
                {
                    int i = exprForDisassem.IndexOf("+");
                    i = i == -1 ? exprForDisassem.Count : i;

                    int j = exprForDisassem.IndexOf("-");
                    j = j == -1 ? exprForDisassem.Count : j;


                    ind = i < j ? i : j;
                    oper.Val = i < j ? "+" : "-";
                }
                else
                {
                    break;
                }




                if(ind==0)
                {
                    oper.children[0] = null;

                    if (exprForDisassem[ind + 1][0] >= '0' && exprForDisassem[ind + 1][0] <= '9' ||
                        exprForDisassem[ind + 1] == "pi" || exprForDisassem[ind + 1] == "e")
                        oper.children[1] = new NodeNum(exprForDisassem[ind + 1]);
                    else if (exprForDisassem[ind + 1] == "x")
                        oper.children[1] = new NodeX();
                    else
                    {
                        int nodeInd = int.Parse(exprForDisassem[ind + 1].Substring(1));
                        oper.children[1] = nodeList[nodeInd];
                        oper.children[1].parent = oper;

                        nodeList.RemoveAt(nodeInd);
                    }


                    nodeList.Add(oper);

                    exprForDisassem.RemoveAt(ind);
                    exprForDisassem.RemoveAt(ind);

                    exprForDisassem.Insert(0, "N"+(nodeList.Count-1).ToString());


                }
                else if(ind == exprForDisassem.Count-1)
                {
                    oper.children[1] = null;

                    if (exprForDisassem[ind - 1][0] >= '0' && exprForDisassem[ind - 1][0] <= '9' ||
                        exprForDisassem[ind - 1] == "pi" || exprForDisassem[ind - 1] == "e")
                        oper.children[0] = new NodeNum(exprForDisassem[ind - 1]);
                    else if (exprForDisassem[ind - 1] == "x")
                        oper.children[0] = new NodeX();
                    else
                    {
                        int nodeInd = int.Parse(exprForDisassem[ind - 1].Substring(1));
                        oper.children[0] = nodeList[nodeInd];
                        oper.children[0].parent = oper;

                        nodeList.RemoveAt(nodeInd);
                    }


                    nodeList.Add(oper);

                    exprForDisassem.RemoveAt(ind-1);
                    exprForDisassem.RemoveAt(ind-1);

                    exprForDisassem.Add("N" + (nodeList.Count - 1).ToString());
                }
                else
                {
                    int nodeInd1 = -1, nodeInd2 = -1;

                    if (exprForDisassem[ind - 1][0] >= '0' && exprForDisassem[ind - 1][0] <= '9' ||
                        exprForDisassem[ind - 1] == "pi" || exprForDisassem[ind - 1] == "e")
                        oper.children[0] = new NodeNum(exprForDisassem[ind - 1]);
                    else if (exprForDisassem[ind - 1] == "x")
                        oper.children[0] = new NodeX();
                    else
                    {
                        nodeInd1 = int.Parse(exprForDisassem[ind - 1].Substring(1));
                        oper.children[0] = nodeList[nodeInd1];
                        oper.children[0].parent = oper;

                    }



                    if (exprForDisassem[ind + 1][0] >= '0' && exprForDisassem[ind + 1][0] <= '9' ||
                        exprForDisassem[ind + 1] == "pi" || exprForDisassem[ind + 1] == "e")
                        oper.children[1] = new NodeNum(exprForDisassem[ind + 1]);
                    else if (exprForDisassem[ind + 1] == "x")
                        oper.children[1] = new NodeX();
                    else
                    {
                        nodeInd2 = int.Parse(exprForDisassem[ind + 1].Substring(1));
                        oper.children[1] = nodeList[nodeInd2];
                        oper.children[1].parent = oper;

                    }



                    if (nodeInd1 != -1 && nodeInd2 != -1)
                    {
                        if(nodeInd1 < nodeInd2)
                        {
                            int temp = nodeInd1;
                            nodeInd1 = nodeInd2;
                            nodeInd2 = temp;
                        }
                    }

                    if (nodeInd1!=-1)
                        nodeList.RemoveAt(nodeInd1);

                    if(nodeInd2!=-1)
                        nodeList.RemoveAt(nodeInd2);


                    nodeList.Add(oper);

                    exprForDisassem.RemoveAt(ind - 1);
                    exprForDisassem.RemoveAt(ind - 1);
                    exprForDisassem.RemoveAt(ind - 1);

                    exprForDisassem.Insert(ind-1, "N" + (nodeList.Count - 1).ToString());
                }
            }



            if (exprForDisassem[0][0] != 'N')
            {
                if (exprForDisassem[0] == "x")
                    nodeList.Add(new NodeX());
                else 
                    nodeList.Add(new NodeNum(exprForDisassem[0]));


                exprForDisassem.Clear();
                exprForDisassem.Add("N" + (nodeList.Count - 1).ToString());
            }


            for (int i = 0; i < endInd - startInd + 1; i++)
            {
                tokenList.RemoveAt(startInd);
            }

            tokenList.Insert(startInd, exprForDisassem[0]);
        }



        //
        private void tokenSplit(List<string> tokenList)
        {
            for (int i = 0; i < expr.Length; i++)
            {
                switch (expr[i])
                {
                    case '+':
                        tokenList.Add("+");
                        break;

                    case '-':
                        tokenList.Add("-");
                        break;

                    case '*':
                        tokenList.Add("*");
                        break;

                    case '/':
                        tokenList.Add("/");
                        break;

                    case '^':
                        tokenList.Add("^");
                        break;

                    case 'e':
                        tokenList.Add("e");
                        break;

                    case '(':
                        tokenList.Add("(");
                        break;

                    case ')':
                        tokenList.Add(")");
                        break;




                    default:

                        if (expr[i] == 'p' && expr[i + 1] == 'i')
                        {
                            tokenList.Add("pi");
                            i++;
                            break;
                        }

                        if (expr[i] == 'x')
                        {
                            tokenList.Add("x");
                            break;
                        }

                        if (expr[i] >= '0' && expr[i] <= '9')
                        {
                            int j = i;
                            while (j + 1 < expr.Length && (expr[j + 1] >= '0' && expr[j + 1] <= '9' || expr[j + 1] == ','))
                            {
                                j++;
                            }

                            tokenList.Add(expr.Substring(i, j - i + 1));
                            i = j;
                            break;
                        }

                        foreach (var item in baseFuncs)
                        {
                            if (i + item.Length - 1 < expr.Length && item == expr.Substring(i, item.Length))
                            {
                                tokenList.Add(item);
                                i += item.Length - 1;
                                break;
                            }
                        }

                        break;
                }
            }
        }




        //
        public double Calc(double x)
        {
            return root.Calc(x);
        }




        //
        private void CheckFunc(string Expression)
        {
            try
            {
            
                int countOpenBracket = 0, countCloseBracket = 0;
                for (int i = 0; i < Expression.Length; i++)
                {
                    i = Expression.IndexOf('(', i);
                    if (i != -1)
                        countOpenBracket++;
                    else
                        break;
                }

                for (int i = 0; i < Expression.Length; i++)
                {
                    i = Expression.IndexOf(')', i);
                    if (i != -1)
                        countCloseBracket++;
                    else
                        break;
                }

                if (countCloseBracket != countOpenBracket)
                {
                    throw new Exception("Не хватает скобок");
                }



                foreach (var item in MathFuncParser.baseFuncs)
                {
                    int i = Expression.IndexOf(item);
                    while (i != -1)
                    {
                        if (Expression[i + item.Length] != '(')
                        {
                            throw new Exception("Нет открывающейся скобоки после " + item) ;
                        }

                        i = Expression.IndexOf(item, i + 1);
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }

        }








/// <summary>
/// Node classes of tree
/// </summary>


        abstract class Node
        {
            public string Val;
            public Node parent;
            public Node[] children;

            public Node(int N)
            {
                if (N != 0)
                    children = new Node[N];
                else
                    children = null;
            }

            public abstract double Calc(double x);
        }



        class NodeOper: Node
        {
            public NodeOper(string val)
                : base(2)
            {
                Val = val;
            }

            public override double Calc(double x)
            {
                double result = 0;
                double a = 0, b = 0;

                try
                {
                    switch(Val)
                    {
                        case "+":
                            a = children[0] == null ? 0: children[0].Calc(x);
                            b = children[1] == null ? 0: children[1].Calc(x);
                            result = a + b;
                            break;

                        case "-":
                            a = children[0] == null ? 0 : children[0].Calc(x);
                            b = children[1] == null ? 0 : children[1].Calc(x);
                            result = a - b;
                            break;

                        case "*":
                            a = children[0] == null ? 1 : children[0].Calc(x);
                            b = children[1] == null ? 1 : children[1].Calc(x);
                            result = a * b;
                            break;

                        case "/":
                            a = children[0] == null ? 1 : children[0].Calc(x);
                            b = children[1] == null ? 1 : children[1].Calc(x);
                            result = a / b;
                            break;

                        case "^":
                            a = children[0] == null ? 1 : children[0].Calc(x);
                            b = children[1] == null ? 1 : children[1].Calc(x);
                            result = Math.Pow(a, b);
                            break;
                    }

                }
                catch (Exception)
                {

                    throw;
                }

                return result;
            }
        }

        class NodeFunc : Node
        {
            public NodeFunc(string val)
                : base(1)
            {
                Val = val;
            }

            public override double Calc(double x)
            {
                double res = 0;

                try
                {

                   switch(Val)
                    {
                        case "log":
                            res = Math.Log(children[0].Calc(x));
                            break;

                        case "sin":
                            res = Math.Sin(children[0].Calc(x));
                            break;

                        case "cos":
                            res = Math.Cos(children[0].Calc(x));
                            break;

                        case "tg":
                            res = Math.Tan(children[0].Calc(x));
                            break;

                        case "ctg":
                            res = 1/Math.Tan(children[0].Calc(x));
                            break;
                    }

                }
                catch (Exception)
                {

                    throw;
                }

                return res;
            }
        }

        class NodeNum : Node
        {
            public NodeNum(string val)
                : base(0)
            {
                Val = val;
            }


            public override double Calc(double x)
            {
                double res = 0;
                if (Val[0] < '0' && Val[0] > '9')
                {
                    if (Val[0] == 'e')
                        res = Math.E;
                    else
                        res = Math.PI;
                }
                else
                    res = double.Parse(Val);


                return res;
            }
        }

        class NodeX : Node
        {
            public NodeX()
                : base(0)
            {
                Val = "x";
            }

            public override double Calc(double x)
            {
                return x;
            }
        }






    }



}
