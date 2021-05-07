
namespace GameApp.DataBinder
{
    public static class BindableValuesEx
    {
        public static ValueWrap GetWrap(this BindableValues values, string key)
        {
            foreach (var wrap in values.wraps)
            {
                if (wrap.name == key)
                {
                    return wrap;
                }
            }
            return null;
        }

        public static void RemoveWrap(this BindableValues values, string key)
        {
            for (var i = 0; i < values.wraps.Count; i++)
            {
                if (values.wraps[i].name == key)
                {
                    values.wraps.RemoveAt(i);
                    return;
                }
            }
        }

    }
}