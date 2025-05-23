﻿using System;
using System.Runtime.InteropServices;

namespace GLFrameWork
{
    /// <summary>
    ///     Wrapper around a EGL display pointer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct EGLDisplay : IEquatable<EGLDisplay>
    {
        /// <summary>
        ///     Describes a default/null instance.
        /// </summary>
        public static readonly EGLDisplay None;

        /// <summary>
        ///     Internal pointer.
        /// </summary>
        private readonly IntPtr handle;

        /// <summary>
        ///     Performs an implicit conversion from <see cref="EGLDisplay" /> to <see cref="IntPtr" />.
        /// </summary>
        /// <param name="display">The display.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator IntPtr(EGLDisplay display) { return display.handle; }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() { return handle.ToString(); }

        /// <summary>
        ///     Determines whether the specified <see cref="EGLDisplay" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="EGLDisplay" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="EGLDisplay" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EGLDisplay other) { return handle.Equals(other.handle); }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is EGLDisplay display)
                return Equals(display);
            return false;
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode() { return handle.GetHashCode(); }

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(EGLDisplay left, EGLDisplay right) { return left.Equals(right); }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(EGLDisplay left, EGLDisplay right) { return !left.Equals(right); }
    }
}