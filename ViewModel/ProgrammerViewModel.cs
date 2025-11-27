using System;
using System.Collections.Generic;
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

        private bool isHexMode;
        public bool IsHexMode
        {
            get { return isHexMode; }
            set { SetProperty<bool>(ref isHexMode, value); }
        }

        private bool isDecMode = true;
        public bool IsDecMode
        {
            get { return isDecMode; }
            set { SetProperty<bool>(ref isDecMode, value); }
        }
        private bool isOctMode;
        public bool IsOctMode
        {
            get { return isOctMode; }
            set { SetProperty<bool>(ref isOctMode, value); }
        }
        private bool isBinMode;
        public bool IsBinMode
        {
            get { return isBinMode; }
            set { SetProperty<bool>(ref isBinMode, value); }
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
                        DisplayText = Model.OperationRegistry.GetOperation(Model.OperationType.Equal).Execute(DisplayText);
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