using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMainWinods.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 转换为Int类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static int ObjectToInt(this object thisValue)
        {
            int reval = 0;
            if (thisValue == null) return reval;
            if (thisValue is Enum) return (int)thisValue;
            return thisValue != null && thisValue != DBNull.Value && int.TryParse(thisValue.ToString(), out reval) ? reval : 0;
        }

        /// <summary>
        /// 转换为String类型
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string ObjectToString(this object thisValue)
        {
            return thisValue != null ? thisValue.ToString().Trim() : string.Empty;
        }

    }
}
