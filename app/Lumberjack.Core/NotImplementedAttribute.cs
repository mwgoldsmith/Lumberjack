using System;

namespace Medidata.Lumberjack.Core
{
    [AttributeUsage(
        AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field |
        AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Delegate, Inherited = false)]
    public sealed class NotImplementedAttribute : Attribute
    {
        #region Private fields

        private readonly string _message;
        private readonly bool _error;

        #endregion

        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        public NotImplementedAttribute()
            : this(null) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public NotImplementedAttribute(string message)
            : this(message, false) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="error"></param>
        public NotImplementedAttribute(string message, bool error) {
            _message = message;
            _error = error;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string Message {
            get { return _message; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsError {
            get { return _error; }
        }

        #endregion
    }
}