using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Windows.Input; // 在 WinUI/UWP 中通常使用 System.Windows.Input
using Model;
namespace ViewModel
{

    public class StanderViewModel : ViewModelBase
    {


        // 新增用於選單的屬性
        private string _currentCalculatorType = "標準";
        public string CurrentCalculatorType
        {
            get { return _currentCalculatorType; }
            set { SetProperty<string>(ref _currentCalculatorType, value); }
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

        public Action EqualsClickEvent;
        Model.OperationRegistry registry;
        public StanderViewModel()
        {
            registry = new Model.OperationRegistry();

            registry.RegisterOperation(Model.OperationType.Equal, new Model.StandardCommand.EqualOperation());
            registry.RegisterOperation(Model.OperationType.AC, new Model.StandardCommand.ACOperation());
            registry.RegisterOperation(Model.OperationType.Back, new Model.StandardCommand.BackOperation());
            registry.RegisterOperation(Model.OperationType.Percentage, new Model.StandardCommand.PercentageOperation());


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

        }

    }
    
}