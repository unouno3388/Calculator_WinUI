using Microsoft.UI.Xaml.Data;
using System;
using System.Reflection;

namespace Calculator.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null || parameter == null)
                return false;

            // 比较 ViewModel 的实际值 (value) 是否等于 RadioButton 传入的预期值 (parameter)
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isChecked && isChecked)
            {
                var enumString = parameter as string;

                // 确保 targetType 是 Enum 且参数有效
                if (targetType.GetTypeInfo().IsEnum && enumString != null)
                {
                    try
                    {
                        // 1. 解析字符串，返回值为 object (其底层类型通常是 int)
                        object parsedValue = Enum.Parse(targetType, enumString);

                        // 🌟 关键修正：使用 Enum.ToObject 明确返回目标 Enum 类型
                        // 这能确保返回的对象类型与 ViewModel 属性类型完全匹配，避免 InvalidCastException
                        return Enum.ToObject(targetType, parsedValue);
                    }
                    catch (ArgumentException)
                    {
                        // 如果 Enum.Parse 失败，返回 UnsetValue
                        return Microsoft.UI.Xaml.DependencyProperty.UnsetValue;
                    }
                }
            }

            return Microsoft.UI.Xaml.DependencyProperty.UnsetValue;
        }
    }
}