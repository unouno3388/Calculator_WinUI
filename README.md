## 💻 Calculator (WinUI)

這是一個使用 **WinUI (Windows App SDK)** 框架開發的計算機應用程式，旨在提供一個功能齊全且具備現代 Windows 介面的多功能計算工具。目前支援「標準」與「程式設計師」兩種計算模式。

### ✨ 功能特色

#### 1. 標準模式 (Standard Mode)
* **基本算術運算**：支援加 (`+`)、減 (`-`)、乘 (`*`)、除 (`/`) 運算。
* **百分比計算**：支援 `%` 鍵，用於計算運算式中的百分比值。
* **小數點支援**：支援浮點數運算。

#### 2. 程式設計師模式 (Programmer Mode) 🆕
* **進制切換**：介面支援 **HEX** (十六進位)、**DEC** (十進位)、**OCT** (八進位)、**BIN** (二進位) 的顯示與切換。
* **十六進位輸入**：支援 `A` - `F` 按鍵輸入。
* **位元運算**：支援位元左移 (`<<` / `LSH`) 與右移 (`>>` / `RSH`) 運算。

#### 3. 通用功能
* **導航選單**：使用 `NavigationView` 提供模式切換功能，可於不同計算機類型間切換。
* **清空與回退**：
    * `C` (Clear) 鍵用於清除當前輸入。
    * **Backspace** 鍵 (`⌫`) 用於刪除最後輸入的字元。
* **WinUI 介面**：採用現代化的 `Grid` 佈局、Fluent Design 風格圖示與自訂樣式。

### 📐 專案架構與實作細節

專案採用 **MVVM (Model-View-ViewModel)** 模式，並針對不同計算模式進行了模組化設計。

#### 1. View (介面層)

* **MainWindow (Shell)**：
    * 作為應用程式的主視窗，包含 `NavigationView` 用於管理頁面導航。
    * 負責處理選單點擊事件，將 `ContentFrame` 導航至對應的計算機頁面 (`StandardCalculator` 或 `ProgrammerCalculator`)。
* **StandardCalculator**：標準計算機的獨立頁面，負責基礎運算的 UI 呈現。
* **ProgrammerCalculator**：程式設計師計算機的獨立頁面，包含進位選擇 (`RadioButton`) 和擴充按鍵佈局。

#### 2. ViewModel (邏輯層)

* **ViewModelBase**：
    * 實作 `INotifyPropertyChanged` 介面。
    * 定義了 `MenuItems` 列表，用於生成導航選單。
* **StanderViewModel**：
    * 負責標準模式的邏輯。
    * 綁定基本的 `ButtonClickCommand`、`EqualsClickCommand` 等命令，並呼叫 `StandardCommand` 模型進行運算。
* **ProgrammerViewModel**：
    * 負責程式設計師模式的邏輯。
    * 新增 `IsHexMode`, `IsDecMode` 等屬性來控制介面狀態（如 `A-F` 按鈕的啟用/禁用）。
    * 新增 `BitwiseCommand` 處理位元運算指令。

#### 3. Model (資料與演算法層)

核心邏輯已重構為針對不同模式的處理器，共用部分基礎邏輯：

* **ExpressionProcessor (抽象基底)**：
    * 定義了運算式的核心演算法，包括 **TokenizeExpression** (字串切割) 和 **ConvertToPostfix** (中綴轉後綴/Shunting-Yard 演算法)。
* **StandardCommand**：
    * `StanderdExpressionProcessor`：單例模式，處理十進位浮點數運算。
    * 實作標準的加減乘除邏輯。
* **ProgrammerCommand**：
    * `ProgrammerExpressionProcessor`：單例模式，專門處理程式設計師模式的運算。
    * **擴充解析邏輯**：支援解析十六進位字串（包含 A-F），在 `Hexadecimal` 模式下運作。
    * **位元運算支援**：在 `EvaluateRPN` 與 `TokenizeExpression` 中加入了對 `LSH` (左移) 和 `RSH` (右移) 的支援。

### 🛠 技術棧與環境

* **專案類型**：C# Windows Application (WinExe).
* **框架**：.NET 8.0 with WinUI 3 (Windows App SDK).
* **目標平台**：Windows 10.0.19041.0 (最低支援 10.0.17763.0)。
* **開發工具**：Visual Studio 2022.

### 🚀 如何運行

1.  **安裝必要組件**：確保您的開發環境安裝了 **Visual Studio** 和 **Windows App SDK** 的開發工具.
2.  **打開專案**：使用 Visual Studio 打開 `Calculator.sln` 檔案.
3.  **選擇啟動配置**：您可以選擇 `Calculator (Package)` 或 `Calculator (Unpackaged)` 進行啟動和測試.
4.  **運行**：按下 F5 或點擊運行按鈕來啟動應用程式.