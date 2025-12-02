using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;

using Model ;
namespace ViewModel
{
    public class ProgrammerViewModel : ViewModelBase
    {

        
        // 新增用於選單的屬性
        private string _currentCalculatorType = "程式設計師模式";
        public string CurrentCalculatorType
        {
            get { return _currentCalculatorType; }
            set { SetProperty<string>(ref _currentCalculatorType, value); }
        }

        //private bool isHexMode;
        //public bool IsHexMode
        //{
        //    get { return isHexMode; }
        //    set { SetProperty<bool>(ref isHexMode, value); }
        //}

        //private bool isDecMode = true;
        //public bool IsDecMode
        //{
        //    get { return isDecMode; }
        //    set { SetProperty<bool>(ref isDecMode, value); }
        //}
        //private bool isOctMode;
        //public bool IsOctMode
        //{
        //    get { return isOctMode; }
        //    set { SetProperty<bool>(ref isOctMode, value); }
        //}
        //private bool isBinMode;
        //public bool IsBinMode
        //{
        //    get { return isBinMode; }
        //    set { SetProperty<bool>(ref isBinMode, value); }
        //}

        public enum BitDisplayMode 
        {
           Hexadecimal = 16,
           Decimal = 10,
           Octal = 8,
           Binary = 2,
        }
        // 修改為私有欄位，並添加公開屬性 (使用 SetProperty)
        private BitDisplayMode _bitDisplayMode = BitDisplayMode.Decimal;
        public BitDisplayMode CurrentBitDisplayMode // 新的公開屬性名稱
        {
            get { return _bitDisplayMode; }
            set { SetProperty<BitDisplayMode>(ref _bitDisplayMode, value); }
        }

        public int CurrentBitDisplayModeInt
        {
            get => (int)_bitDisplayMode; // 返回枚举的整数值
            set
            {
                // 当 XAML 设置新的整数值时，将其转换回枚举
                Debug.WriteLine($"Setting CurrentBitDisplayModeInt to {value}");
                BitDisplayMode newMode = (BitDisplayMode)value;
                SetProperty<BitDisplayMode>(ref _bitDisplayMode, newMode);
                if (_bitDisplayMode != newMode)
                {
                    _bitDisplayMode = newMode;
                    // 通知 UI 绑定更新
                    
                    //OnPropertyChanged(nameof(CurrentBitDisplayModeInt));
                }
            }
        }
        private string _displayText = "";
        public string DisplayText
        {
            get { return _displayText; }
            set { SetProperty<string>(ref _displayText, value); }
        }
        public ICommand ButtonClickCommand { get; }
        public ICommand EqualsClickCommand { get; }
        public ICommand ACClickCommand { get; }
        public ICommand BackClickCommand { get; }
        public ICommand PercentageClickCommand { get; }
        public ICommand BitwiseCommand { get; }

        public Action EqualsClickEvent;
        Model.OperationRegistry registry;
        public ProgrammerViewModel()
        {
            registry = new Model.OperationRegistry();

            registry.RegisterOperation(Model.OperationType.Equal, new Model.ProgrammerCommand.EqualOperation());
            registry.RegisterOperation(Model.OperationType.AC, new Model.ProgrammerCommand.ACOperation());
            registry.RegisterOperation(Model.OperationType.Back, new Model.ProgrammerCommand.BackOperation());
            registry.RegisterOperation(Model.OperationType.Percentage, new Model.ProgrammerCommand.PercentageOperation());



            ButtonClickCommand = new RelayCommand<string>(
                    (string parameter) =>
                    {
                        DisplayText += parameter;
                    }
                );
            EqualsClickCommand = new RelayCommand
            (
                    () =>
                    {
                        //string str = Model.OperationRegistry.GetOperation(Model.OperationType.Equal).Execute(DisplayText);
                        string calculatedResultString = Model.OperationRegistry.GetOperation(Model.OperationType.Equal).Execute(DisplayText);
                        string cleanResult = calculatedResultString.Trim();
                        long resultValue = 0; // 使用 long 确保计算精度
                        Debug.WriteLine($"Calculated Result String: {cleanResult}");
                        int baseValue = CurrentBitDisplayModeInt;
                        Debug.WriteLine($"Current Base Value: {baseValue}");
                        if (long.TryParse(cleanResult, out resultValue))
                        {
                            string finalDisplay = Convert.ToString(resultValue, baseValue);
                            DisplayText = finalDisplay.ToUpper(); // 十六进制结果转换为大写
                        }
                        else
                        {
                            // 如果十进制解析失败，意味着计算模块返回了非法字符（例如 "Error" 或 "NaN"）
                            DisplayText = "Error: Calculation failed or returned non-numeric data.";
                        }
                    }
            );
            ACClickCommand = new RelayCommand
            (
                    () =>
                    {
                        DisplayText = Model.OperationRegistry.GetOperation(Model.OperationType.AC).Execute(DisplayText);
                    }
            );
            BackClickCommand = new RelayCommand
            (
                    () =>
                    {
                        DisplayText = Model.OperationRegistry.GetOperation(Model.OperationType.Back).Execute(DisplayText);
                    }
            );
            PercentageClickCommand = new RelayCommand
            (
                    () =>
                    {
                        DisplayText = Model.OperationRegistry.GetOperation(Model.OperationType.Percentage).Execute(DisplayText);
                    }
            );
            BitwiseCommand = new RelayCommand<string>(
                    (string parameter) =>
                    {
                        DisplayText += " " + parameter + " ";
                    }
                );

        }

        
    }
}