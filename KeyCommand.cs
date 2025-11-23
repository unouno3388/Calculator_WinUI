using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace KeyCommand
{
    
    public class KeyCommand 
    {
        public enum OperationType
        {
            Equal,
            AC,
            Back,
            Percentage
        }
        public interface IOperation
        {
            //void Execute();
            void Execute(FrameworkElement element);
        }
        public class OperationRegistry
        {
            //private static readonly Dictionary<string, IOperation> operations = new() { };
            //public void RegisterOperation(string name, IOperation operation)
            //{
            //    operations[name] = operation;
            //}
            //public void UnregisterOperation(string name)
            //{
            //    operations.Remove(name);
            //}
            private static readonly Dictionary<OperationType, IOperation> operations = new() { };

            public void RegisterOperation(OperationType type, IOperation operation)
            {
                operations[type] = operation;
            }

            public static IOperation GetOperation(OperationType operation)
            {
                IOperation op;
                if (operations.TryGetValue(operation, out op))
                {
                    return op;
                }
                throw new NotImplementedException();
            }
        }
        public class EqualOperation : IOperation
        {

            public void Execute(FrameworkElement element)
            {
                //EvaluateRPN(TokenizeExpression(op));
                if (element is TextBlock textBlock)
                {
                    try
                    {
                        textBlock.Text += " = " + EvaluateRPN(ShuntingYard.ConvertToPostfix(TokenizeExpression(textBlock.Text))).ToString("0.#########");
                    }
                    catch (Exception ex)
                    {
                        textBlock.Text = "Error";
                        //MessageBox.Show(ex.ToString());
                    }
                }
            }
            public static List<string> TokenizeExpression(string expression)
            {
                List<string> tokens = new List<string>();
                string currentNumber = "";

                // 運算子集合
                char[] operators = new char[] { '+', '-', '*', '/' };

                foreach (char c in expression)
                {
                    if (char.IsDigit(c) || c == '.') // 處理數字和小數點
                    {
                        currentNumber += c;
                    }
                    else if (operators.Contains(c)) // 處理運算子
                    {
                        // 如果前面有累積的數字，先將數字Token推入列表
                        if (!string.IsNullOrEmpty(currentNumber))
                        {
                            tokens.Add(currentNumber);
                            currentNumber = "";
                        }
                        // 將運算子Token推入列表
                        tokens.Add(c.ToString());
                    }
                    // 如果加入括號或其他符號，需在此處擴展處理邏輯
                }

                // 迴圈結束後，將最後一個累積的數字推入
                if (!string.IsNullOrEmpty(currentNumber))
                {
                    tokens.Add(currentNumber);
                }

                return tokens;
            }
            // 中序轉後序的算法
            public class ShuntingYard
            {
                // 定義運算子的優先級
                private static readonly Dictionary<string, int> Precedence = new Dictionary<string, int>
                {
                    { "+", 2 }, { "-", 2 },
                    { "*", 3 }, { "/", 3 }
                    // 如果有括號，"(", ")" 也要加入，但邏輯更複雜
                };

                // 判斷是否為運算子
                private static bool IsOperator(string token)
                {
                    return Precedence.ContainsKey(token);
                }

                // 獲取棧頂運算子
                private static string Peek(Stack<string> stack)
                {
                    return stack.Count > 0 ? stack.Peek() : null;
                }

                // 中綴 (Tokens) 轉換為後綴 (Tokens)
                public static List<string> ConvertToPostfix(List<string> infixTokens)
                {
                    Stack<string> operatorStack = new Stack<string>(); // 運算子堆疊
                    List<string> outputList = new List<string>();       // 輸出列表 (RPN)

                    // 算法主體
                    foreach (var token in infixTokens)
                    {
                        if (double.TryParse(token, out _))
                        {
                            // 1. 如果是數字 (運算元)：直接放入輸出列表
                            outputList.Add(token);
                        }
                        else if (IsOperator(token))
                        {
                            // 2. 如果是運算子：
                            while (Peek(operatorStack) != null &&
                                   IsOperator(Peek(operatorStack)) &&
                                   Precedence[token] <= Precedence[Peek(operatorStack)])
                            {
                                // 只要堆疊頂部運算子的優先級 >= 當前運算子
                                // 就將堆疊頂部運算子彈出，放入輸出列表
                                outputList.Add(operatorStack.Pop());
                            }
                            // 將當前運算子壓入堆疊
                            operatorStack.Push(token);
                        }
                        // 由於您的範例沒有括號，我們省略括號處理邏輯
                    }

                    // 3. 處理完所有 Token 後，將運算子堆疊中剩餘的運算子全部彈出並放入輸出列表
                    while (operatorStack.Count > 0)
                    {
                        outputList.Add(operatorStack.Pop());
                    }

                    return outputList;
                }
            }
            // 假設我們已經完成了中綴轉後綴的步驟，得到了 Token 列表。
            // (注意: 實際的後綴表達式 Token 應該是數字和運算子分開的)
            public static double EvaluateRPN(List<string> tokens)
            {
                // 運算元堆疊：用來存放數字
                Stack<double> operandStack = new Stack<double>();

                foreach (var token in tokens)
                {
                    if (double.TryParse(token, out double number))
                    {
                        // 1. 如果是數字，推入堆疊
                        operandStack.Push(number);
                    }
                    else if (IsOperator(token))
                    {
                        // 2. 如果是運算子 (+, -, *, /)
                        if (operandStack.Count < 1)
                        {
                            throw new InvalidOperationException("後綴表達式無效：運算元不足。");
                        }

                        // 彈出右運算元 (b) 和左運算元 (a)
                        double operandB = operandStack.Pop();
                        double operandA = operandStack.Pop();

                        // 執行運算並將結果推回堆疊
                        double result = PerformOperation(operandA, operandB, token);
                        operandStack.Push(result);
                    }
                    else
                    {
                        throw new ArgumentException($"未知的 Token: {token}");
                    }
                }

                // 3. 遍歷完成後，堆疊中剩下的唯一元素就是最終結果
                if (operandStack.Count != 1)
                {
                    throw new InvalidOperationException("後綴表達式無效：計算結束後堆疊中有多個結果。");
                }

                return operandStack.Pop();
            }

            private static bool IsOperator(string token)
            {
                return token == "+" || token == "-" || token == "*" || token == "/";
            }

            private static double PerformOperation(double a, double b, string op)
            {
                switch (op)
                {
                    case "+": return a + b;
                    case "-": return a - b;
                    case "*": return a * b;
                    case "/":
                        if (b == 0) throw new DivideByZeroException();
                        return a / b;
                    default: return 0; // 不會到達此處
                }
            }
        }
        public class ACOperation : IOperation
        {
            public void Execute(FrameworkElement element)
            {
                if (element is TextBlock textBlock)
                {
                    textBlock.Text = "";

                }
                // Implementation for AC operation
            }
        }

        public class BackOperation : IOperation
        {
            public void Execute(FrameworkElement element)
            {
                if (element is TextBlock textBlock)
                {
                    if (textBlock.Text.Length > 0)
                    {
                        textBlock.Text = textBlock.Text[..^1];
                    }
                }
                // Implementation for Back operation
            }
        }

        public class PercentageOperation : IOperation
        {
            public void Execute(FrameworkElement element) 
            {
                char[] operators = new char[] { '+', '-', '*', '/' };
                if (element is TextBlock textBox)
                {
                    string text = textBox.Text;
                    if (text.Length == 0) return;
                    // 找到最後一個運算子的位置
                    int lastOperatorIndex = -1;
                    for (int i = text.Length - 1; i >= 0; i--)
                    {
                        if (operators.Contains(text[i]))
                        {
                            lastOperatorIndex = i;
                            break;
                        }
                    }
                    // 如果找到了運算子，計算百分比
                    if (lastOperatorIndex != -1 && lastOperatorIndex < text.Length - 1)
                    {
                        string numberStr = text[(lastOperatorIndex + 1)..];
                        if (double.TryParse(numberStr, out double number))
                        {
                            double percentageValue = number / 100.0;
                            textBox.Text = text[..(lastOperatorIndex + 1)] + percentageValue.ToString();
                        }
                    }
                    else    // 如果沒有運算子，則對整個數字計算百分比
                    {     
                        if (double.TryParse(text, out double number))
                        {
                            double percentageValue = number / 100.0;
                            textBox.Text = percentageValue.ToString();
                        }
                    }
                }
            }
        }
    }
}