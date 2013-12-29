using System;
using System.IO;
using System.Xml.Serialization;

namespace Medidata.Lumberjack.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SerializableBase : SessionObject
    {
        #region Initializers

        /// <summary>
        /// 
        /// </summary>
        protected SerializableBase() : this(null) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        protected SerializableBase(UserSession session) : base(session) { }

        #endregion

        #region Protected methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Filename of the file to write the XML data to.</param>
        /// <returns></returns>
        protected bool Serialize(string filename) {
            return Serialize(filename, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Filename of the file to write the XML data to.</param>
        /// <param name="o">The object to serialize.</param>
        /// <returns></returns>
        protected bool Serialize(string filename, SerializableBase o) {
            var success = false;
            var type = o.GetType();

            try {
                using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    var serializer = new XmlSerializer(type);
                    serializer.Serialize(fs, o);
                    fs.Close();
                }

                success = true;
            } catch (IOException) {
                OnError("An IO exception occured while serializing " + type.Name + " to file \"" + filename + "\".");
            } catch (SystemException) {
                OnError("A system exception occurred while serializing " + type.Name + " to file \"" + filename + "\".");
            } catch (Exception ex) {
                OnError("An exception occured while serializing " + type.FullName + " to file \"" + filename + "\".\r\nException: " + ex.Message, ex);
            }

            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename">Filename of the XML file to deserialize.</param>
        /// <returns></returns>
        protected SerializableBase Deserialize(string filename) {
            object o = null;
            var type = GetType();

            try {
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.None)) {
                    var serializer = new XmlSerializer(type);
                    o = serializer.Deserialize(fs);
                    fs.Close();
                }
            } catch (IOException) {
                OnError("An IO exception occured while deserializing " + type.Name + " from file \"" + filename + "\".");
            } catch (SystemException ex) {
                OnError("A system exception occurred while deserializing " + type.Name + " from file \"" + filename + "\"", ex);
            } catch (Exception ex) {
                OnError("An exception occured while deserializing " + type.FullName + " from file \"" + filename + "\".\r\nException: " + ex.Message, ex);
            }

            return (SerializableBase)o;
        }

        #endregion
    }
}
