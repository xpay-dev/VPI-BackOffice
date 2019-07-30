using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SDGUtil
{
    public static class ExtensionMethods
    {
        public static void CopyProperties<T, K>(this T targetObject, K source, string[] objExclusions = null)
        {
            PropertyInfo[] allProporties = source.GetType().GetProperties();
            PropertyInfo targetProperty;

            foreach (PropertyInfo fromProp in allProporties)
            {
                targetProperty = targetObject.GetType().GetProperty(fromProp.Name);
                if (targetProperty == null) continue;
                if (!targetProperty.CanWrite) continue;

                if (objExclusions != null)
                    if (objExclusions.Contains(targetProperty.PropertyType.Name)) continue;

                targetProperty.SetValue(targetObject, fromProp.GetValue(source, null), null);
            }
        }
    }
}
