namespace WiseyeStudio.Steam.Utils
{
    public enum FloatingGamepadTextInputMode
    {
        /// <summary>
        /// Enter dismisses the keyboard.
        /// </summary>
        SingleLine = 0,
        /// <summary>
        /// The user needs to explicitly close the keyboard.
        /// </summary>
        MultipleLines = 1,
        /// <summary>
        /// Keyboard layout is email, enter dismisses the keyboard.
        /// </summary>
        Email = 2,
        /// <summary>
        /// Keyboard layout is numeric, enter dismisses the keyboard.
        /// </summary>
        Numeric = 3
    }
}
