## 💻 Calculator (WinUI)

這是一個使用 **WinUI (Windows App SDK)** 框架開發的計算機應用程式，旨在提供一個功能齊全且具備現代 Windows 介面的基本計算工具。

### ✨ 功能特色

* **基本算術運算**：支援加 (`+`)、減 (`-`)、乘 (`*` 或 `×`)、除 (`/` 或 `÷`) 運算。
* **百分比計算**：支援 `%` 鍵，用於計算當前數字或運算式中最後一個數字的百分比值。
* **清空與回退**：
    * `C` (Clear) 鍵用於清除當前輸入。
    * **Backspace** 鍵 (`⌫`) 用於刪除最後輸入的字元。
* **小數點支援**：支援使用 `.` 進行小數點輸入。
* **WinUI 介面**：應用程式窗口標題設定為 "**小算盤**"；介面使用 `Grid` 佈局和自訂樣式（例如 `CalculatorNumberButtonStyle` 和 `CalculatorEqualsButtonStyle`）來組織按鈕和顯示區。

### 📐 專案架構與實作細節

專案採用 **MVVM (Model-View-ViewModel)** 模式來分離使用者介面、業務邏輯和資料。

#### 1. View (MainWindow.xaml / MainWindow.xaml.cs)

* `MainWindow.xaml` 檔案定義了計算機的介面佈局。
* UI 包含一個用於顯示公式或結果的 `TextBlock` (`ResultDisplay`)，其 `Text` 屬性繫結到 ViewModel 的 `DisplayText` 屬性。
* 按鈕 (`Button0` 到 `Button9`, `ButtonPlus`, `ButtonEquals` 等) 通過 **Command** 機制繫結到 `ButtonViewModel` 中的相應 `ICommand`.
    * 數字和基本運算符使用 `ButtonClickCommand<string>`，並通過 `CommandParameter` 傳遞其值.
    * 特殊操作（如等號、AC、回退、百分比）有各自的專屬命令 (`EqualsClickCommand`, `ACClickCommand`, `BackClickCommand`, `PercentageClickCommand`).

#### 2. ViewModel (ViewModel/ButtonViewModel.cs)

* `ButtonViewModel` 繼承自 `ViewModelBase` 以實現 `INotifyPropertyChanged`，用於通知 UI 屬性 (例如 `DisplayText`) 的變更.
* 它包含多個 `ICommand` 實作，負責處理按鈕點擊事件.
* 在建構函式中，它註冊了運算邏輯到 `KeyCommand.OperationRegistry`.
* 當執行計算 (`EqualsClickCommand`) 或其他特殊操作時，它會呼叫 `KeyCommand` 類別中對應的 `IOperation` 實作.

#### 3. Model (Model/KeyCommand.cs)

* `KeyCommand.cs` 檔案包含了計算機的**核心邏輯**.
* 它定義了 `IOperation` 介面和不同的操作實作類別：
    * `EqualOperation`：實作了 **中綴轉後綴** (Shunting-Yard 演算法) 和 **後綴表達式求值** (RPN 演算法).
        * `TokenizeExpression`：將中綴表達式字串拆分為 Token 列表.
        * `ShuntingYard.ConvertToPostfix`：將 Token 列表從中綴轉換為後綴.
        * `EvaluateRPN`：計算後綴表達式的值.
    * `ACOperation`：清除當前顯示文字.
    * `BackOperation`：刪除最後一個字元.
    * `PercentageOperation`：計算百分比.

### 🛠 技術棧與環境

* **專案類型**：C# Windows Application (WinExe).
* **框架**：.NET 8.0 with WinUI (Windows App SDK).
* **目標平台**：Windows 10.0.19041.0 (最低支援 10.0.17763.0).
* **專案建置**：Visual Studio Solution (`.sln`) 文件，包含 `Calculator` 專案 (`.csproj`).

### 🚀 如何運行

1.  **安裝必要組件**：確保您的開發環境安裝了 **Visual Studio** 和 **Windows App SDK** 的開發工具.
2.  **打開專案**：使用 Visual Studio 打開 `Calculator.sln` 檔案.
3.  **選擇啟動配置**：您可以選擇 `Calculator (Package)` 或 `Calculator (Unpackaged)` 進行啟動和測試.
4.  **運行**：按下 F5 或點擊運行按鈕來啟動應用程式.# Calculator