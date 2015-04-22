using System;

namespace MadsKristensen.ExtensibilityTools.VSCT.Signing
{
    /// <summary>
    /// Helper class to display items in combo-box.
    /// This code comes from: https://github.com/phofman/signature
    /// </summary>
    sealed class ComboBoxItem
    {
        private readonly string _text;

        /// <summary>
        /// Init constructor.
        /// </summary>
        public ComboBoxItem(object data, string text)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");

            Data = data;
            _text = text;
        }

        #region Properties

        public object Data
        {
            get;
            private set;
        }

        #endregion

        public override string ToString()
        {
            return _text;
        }
    }
}
