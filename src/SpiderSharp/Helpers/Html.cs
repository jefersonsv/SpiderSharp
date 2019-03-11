using System;

namespace SpiderSharp.Helpers
{
    public static class Html
    {
        public static Nodes TryParse(string content)
        {
            try
            {
                return new Nodes(content);
            }
            catch (Exception ex)
            {
            }

            return null;
        }
    }
}