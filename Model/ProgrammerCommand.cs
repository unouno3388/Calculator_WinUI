using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Model
{ 
   public class ProgrammerCommand 
   {

        /// <summary>
        /// 代表一個用於「程式設計師（Programmer）」模式的運算式處理器，繼承自 <see cref="ExpressionProcessor"/>。
        /// 
        /// 功能摘要：
        /// - 擴充數字解析邏輯：在 <see cref="NumberBase.Hexadecimal"/> 時能解析十六進位字串（包含 A-F），否則使用十進位解析。
        /// - 提供後綴（RPN）運算評估：<see cref="EvaluateRPN(System.Collections.Generic.List{string})"/> 以 <c>int</c> 為運算與回傳型別，處理基本算術與位移運算（含 <c>LSH</c>）。
        /// - 處理錯誤情況並拋出適當例外（例如運算元不足或除以零）。
        /// - 採用單例模式（靜態 <c>instance</c>）以維持共用狀態（例如預設的 <see cref="CurrentBase"/> 為十六進位）。
        /// 
        /// 使用情境：此類別主要供 UI 或命令邏輯在程式員模式下解析與計算運算式時使用，可直接存取單例來執行字串 token 化、轉為後綴表示法與評估結果。
        /// </summary>
        sealed class  ProgrammerExpressionProcessor : ExpressionProcessor
        {
            // 可以在此處擴展或覆寫 ExpressionProcessor 的方法       
            private ProgrammerExpressionProcessor() 
            {
                CurrentBase = NumberBase.Hexadecimal;               
            }
            private static ProgrammerExpressionProcessor _instance;
            public static ProgrammerExpressionProcessor instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new ProgrammerExpressionProcessor();
                    }
                    return _instance;
                }
            }
        }
        //static ProgrammerExpressionProcessor processor = new ProgrammerExpressionProcessor();
        public class EqualOperation : IOperation
        {

            public string Execute(string op)
            {
                //EvaluateRPN(TokenizeExpression(op));
                //if (element is TextBlock textBlock)
                //{

                //ProgrammerExpressionProcessor pm;
                try
                {
                    op = ProgrammerExpressionProcessor.instance.EvaluateRPN(
                         ProgrammerExpressionProcessor.instance.ConvertToPostfix(
                         ProgrammerExpressionProcessor.instance.TokenizeExpression(op))).ToString("0.#########");
                    //Debug.WriteLine("Result: " + processor.TokenizeExpression(op));
                    //processor.ConvertToPostfix(processor.TokenizeExpression(op)).ForEach(t => op+=t);
                    //op = 

                }
                catch (Exception ex)
                {
                    op = "Error";
                    //MessageBox.Show(ex.ToString());
                }
                return op;
            }
            
        }
        public class ACOperation : IOperation
        {
            public string Execute(string op)
            {
                return op = "";
            }
        }

        public class BackOperation : IOperation
        {
            public string Execute(string op)
            {
                try
                {
                    op = op[..^1];
                }
                catch
                {
                    op = "";
                }
                return op;
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

            public string Execute(string op)
            {
                char[] operators = new char[] { '+', '-', '*', '/' };

                string text = op;
                if (text.Length == 0) return "";
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
                        op = text[..(lastOperatorIndex + 1)] + percentageValue.ToString();
                    }
                }
                else    // 如果沒有運算子，則對整個數字計算百分比
                {
                    if (double.TryParse(text, out double number))
                    {
                        double percentageValue = number / 100.0;
                        op = percentageValue.ToString();
                    }
                }
                return op;
            }
        }

    }
}