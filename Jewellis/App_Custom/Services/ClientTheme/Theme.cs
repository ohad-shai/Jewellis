﻿using System;

namespace Jewellis.App_Custom.Services.ClientTheme
{
    /// <summary>
    /// Represents a supported theme in the application.
    /// </summary>
    public class Theme
    {
        #region Properties

        /// <summary>
        /// Gets the ID of the theme.
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the display name of the theme.
        /// </summary>
        public string DisplayName { get; private set; }

        #endregion

        /// <summary>
        /// Represents a supported theme in the application.
        /// </summary>
        /// <param name="id">The id of the theme.</param>
        /// <param name="displayName">The display name of the theme.</param>
        public Theme(string id, string displayName)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id), $"{nameof(id)} cannot be null or empty.");

            this.ID = id;
            this.DisplayName = displayName;
        }

    }
}
