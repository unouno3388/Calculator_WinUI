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
using keyCMD = KeyCommand.KeyCommand;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Calculator
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Title = "小算盤"; // 設定視窗標題

            keyCMD.OperationRegistry registry = new keyCMD.OperationRegistry();
            registry.RegisterOperation(keyCMD.OperationType.Equal, new keyCMD.EqualOperation());
            registry.RegisterOperation(keyCMD.OperationType.AC, new keyCMD.ACOperation());
            registry.RegisterOperation(keyCMD.OperationType.Back, new keyCMD.BackOperation());
            registry.RegisterOperation(keyCMD.OperationType.Percentage, new keyCMD.PercentageOperation());
            Keyevent();
        }
        private void Keyevent()
        {
            string Formula = "";
            keyCMD.OperationType Operation;
            Button1.Click += (s, e) => { ResultDisplay.Text += "1"; };
            Button2.Click += (s, e) => { ResultDisplay.Text += "2"; };
            Button3.Click += (s, e) => { ResultDisplay.Text += "3"; };
            Button4.Click += (s, e) => { ResultDisplay.Text += "4"; };
            Button5.Click += (s, e) => { ResultDisplay.Text += "5"; };
            Button6.Click += (s, e) => { ResultDisplay.Text += "6"; };
            Button7.Click += (s, e) => { ResultDisplay.Text += "7"; };
            Button8.Click += (s, e) => { ResultDisplay.Text += "8"; };
            Button9.Click += (s, e) => { ResultDisplay.Text += "9"; };
            Button0.Click += (s, e) => { ResultDisplay.Text += "0"; };
            ButtonPlus.Click += (s, e) => { ResultDisplay.Text += "+"; };
            ButtonMinus.Click += (s, e) => { ResultDisplay.Text += "-"; };
            ButtonMultiply.Click += (s, e) => { ResultDisplay.Text += "*"; };
            ButtonDivide.Click += (s, e) => { ResultDisplay.Text += "/"; };
            ButtonDecimal.Click += (s, e) => { ResultDisplay.Text += "."; };
            ButtonEquals.Click += (s, e) =>
            {
                Operation = keyCMD.OperationType.Equal;
                Formula = ResultDisplay.Text;
                keyCMD.OperationRegistry.GetOperation(Operation).Execute(ResultDisplay);

            };
            ButtonClear.Click += (s, e) =>
            {
                Operation = keyCMD.OperationType.AC;
                Formula = "";
                keyCMD.OperationRegistry.GetOperation(Operation).Execute(ResultDisplay);
            };
            ButtonBackspace.Click += (s, e) =>
            {
                Operation = keyCMD.OperationType.Back;
                keyCMD.OperationRegistry.GetOperation(Operation).Execute(ResultDisplay);
            };
            ButtonPercent.Click += (s, e) =>
            {
                Operation = keyCMD.OperationType.Percentage;
                keyCMD.OperationRegistry.GetOperation(Operation).Execute(ResultDisplay);
            };
            //KeyDown += (s, e) =>
            //{
            //    if (e.KeyCode == Keys.NumPad1) { }
            //    switch (e.KeyCode)
            //    {
            //        case Keys.Enter: buttonEq.PerformClick(); break;
            //        case Keys.Escape: buttonAC.PerformClick(); break;
            //    }
            //};
        }
    }
}
