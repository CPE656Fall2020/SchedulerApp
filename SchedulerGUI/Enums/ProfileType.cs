using System.ComponentModel;

namespace SchedulerGUI.Enums
{
    /// <summary>
    /// <see cref="ProfileType"/> defines different profile types that can be displayed.
    /// </summary>
    public enum ProfileType
    {
        /// <summary>
        /// AES Encryption profile type.
        /// </summary>
        [Description("AES Encryption")]
        AES,

        /// <summary>
        /// LZ4 Compression profile type.
        /// </summary>
        [Description("LZ4 Compression")]
        LZ4,
    }
}
