namespace FIU.ThriveToDocuSign.Core
{
    using System.ComponentModel;

    public enum DocuSignApiType
    {
        /// <summary>
        /// Rooms API
        /// </summary>
        [Description("reg")]
        Rooms = 0,

        /// <summary>
        /// ESignature API
        /// </summary>
        [Description("eg")]
        ESignature = 1,

        /// <summary>
        /// Click API
        /// </summary>
        [Description("ceg")]
        Click = 2,

        /// <summary>
        /// Monitor API
        /// </summary>
        [Description("meg")]
        Monitor = 3,

        /// <summary>
        /// Admin API
        /// </summary>
        [Description("aeg")]
        Admin = 4,
    }

    public static class DocuSignApiTypeExtensions
    {
        public static string ToKeywordString(this DocuSignApiType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }
}