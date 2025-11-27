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

using ViewModel;
//using keyCMD = KeyCommand.KeyCommand;
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
        public StanderViewModel ButtonViewModel { get; } = new StanderViewModel();

        public MainWindow()
        {
            InitializeComponent();
            Title = "小算盤"; // 設定視窗標題

            // 處理程式啟動時的初始化
            NavView.Loaded += (s, e) =>
            {
                // 取得綁定在介面上的 ViewModel
                if (RootGrid.DataContext is StanderViewModel vm && vm.MenuItems.Count > 0)
                {
                    // 1. 設定選單選中第一項 (標準型)
                    var firstItem = vm.MenuItems[0];
                    NavView.SelectedItem = firstItem;

                    // 2. 直接呼叫導航邏輯 (不需要偽造事件參數)
                    NavigateToView(firstItem);
                }
            };

        }
        // 當使用者點擊選單時觸發
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // 從事件參數中取得被選中的項目
            if (args.SelectedItem is CalculatorMenuItem selectedItem)
            {
                NavigateToView(selectedItem);
            }
        }

        // 🔥 核心修改：將導航邏輯獨立出來，讓大家都能呼叫
        private void NavigateToView(CalculatorMenuItem item)
        {
            if (item == null) return;

            switch (item.Type)
            {
                case "Standard":
                    // 導航到標準計算機
                    // 檢查是否重複導航，避免無謂的重新載入
                    if (ContentFrame.CurrentSourcePageType != typeof(StandardCalculator))
                    {
                        ContentFrame.Navigate(typeof(StandardCalculator));
                    }
                    break;

                case "Programmer":
                    // 導航到程式設計師計算機
                    if (ContentFrame.CurrentSourcePageType != typeof(ProgrammerCalculator))
                    {
                        ContentFrame.Navigate(typeof(ProgrammerCalculator));
                    }
                    break;
            }

            // 更新 ViewModel 的標題 (左上角文字)
            if (RootGrid.DataContext is StanderViewModel vm)
            {
                vm.CurrentCalculatorType = item.Name;
            }
        }
    }
}
