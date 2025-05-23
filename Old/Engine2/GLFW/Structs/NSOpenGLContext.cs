using System;
using System.Runtime.InteropServices;

namespace GLFrameWork
{
    /// <summary>
    ///     Wrapper around a NSOpenGL context pointer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    // ReSharper disable once InconsistentNaming
    public struct NSOpenGLContext : IEquatable<NSOpenGLContext>
    {
        /// <summary>
        ///     Describes a default/null instance.
        /// </summary>
        public static readonly NSOpenGLContext None;

        /// <summary>
        ///     Internal pointer.
        /// </summary>
        private readonly IntPtr handle;

        /// <summary>
        ///     Performs an implicit conversion from <see cref="NSOpenGLContext" /> to <see cref="IntPtr" />.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator IntPtr(NSOpenGLContext context) { return context.handle; }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() { return handle.ToString(); }

        /// <summary>
        ///     Determines whether the specified <see cref="NSOpenGLContext" />, is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="NSOpenGLContext" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="NSOpenGLContext" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(NSOpenGLContext other) { return handle.Equals(other.handle); }

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is NSOpenGLContext context)
                return Equals(context);
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
        public static bool operator ==(NSOpenGLContext left, NSOpenGLContext right) { return left.Equals(right); }

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(NSOpenGLContext left, NSOpenGLContext right) { return !left.Equals(right); }
    }
}