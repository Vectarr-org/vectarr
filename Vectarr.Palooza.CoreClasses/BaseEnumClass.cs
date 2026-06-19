using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vectarr.Palooza.CoreClasses
{
    public abstract class BaseEnumClass<T> where T : IConvertible
    {
        public abstract string GetDisplayText(T enumValue);

        public List<T> GetAllValues()
        {
            var baseArray = Enum.GetValues(typeof(T));
            return baseArray.Cast<T>().ToList();
        }

        public T GetEnum(string displayText)
        {
            var values = GetAllValues();
            foreach (var value in values)
            {
                if (displayText.Equals(GetDisplayText(value), StringComparison.OrdinalIgnoreCase))
                    return value;
            }
            throw new ArgumentOutOfRangeException(nameof(displayText), displayText, null);
        }

        public T GetParsedValue(string value)
        {
            return (T)Enum.Parse(typeof(T), value);
        }

        public List<T> GetCommaParsedValues(string values)
        {
            var list = new List<T>();
            var separatedValues = values.Split(",");
            foreach (var value in separatedValues)
            {
                list.Add((T)Enum.Parse(typeof(T), value));
            }
            return list;
        }

        public List<EnumListModel> GetListModel()
        {
            var values = Enum.GetValues(typeof(T));
            return values.Cast<T>().ToList().Select(x => new EnumListModel
            {
                Id = Convert.ToInt32(x),
                DisplayName = GetDisplayText(x)
            })
            .OrderBy(x => x.DisplayName)
            .ToList();
        }

        public List<EnumListModel> GetNonOrderedListModel()
        {
            var values = Enum.GetValues(typeof(T));
            return values.Cast<T>().ToList().Select(x => new EnumListModel
            {
                Id = Convert.ToInt32(x),
                DisplayName = GetDisplayText(x)
            })
            .ToList();
        }
    }
}
