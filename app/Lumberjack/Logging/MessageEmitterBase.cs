using System;

namespace Medidata.Lumberjack.Logging
{
    public abstract class MessageEmitterBase
    {
        #region Public fields

        /// <summary>
        /// 
        /// </summary>
        public event MessageEmitterHandler OnStatusChange = null;

        /// <summary>
        /// 
        /// </summary>
        public event MessageEmitterHandler OnError = null;

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void StatusMessage(string message)
        {
            if (OnStatusChange != null)
                OnStatusChange.Invoke(this, new MessageEmitterEventArgs(MessageTypeEnum.Status, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        protected void ErrorMessage(string message)
        {
            if (OnError != null)
                OnError.Invoke(this, new MessageEmitterEventArgs(MessageTypeEnum.Error, message));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        protected void ErrorMessage(Exception ex)
        {
            if (OnError != null)
                OnError.Invoke(this, new MessageEmitterEventArgs(MessageTypeEnum.Exception, ex));
        }

        #endregion
    }
}
