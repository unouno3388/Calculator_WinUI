using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel; // 确保引用了 ViewModel 命名空间
namespace Calculator
{
    public sealed partial class ProgrammerCalculator : Page
    {

        public ProgrammerViewModel ViewModel { get; set; }
        public ProgrammerCalculator()
        {

            this.InitializeComponent();

            // 实例化 ViewModel，并设置给 Page 的公共属性
            this.ViewModel = new ProgrammerViewModel();
            
            this.DataContext = this.ViewModel;
        }
        public bool IsHex(int currentModeInt)
        {
            // 假设 ProgrammerViewModel.BitDisplayMode.Hexadecimal 的值为 0
            Debug.WriteLine($"IsHex check for mode int: {currentModeInt}");
            const int HEX_MODE_VALUE = 16;
            return currentModeInt == HEX_MODE_VALUE;
        }
        public bool IsModeChecked(int valueType, string targetTag)
        {
            if (int.TryParse(targetTag, out int tagValue))
            {
                return valueType == tagValue;
            }
            return false;
        }
        private void ModeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb)
            {
                if (rb.Tag is string tagString && int.TryParse(tagString, out int tagValue))
                {
                    // 只有当 RadioButton 被选中时才更新 ViewModel
                    if (rb.IsChecked == true)
                    {
                        // 将 Tag 的整数值设置给 ViewModel 的 int 属性
                        this.ViewModel.CurrentBitDisplayModeInt = tagValue;
                    }
                }
            }
        }
    }
}
