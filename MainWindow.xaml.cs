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

using Viewmodel;
using keyCMD = KeyCommand.KeyCommand;
using System.Runtime.Serialization.DataContracts;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Calculator
{

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //public string displyText { get; set; }
        public static string Formula;
        delegate void UpdateDisplayDelegate(string text);
        UpdateDisplayDelegate updateDisplay ;
        public ButtonViewModel ButtonViewModel { get; } = new ButtonViewModel();

        public MainWindow()
        {
            InitializeComponent();
            Title = "小算盤"; // 設定視窗標題
            
            //keyCMD.OperationRegistry registry = new keyCMD.OperationRegistry();
            //registry.RegisterOperation(keyCMD.OperationType.Equal, new keyCMD.EqualOperation());
            //registry.RegisterOperation(keyCMD.OperationType.AC, new keyCMD.ACOperation());
            //registry.RegisterOperation(keyCMD.OperationType.Back, new keyCMD.BackOperation());
            //registry.RegisterOperation(keyCMD.OperationType.Percentage, new keyCMD.PercentageOperation());
            //Keyevent();
        }
        private void Keyevent()
        {
            
            Formula = "";
            keyCMD.OperationType Operation;
            TextBlock Display = new TextBlock();
            
            ButtonEquals.Click += (s, e) =>
            {
                Operation = keyCMD.OperationType.Equal;
                //Formula = ResultDisplay.Text;         
                Display.Text = ButtonViewModel.DisplayText;
                //keyCMD.OperationRegistry.GetOperation(Operation).Execute(Display);

            };
            //ButtonClear.Click += (s, e) =>
            //{
            //    Operation = keyCMD.OperationType.AC;
            //    ButtonViewModel.DisplayText = "";
            //    Display.Text = ButtonViewModel.DisplayText;
            //    keyCMD.OperationRegistry.GetOperation(Operation).Execute(Display);
            //};
            //ButtonBackspace.Click += (s, e) =>
            //{
            //    Operation = keyCMD.OperationType.Back;
            //    Display.Text = ButtonViewModel.DisplayText;
            //    keyCMD.OperationRegistry.GetOperation(Operation).Execute(Display);
            //};
            //ButtonPercent.Click += (s, e) =>
            //{
            //    Operation = keyCMD.OperationType.Percentage;
            //    Display.Text = ButtonViewModel.DisplayText;
            //    keyCMD.OperationRegistry.GetOperation(Operation).Execute(Display);
            //};
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
