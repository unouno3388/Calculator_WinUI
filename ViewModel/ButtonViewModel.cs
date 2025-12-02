//namespace Viewmodel
//{
//    using System;
//    using System.ComponentModel;
//    using System.Reflection.Metadata;
//    using System.Runtime.CompilerServices;
//    using System.Windows.Input; // 在 WinUI/UWP 中通常使用 System.Windows.Input

//    using model = Model;
//    // 基礎 ViewModel 類別，用於處理屬性變更通知
//    public class ViewModelBase : INotifyPropertyChanged
//    {
//        public event PropertyChangedEventHandler PropertyChanged;

//        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
//        {
//            // 如果有訂閱者，則發出屬性變更通知
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
//        }

//        // 協助方法：用於設定屬性值並自動發出通知
//        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
//        {
//            if (Equals(storage, value))
//            {
//                return false;
//            }

//            storage = value;
//            OnPropertyChanged(propertyName);
//            return true;
//        }
//    }


//    public class RelayCommand : ICommand
//    {
//        private readonly Action _execute;
//        private readonly Func<bool> _canExecute;

//        // 定義 ICommand 介面中必須實作的事件
//        public event EventHandler CanExecuteChanged;

//        // 建構子：接收一個 Action (要執行的方法)
//        public RelayCommand(Action execute) : this(execute, null) { }

//        // 建構子：接收一個 Action 和一個 Func<bool> (判斷是否可執行的函式)
//        public RelayCommand(Action execute, Func<bool> canExecute)
//        {
//            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
//            _canExecute = canExecute;
//        }

//        // 判斷命令是否可以執行
//        public bool CanExecute(object parameter)
//        {
//            return _canExecute == null || _canExecute();
//        }

//        // 執行命令的動作
//        public void Execute(object parameter)
//        {
//            _execute();
//        }

//        // 手動觸發 CanExecuteChanged 事件的方法
//        public void RaiseCanExecuteChanged()
//        {
//            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
//        }
//    }

//    public class RelayCommand<T> : ICommand
//    {
//        private readonly Action<T> _execute;
//        private readonly Func<T, bool> _canExecute;

//        public event EventHandler CanExecuteChanged;

//        // 建構函式：需要一個 Action<T> 來執行命令
//        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
//        {
//            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
//            _canExecute = canExecute;
//        }

//        // 判斷命令是否可以執行
//        public bool CanExecute(object parameter)
//        {
//            // 如果 parameter 不是 T 類型，且 T 不是可空類型，則視為不可執行
//            if (parameter != null && !(parameter is T))
//            {
//                return false;
//            }

//            // 如果沒有 CanExecute 邏輯，預設為 true；否則執行 CanExecute 函式
//            return _canExecute == null || _canExecute((T)parameter);
//        }

//        // 執行命令的動作
//        public void Execute(object parameter)
//        {
//            // 將傳入的 object 參數安全地轉換為 T 類型
//            _execute((T)parameter);
//        }

//        // 用於手動觸發 CanExecuteChanged 事件
//        public void RaiseCanExecuteChanged()
//        {
//            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
//        }
//    }
//    public class ButtonViewModel : ViewModelBase
//    {
//        private string _displayText = "";
//        public string DisplayText
//        {
//            get { return _displayText; }
//            set { SetProperty<string>(ref _displayText, value); }
//        }
//        public ICommand ButtonClickCommand { get; }
//        public ICommand EqualsClickCommand { get; }
//        public ICommand ACClickCommand { get; }
//        public ICommand BackClickCommand { get; }
//        public ICommand PercentageClickCommand { get; }

//        public Action EqualsClickEvent;
//        model.OperationRegistry registry;
//        public ButtonViewModel()
//        {
//            registry = new model.OperationRegistry();

//            registry.RegisterOperation(model.OperationType.Equal, new model.StandardCommand.EqualOperation());
//            registry.RegisterOperation(model.OperationType.AC, new model.StandardCommand.ACOperation());
//            registry.RegisterOperation(model.OperationType.Back, new model.StandardCommand.BackOperation());
//            registry.RegisterOperation(model.OperationType.Percentage, new model.StandardCommand.PercentageOperation());

//            ButtonClickCommand = new RelayCommand<string>(
//                    (string parameter) =>
//                    {
//                        DisplayText += parameter;
//                    }
//                );
//            EqualsClickCommand = new RelayCommand
//            (
//                  () =>
//                  {
//                      DisplayText = model.OperationRegistry.GetOperation(model.OperationType.Equal).Execute(DisplayText);
//                  }
//            );
//            ACClickCommand = new RelayCommand
//            (
//                  () =>
//                  {
//                      DisplayText = model.OperationRegistry.GetOperation(model.OperationType.AC).Execute(DisplayText);
//                  }
//            );
//            BackClickCommand = new RelayCommand
//            (
//                  () =>
//                  {
//                      DisplayText = model.OperationRegistry.GetOperation(model.OperationType.Back).Execute(DisplayText);
//                  }
//            );
//            PercentageClickCommand = new RelayCommand
//            (
//                  () =>
//                  {
//                      DisplayText = model.OperationRegistry.GetOperation(model.OperationType.Percentage).Execute(DisplayText);
//                  }
//            );

//        }


//    }

//}