namespace LitJson
{
    public static class JsonHelper
    {
        public static T ToObject<T>(this JsonData dict)
        {
            return JsonMapper.ToObject<T>(dict.ToJson());
        }

        public static bool optBoolean(this JsonData dict, string key)
        {
            if (dict.Keys.Contains(key))
            {
                if (dict[key].IsBoolean)
                {
                    return (bool) dict[key];
                }
            }

            return false;
        }

        public static double optDouble(this JsonData dict, string key)
        {
            if (dict.Keys.Contains(key))
            {
                if (dict[key].IsDouble)
                {
                    return (double) dict[key];
                }

                if (dict[key].IsInt)
                {
                    return (int) dict[key];
                }

                if (dict[key].IsLong)
                {
                    return (long) dict[key];
                }
            }

            return 0;
        }

        public static int optInt(this JsonData dict, string key)
        {
            if (dict.Keys.Contains(key))
            {
                if (dict[key].IsInt)
                {
                    return (int) dict[key];
                }

            }

            return 0;
        }

        public static long optLong(this JsonData dict, string key)
        {
            if (dict.Keys.Contains(key))
            {
                if (dict[key].IsLong)
                {
                    return (long) dict[key];
                }
            }

            return 0;
        }

        public static string optString(this JsonData dict, string key)
        {
            return dict.getJson<string>(key) as string;
        }

        public static object getJson<T>(this JsonData dict, string key) where T : class
        {
            if (dict.Keys.Contains(key))
            {
                var value = dict[key];
                switch (value.GetJsonType())
                {
// #if false
                case JsonType.Boolean:
                    return (bool) value;
                case JsonType.Double:
                    return (double) value;
                case JsonType.Int:
                    return (int) value;
                case JsonType.Long:
                    return (long) value;
// #endif
                    case JsonType.String:
                        return (string) value;
                    default:
                        return value;
                }
            }

            return default(T);
        }
    }
}