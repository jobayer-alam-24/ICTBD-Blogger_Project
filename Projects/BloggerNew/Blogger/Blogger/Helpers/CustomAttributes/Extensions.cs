using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blogger.Extensions;

namespace Blogger.Helpers.CustomAttributes
{
    public static class Extensions
    {
        public static string GetBlogUrl(this string value)
        {
            return value.Replace(' ', '-').ToLower();
        }
        public static string GetCategoryImageUrl(this string value)
        {
            return Defaults.CATEGORY_ROOT_PATH + value;
        }
        public static string GetBlogImageUrl(this string value)
        {
            return Defaults.POST_ROOT_PATH + value;
        }
    }
}