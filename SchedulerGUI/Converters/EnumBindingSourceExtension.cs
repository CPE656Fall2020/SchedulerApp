﻿using System;
using System.Windows.Markup;

namespace SchedulerGUI.Converters
{
    /// <summary>
    /// <see cref="EnumBindingSourceExtension"/> provides support for binding a dropdown list to an enum.
    /// </summary>
    /// <remarks>
    /// Originally Adapted from https://brianlagunas.com/a-better-way-to-data-bind-enums-in-wpf/.
    /// Copied from https://github.com/alexdillon/GroupMeClient/blob/develop/GroupMeClient.WpfUI/Extensions/EnumBindingSourceExtension.cs.
    /// </remarks>
    public class EnumBindingSourceExtension : MarkupExtension
    {
        private Type enumType;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBindingSourceExtension"/> class.
        /// </summary>
        public EnumBindingSourceExtension()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnumBindingSourceExtension"/> class.
        /// </summary>
        /// <param name="enumType">The enum type to bind to.</param>
        public EnumBindingSourceExtension(Type enumType)
        {
            this.EnumType = enumType;
        }

        /// <summary>
        /// Gets or sets the type of enum that is being bound.
        /// </summary>
        public Type EnumType
        {
            get
            {
                return this.enumType;
            }

            set
            {
                if (value != this.enumType)
                {
                    if (value != null)
                    {
                        Type enumType = Nullable.GetUnderlyingType(value) ?? value;

                        if (!enumType.IsEnum)
                        {
                            throw new ArgumentException("Type must be for an Enum.");
                        }
                    }

                    this.enumType = value;
                }
            }
        }

        /// <inheritdoc/>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.enumType == null)
            {
                throw new InvalidOperationException("The EnumType must be specified.");
            }

            Type actualEnumType = Nullable.GetUnderlyingType(this.enumType) ?? this.enumType;
            Array enumValues = Enum.GetValues(actualEnumType);

            if (actualEnumType == this.enumType)
            {
                return enumValues;
            }

            Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            enumValues.CopyTo(tempArray, 1);
            return tempArray;
        }
    }
}
