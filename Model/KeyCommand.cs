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
using System.Windows.Input;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;

namespace Model
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
        //void Execute(FrameworkElement element);
        string Execute(string op);
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
    public abstract class ExpressionProcessor
    {
        public enum NumberBase
        {
            Decimal = 10,
            Hexadecimal = 16
        }
        public NumberBase CurrentBase { get; set; } = NumberBase.Decimal;
        // 將表達式字串切割成 Token 列表
        /// <summary>
        /// 將表達式字串切割成 Token 列表
        /// </summary>
        /// <remarks>This method does not validate the syntax of the expression and only supports
        /// basic operators ('+', '-', '*', '/'). Parentheses and other symbols are not tokenized and will be
        /// ignored. To handle additional symbols, extend the tokenization logic as needed.</remarks>
        /// <param name="expression">The expression to tokenize. The string should contain numbers and supported operators ('+', '-', '*',
        /// '/').</param>
        /// <returns>A list of strings, where each string is a token representing either a number or an operator from the
        /// input expression. The list will be empty if the input is null or empty.</returns>
        /// 
        public virtual List<string> TokenizeExpression(string expression)
        {
            List<string> tokens = new List<string>();

            // 定義單字型運算子
            var wordOperators = new HashSet<string> { "LSH", "RSH" };
            // 定義符號型運算子
            var symbolOperators = new HashSet<char> { '+', '-', '*', '/', '(', ')' };

            int i = 0;
            while (i < expression.Length)
            {
                char c = expression[i];

                // 1. 忽略空白
                if (char.IsWhiteSpace(c))
                {
                    i++;
                    continue;
                }

                // --- 修正重點開始 ---
                // 判斷是否為「有效數字字元」
                // 如果是 10 進位：0-9 和 .
                // 如果是 16 進位：0-9 和 A-F (通常 16 進位不支援小數點)
                bool isHexDigit = (c >= '0' && c <= '9') ||
                                  (c >= 'A' && c <= 'F') ||
                                  (c >= 'a' && c <= 'f');

                // 如果當前模式是 Hex，且遇到 A-F，視為數字
                // 或者原本就是 0-9 或小數點
                if ((CurrentBase == NumberBase.Hexadecimal && isHexDigit) ||
                    (char.IsDigit(c) || c == '.'))
                {
                    string number = "";
                    while (i < expression.Length)
                    {
                        char nextC = expression[i];
                        bool isNextHex = (nextC >= '0' && nextC <= '9') ||
                                         (nextC >= 'A' && nextC <= 'F') ||
                                         (nextC >= 'a' && nextC <= 'f');

                        // 讀取條件：
                        // 1. 如果是 Hex 模式：只要是 Hex 字元就讀
                        // 2. 如果是 Dec 模式：只要是 數字 或 小數點 就讀
                        if (CurrentBase == NumberBase.Hexadecimal && isNextHex)
                        {
                            number += nextC;
                            i++;
                        }
                        else if (CurrentBase == NumberBase.Decimal && (char.IsDigit(nextC) || nextC == '.'))
                        {
                            number += nextC;
                            i++;
                        }
                        else
                        {
                            break; // 遇到運算子或非法字元，停止讀取
                        }
                    }
                    tokens.Add(number.ToUpper()); // 轉大寫方便統一處理
                }
                // --- 修正重點結束 ---
                // 3. 處理文字 (LSH, RHS)
                else if (char.IsLetter(c))
                {
                    string word = "";
                    // 讀取直到非字母為止
                    while (i < expression.Length && char.IsLetter(expression[i]))
                    {
                        word += expression[i];
                        i++;
                    }

                    // 轉大寫以防使用者輸入小寫 (例如 lsh -> LSH)
                    word = word.ToUpper();

                    if (wordOperators.Contains(word))
                    {
                        tokens.Add(word);
                    }
                    else
                    {
                        Debug.WriteLine("Unknown word operator: " + word);
                        throw new ArgumentException($"未知的運算子或變數: {word}");
                    }
                }

                // 4. 處理符號運算子 (+, -, *, /, (, ))
                else if (symbolOperators.Contains(c))
                {
                    // --- 負號判斷邏輯 (同上一版) ---
                    if (c == '-')
                    {
                        // 如果是第一個 Token，或者前一個 Token 是運算子/左括號
                        bool isUnary = tokens.Count == 0 || IsOperator(tokens.Last());
                        if (isUnary)
                        {
                            // 這裡有點不同：因為我們沒有 currentNumber 緩存了
                            // 我們需要手動往後讀取數字部分，將其組合成 "-5" 這樣的 Token

                            i++; // 跳過 '-'
                            string negativeNum = "-";

                            // 繼續讀取後面的數字
                            if (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                            {
                                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                                {
                                    negativeNum += expression[i];
                                    i++;
                                }
                                tokens.Add(negativeNum);
                            }
                            else
                            {
                                // 如果負號後面不是數字 (例如 -(...))，那它可能是代表 -1 * ...
                                // 但為了簡化，如果是 -(...) 這種情況，建議視作 RPN 裡的 "NEG" (一元運算子)
                                // 這裡我們先假設它是負數的一部分，如果後面沒數字就報錯或視為單獨的 '-'
                                tokens.Add("-");
                            }
                            continue; // 跳過最外層的 i++ (因為裡面已經加過了)
                        }
                    }

                    tokens.Add(c.ToString());
                    i++;
                }
                else
                {
                    throw new ArgumentException($"無法識別的字元: {c}");
                }
            }

            Debug.WriteLine("Result: " + string.Join(" ", tokens));

            return tokens;
        }

        // 中序轉後序的算法

        #region 中序轉後序的算法
        // 定義運算子的優先級
        private static readonly Dictionary<string, int> Precedence = new Dictionary<string, int>
            {
                { "+", 2 }, { "-", 2 },
                { "*", 3 }, { "/", 3 },
                {"LSH", 4},{ "RSH", 4}
                // 如果有括號，"(", ")" 也要加入，但邏輯更複雜
            };

        // 判斷是否為運算子
        public static bool IsOperator(string token)
        {
            return Precedence.ContainsKey(token);
        }

        // 獲取棧頂運算子
        public static string Peek(Stack<string> stack)
        {
            return stack.Count > 0 ? stack.Peek() : null;
        }
        private int GetPrecedence(string op)
        {
            if (Precedence.TryGetValue(op, out int value))
            {
                return value;
            }
            throw new ArgumentException($"未定義優先級的運算子: {op}");
        }

        // 中綴 (Tokens) 轉換為後綴 (Tokens)

        // 統一解析數字的方法
        public bool TryParseNumber(string token, out double result)
        {
            // 如果是 16 進位
            if (CurrentBase == NumberBase.Hexadecimal)
            {
                // 16 進位通常是整數運算
                try
                {
                    // 使用 Convert.ToInt64 才能解析 A-F
                    long hexVal = Convert.ToInt64(token, 16);
                    result = (double)hexVal; // 轉回 double 以便相容你現有的 Stack<double>
                    return true;
                }
                catch
                {
                    result = 0;
                    return false;
                }
            }
            else
            {
                // 原本的 10 進位處理
                return double.TryParse(token, out result);
            }
        }
        public virtual List<string> ConvertToPostfix(List<string> infixTokens)
        {
            Stack<string> operatorStack = new Stack<string>();
            List<string> outputList = new List<string>();

            foreach (var token in infixTokens)
            {
                // 1. 如果是數字：直接輸出
                if (TryParseNumber(token, out _))
                //if (double.TryParse(token, out _)  )
                {
                    outputList.Add(token);
                }
                // 2. 如果是左括號：直接入棧
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                // 3. 如果是右括號：彈出直到遇到左括號
                else if (token == ")")
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                    {
                        outputList.Add(operatorStack.Pop());
                    }

                    // 如果堆疊空了還沒找到左括號，代表括號不匹配
                    if (operatorStack.Count == 0)
                        throw new ArgumentException("括號不匹配");

                    operatorStack.Pop(); // 彈出並丟棄左括號 "("
                }
                // 4. 如果是運算子 (+, -, *, /, LSH, RHS)
                else if (IsOperator(token))
                {
                    // 當堆疊頂部運算子優先級 >= 當前運算子，且不是左括號時，彈出
                    while (operatorStack.Count > 0 &&
                           operatorStack.Peek() != "(" &&
                           GetPrecedence(token) <= GetPrecedence(operatorStack.Peek()))
                    {
                        outputList.Add(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else
                {
                    // 如果遇到既不是數字也不是已知運算子的東西
                    throw new ArgumentException($"未知的 Token: {token}");
                }
            }

            // 5. 將剩餘運算子彈出
            while (operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                if (op == "(") throw new ArgumentException("括號不匹配");
                outputList.Add(op);
            }
            Debug.WriteLine("Postfix Result: " + string.Join(" ", outputList));
            return outputList;
        }
        #endregion

        public double EvaluateRPN(List<string> tokens)
        {
            // 運算元堆疊：用來存放數字
            Stack<double> operandStack = new Stack<double>();

            foreach (var token in tokens)
            {
                if (TryParseNumber(token, out double number))
                //if (double.TryParse(token, out double number))
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

        public double PerformOperation(double a, double b, string op)
        {
            switch (op)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    if (b == 0) throw new DivideByZeroException();
                    return a / b;
                case "LSH": return (int)a << (int)b;
                case "RSH": return (int)a >> (int)b;
                default: return 0; // 不會到達此處
            }
        }
    }
    public class KeyCommand 
    {
        //public enum OperationType
        //{
        //    Equal,
        //    AC,
        //    Back,
        //    Percentage
        //}
        //public interface IOperation
        //{
        //    //void Execute();
        //    //void Execute(FrameworkElement element);
        //    string Execute(string op);
        //}
        
        //public class OperationRegistry
        //{
        //    //private static readonly Dictionary<string, IOperation> operations = new() { };
        //    //public void RegisterOperation(string name, IOperation operation)
        //    //{
        //    //    operations[name] = operation;
        //    //}
        //    //public void UnregisterOperation(string name)
        //    //{
        //    //    operations.Remove(name);
        //    //}
        //    private static readonly Dictionary<OperationType, IOperation> operations = new() { };

        //    public void RegisterOperation(OperationType type, IOperation operation)
        //    {
        //        operations[type] = operation;
        //    }

        //    public static IOperation GetOperation(OperationType operation)
        //    {
        //        IOperation op;
        //        if (operations.TryGetValue(operation, out op))
        //        {
        //            return op;
        //        }
        //        throw new NotImplementedException();
        //    }
        //}
        public class EqualOperation : IOperation
        {

            public string Execute(string op)
            {
                //EvaluateRPN(TokenizeExpression(op));
                //if (element is TextBlock textBlock)
                //{
                    try
                    {
                        op += " = " + EvaluateRPN(ShuntingYard.ConvertToPostfix(TokenizeExpression(op))).ToString("0.#########");
                    }
                    catch (Exception ex)
                    {
                        op = "Error";
                        //MessageBox.Show(ex.ToString());
                    }
                return op;
                //}
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

