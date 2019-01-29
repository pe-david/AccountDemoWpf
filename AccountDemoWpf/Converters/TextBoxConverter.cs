using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace AccountDemoWpf.Converters
{
    public class TextBoxConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof(string) && toType == typeof(double))
                return 1;

            return 0;
        }

        public bool TryConvert(object @from, Type toType, object conversionHint, out object result)
        {
            // this is NOT working correctly...
            result = null;
            var text = @from as string;
            text = text.Where(char.IsDigit).ToString();
            if (string.IsNullOrEmpty(text)) return false;

            try
            {
                result = Convert.ToDouble(text);
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}
