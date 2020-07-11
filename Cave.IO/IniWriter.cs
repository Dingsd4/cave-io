using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Cave.IO
{
    /// <summary>
    /// Provides a fast and simple ini writer class.
    /// </summary>
    [DebuggerDisplay("{" + nameof(FileName) + "}")]
    public class IniWriter
    {
        readonly Dictionary<string, List<string>> data = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);
        IniProperties properties;

        #region construtors

        /// <summary>Initializes a new instance of the <see cref="IniWriter"/> class.</summary>
        public IniWriter()
        {
            Properties = IniProperties.Default;
            FileName = "Default";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniWriter"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file to write to.</param>
        /// <param name="properties">Encoding properties.</param>
        public IniWriter(string fileName, IniProperties properties)
        {
            Properties = properties.Valid ? properties : IniProperties.Default;
            FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
            if (File.Exists(fileName))
            {
                Load(IniReader.FromFile(fileName));
            }
            else
            {
                using (var f = File.Open(fileName, FileMode.OpenOrCreate))
                {
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IniWriter"/> class.
        /// </summary>
        /// <param name="reader">Settings to initialize the writer from.</param>
        public IniWriter(IniReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            FileName = reader.FileName;
            Properties = reader.Properties;
            Load(reader);
        }

        #endregion

        /// <summary>
        /// Gets or sets the IniProperties.
        /// </summary>
        public IniProperties Properties
        {
            get => properties;
            set
            {
                if (!(value.Culture.Calendar is GregorianCalendar))
                {
                    throw new NotSupportedException($"Calendar {value.Culture.Calendar} is not supported. Use a Properties.Culture with GregorianCalendar!");
                }
                if (value.Culture.IsNeutralCulture && Properties.Culture != CultureInfo.InvariantCulture)
                {
                    throw new NotSupportedException($"Culture {value.Culture} is neutral. Use CultureInfo.InvariantCulture instead!");
                }

                properties = value;
            }
        }

        /// <summary>
        /// Gets or sets name of the ini writer.
        /// </summary>
        public string FileName { get; set; }

        #region static constructors

        /// <summary>Creates an new initialization writer by parsing the specified data.</summary>
        /// <param name="name">The (file)name.</param>
        /// <param name="data">Content to parse.</param>
        /// <param name="properties">The data properties.</param>
        /// <returns>Returns a new <see cref="IniWriter"/> instance.</returns>
        public static IniWriter Parse(string name, string data, IniProperties properties = default)
        {
            return new IniWriter(IniReader.Parse(name, data, properties));
        }

        /// <summary>Creates an new initialization writer by parsing the specified data.</summary>
        /// <param name="name">The name.</param>
        /// <param name="data">Content to parse.</param>
        /// <param name="properties">The data properties.</param>
        /// <returns>Returns a new <see cref="IniWriter"/> instance.</returns>
        public static IniWriter Parse(string name, byte[] data, IniProperties properties = default)
        {
            return Parse(name, Encoding.UTF8.GetString(data), properties);
        }

        /// <summary>Creates an new initialization writer by parsing the specified data.</summary>
        /// <param name="name">The name.</param>
        /// <param name="lines">Content to parse.</param>
        /// <param name="properties">The content properties.</param>
        /// <returns>Returns a new <see cref="IniWriter"/> instance.</returns>
        public static IniWriter Parse(string name, string[] lines, IniProperties properties = default)
        {
            return new IniWriter(IniReader.Parse(name, lines, properties));
        }

        /// <summary>Creates an new initialization writer with the specified preexisting content.</summary>
        /// <param name="fileName">File name to read.</param>
        /// <param name="properties">The content properties.</param>
        /// <returns>Returns a new <see cref="IniWriter"/> instance.</returns>
        public static IniWriter FromFile(string fileName, IniProperties properties = default)
        {
            return File.Exists(fileName) ? Parse(fileName, File.ReadAllBytes(fileName), properties) : new IniWriter(fileName, properties);
        }

        /// <summary>Creates an new initialization writer with the specified preexisting content.</summary>
        /// <param name="name">The name.</param>
        /// <param name="stream">The stream to read.</param>
        /// <param name="count">Number of bytes to read.</param>
        /// <param name="properties">The content properties.</param>
        /// <returns>Returns a new <see cref="IniWriter"/> instance.</returns>
        public static IniWriter FromStream(string name, Stream stream, int count, IniProperties properties = default)
        {
            byte[] data = stream.ReadBlock(count);
            return Parse(name, data, properties);
        }

        #endregion

        /// <summary>
        /// Loads all settings from the specified reader and replaces all present sections.
        /// </summary>
        /// <param name="reader">The reader to obtain the config from.</param>
        public void Load(IniReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            foreach (string section in reader.GetSectionNames())
            {
                data[section] = new List<string>(reader.ReadSection(section, false));
            }
        }

        /// <summary>
        /// Removes a whole section from the ini file.
        /// </summary>
        /// <param name="section">Name of the section.</param>
        public void RemoveSection(string section)
        {
            Ini.CheckName(section, nameof(section));
            if (data.ContainsKey(section))
            {
                if (!data.Remove(section))
                {
                    throw new KeyNotFoundException();
                }
            }
        }

        /// <summary>
        /// Writes (replaces) a whole section at the ini.
        /// </summary>
        /// <param name="section">Name of the section.</param>
        /// <param name="value">The value.</param>
        public void WriteSection(string section, string value)
        {
            WriteSection(section, value.SplitNewLine());
        }

        /// <summary>
        /// Writes (replaces) a whole section at the ini.
        /// </summary>
        /// <param name="section">Name of the section.</param>
        /// <param name="values">The values.</param>
        public void WriteSection(string section, IEnumerable values)
        {
            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            Ini.CheckName(section, nameof(section));

            var strings = new List<string>();
            foreach (object value in values)
            {
                strings.Add($"{value}");
            }
            WriteSection(section, strings);
        }

        /// <summary>
        /// Writes (replaces) a whole section at the ini.
        /// </summary>
        /// <param name="section">Name of the section.</param>
        /// <param name="lines">The lines.</param>
        public void WriteSection(string section, IEnumerable<string> lines)
        {
            if (lines == null)
            {
                throw new ArgumentNullException("lines");
            }
            Ini.CheckName(section, nameof(section));

            var result = new List<string>();
            result.AddRange(lines.Select(line => Ini.Escape(line, Properties.BoxCharacter)));
            data[section] = result;
        }

        /// <summary>
        /// Writes all fields of the struct to the specified section (replacing a present one).
        /// </summary>
        /// <typeparam name="T">The struct type.</typeparam>
        /// <param name="section">The section to write to.</param>
        /// <param name="item">The struct.</param>
        [Obsolete("Use WriteFields instead!")]
        public void WriteStruct<T>(string section, T item)
            where T : struct
            => WriteFields<T>(section, item);

        /// <summary>
        /// Writes all fields of the object to the specified section (replacing a present one).
        /// </summary>
        /// <typeparam name="T">The class type.</typeparam>
        /// <param name="section">The section to write to.</param>
        /// <param name="obj">The object.</param>
        [Obsolete("Use WriteFields instead!")]
        public void WriteObject<T>(string section, T obj)
            where T : class
            => WriteFields<T>(section, obj);

        /// <summary>
        /// Writes all fields of the object to the specified section (replacing a present one).
        /// </summary>
        /// <typeparam name="T">The class type.</typeparam>
        /// <param name="section">The section to write to.</param>
        /// <param name="obj">The object.</param>
        public void WriteFields<T>(string section, T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                var value = field.GetValue(obj);
                WriteSetting(section, field.Name, value);
            }
        }

        /// <summary>
        /// Writes all properties of the object to the specified section (replacing a present one).
        /// </summary>
        /// <typeparam name="T">The class type.</typeparam>
        /// <param name="section">The section to write to.</param>
        /// <param name="obj">The object.</param>
        public void WriteProperties<T>(string section, T obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var field in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var value = field.GetValue(obj, null);
                WriteSetting(section, field.Name, value);
            }
        }

        /// <summary>
        /// Writes a setting to the ini file (replacing a present one).
        /// </summary>
        /// <param name="section">Name of the section.</param>
        /// <param name="name">Name of the setting.</param>
        /// <param name="value">Value of the setting.</param>
        public void WriteSetting(string section, string name, object value)
        {
            WriteSetting(section, name, StringExtensions.ToString(value, Properties.Culture));
        }

        /// <summary>
        /// Writes a setting to the ini tile (replacing a present one).
        /// </summary>
        /// <param name="section">Name of the section.</param>
        /// <param name="valueName">Name of the setting.</param>
        /// <param name="value">Value of the setting.</param>
        public void WriteSetting(string section, string valueName, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            Ini.CheckName(section, nameof(section));
            Ini.CheckName(valueName, nameof(valueName));

            List<string> result;
            if (data.ContainsKey(section))
            {
                result = data[section];
            }
            else
            {
                result = new List<string>();
                data[section] = result;
            }

            // try to replace first
            for (int i = 0; i < result.Count; i++)
            {
                string setting = result[i].BeforeFirst('=').Trim();
                if (string.Equals(setting, valueName.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    result[i] = valueName + "=" + Ini.Escape(value, Properties.BoxCharacter);
                    return;
                }
            }

            // add new one
            result.Add(valueName + "=" + Ini.Escape(value, Properties.BoxCharacter));
        }

        /// <summary>
        /// Saves the content of the ini to a file readable by <see cref="IniReader"/>.
        /// </summary>
        /// <param name="fileName">The fileName to write to.</param>
        public void Save(string fileName = null)
        {
            if (fileName == null)
            {
                fileName = FileName;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(fileName)));
            Stream stream = File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.None);
            try
            {
                if (Properties.Encryption != null)
                {
                    stream = new CryptoStream(stream, Properties.Encryption.CreateEncryptor(), CryptoStreamMode.Write);
                }
                switch (Properties.Compression)
                {
                    case IniCompressionType.Deflate:
                        stream = new DeflateStream(stream, CompressionMode.Compress, true);
                        break;
                    case IniCompressionType.GZip:
                        stream = new GZipStream(stream, CompressionMode.Compress, true);
                        break;
                    case IniCompressionType.None: break;
                    default: throw new InvalidDataException(string.Format("Unknown Compression {0}", Properties.Compression));
                }

                var writer = new StreamWriter(stream, Properties.Encoding);
                foreach (string section in data.Keys)
                {
                    writer.WriteLine("[" + section + "]");
                    bool allowOneEmpty = false;
                    foreach (string setting in data[section])
                    {
                        if (string.IsNullOrEmpty(setting) || (setting.Trim().Length == 0))
                        {
                            if (allowOneEmpty)
                            {
                                writer.WriteLine();
                                allowOneEmpty = false;
                            }
                            continue;
                        }
                        writer.WriteLine(setting);
                        allowOneEmpty = true;
                    }
                    writer.WriteLine();
                }
                writer.Close();
            }
            catch
            {
                stream.Dispose();
                throw;
            }
        }

        /// <summary>Converts all settings to a new reader.</summary>
        /// <returns>Returns a new instance containing all settings.</returns>
        public IniReader ToReader()
        {
            return IniReader.Parse(FileName, ToString(), Properties);
        }

        /// <summary>
        /// Retrieves the whole data as string.
        /// </summary>
        /// <returns>Returns the content of the settings in ini format.</returns>
        public override string ToString()
        {
            using (var writer = new StringWriter())
            {
                foreach (string section in data.Keys)
                {
                    writer.WriteLine("[" + section + "]");
                    bool allowOneEmpty = false;
                    foreach (string setting in data[section])
                    {
                        if (string.IsNullOrEmpty(setting) || (setting.Trim().Length == 0))
                        {
                            if (allowOneEmpty)
                            {
                                writer.WriteLine();
                                allowOneEmpty = false;
                            }
                            continue;
                        }
                        writer.WriteLine(setting);
                        allowOneEmpty = true;
                    }
                    writer.WriteLine();
                }
                return writer.ToString();
            }
        }
    }
}
