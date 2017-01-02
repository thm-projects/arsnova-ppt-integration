using System.Windows.Controls;

namespace ARSnovaPPIntegration.Presentation.Resources
{
    public class NumericTextBox : TextBox
    {
        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e) 
        { 
            e.Handled = !AreAllValidNumericChars(e.Text); 
            base.OnPreviewTextInput(e); 
        }

        private static bool AreAllValidNumericChars(string str)
        {
            if (str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyGroupSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.CurrencySymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeSign |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NegativeInfinitySymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.NumberGroupSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentDecimalSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentGroupSeparator |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PercentSymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PerMilleSymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveInfinitySymbol |
                str == System.Globalization.NumberFormatInfo.CurrentInfo.PositiveSign)
            {
                return true;
            }

            var ret = true;
            var l = str.Length;
            for (var i = 0; i < l; i++)
            {
                var ch = str[i];
                ret &= char.IsDigit(ch);
            }

            return ret;
        }
    }
}
