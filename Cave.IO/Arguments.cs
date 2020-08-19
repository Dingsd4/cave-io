using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Cave.Collections;
using Cave.Collections.Generic;

namespace Cave.IO
{
    /// <summary>
    ///     Provides easy access to (commandline-) arguments and syntax checking of the following form:.
    ///     <p>
    ///         programname [--optionname ["option parameters"]] [-optionchar ["option parameters"]] parameter1 [parameter2
    ///         [parameter3 [...]]]<br /> the proper sequence is only needed with parameters (often used for files)
    ///     </p>
    /// </summary>
    public sealed class Arguments : IEquatable<Arguments>
    {
        #region static functionality

        /// <summary>Creates an <see cref="Arguments" /> instance from the environment.</summary>
        /// <returns>Returns a new <see cref="Arguments" /> instance.</returns>
        public static Arguments FromEnvironment()
        {
            var result = new Arguments();
            var cmdLine = Environment.CommandLine.ReplaceChars("\r\n", " ");
            result.ReadFromString(cmdLine, ParseOptions.AllowMissingPrefix | ParseOptions.ContainsCommand);
            return result;
        }

        /// <summary>Creates an <see cref="Arguments" /> instance from the specified string array.</summary>
        /// <param name="args">The string array containing the parameters and options.</param>
        /// <returns>Returns a new <see cref="Arguments" /> instance.</returns>
        public static Arguments FromArray(params string[] args) => FromArray(ParseOptions.AllowMissingPrefix, args);

        /// <summary>Creates an <see cref="Arguments" /> instance from the specified string array.</summary>
        /// <param name="opt">Options for the parser.</param>
        /// <param name="args">The string array containing the parameters and options.</param>
        /// <returns>Returns a new <see cref="Arguments" /> instance.</returns>
        public static Arguments FromArray(ParseOptions opt, params string[] args)
        {
            var result = new Arguments();
            result.ReadFromArray(args, opt);
            return result;
        }

        /// <summary>Creates an <see cref="Arguments" /> instance from the specified commandline string.</summary>
        /// <param name="opt">Options for the parser.</param>
        /// <param name="cmdLine">The string containing the full commandline.</param>
        /// <exception cref="ArgumentException">Thrown if single and double quotes are used at the commandline simultaneously.</exception>
        /// <returns>Returns a new <see cref="Arguments" /> instance.</returns>
        public static Arguments FromString(ParseOptions opt, string cmdLine)
        {
            if (cmdLine is null) throw new ArgumentNullException(nameof(cmdLine));
            var result = new Arguments();
            result.ReadFromString(cmdLine, opt);
            return result;
        }

        /// <summary>Creates an <see cref="Arguments" /> instance from the specified commandline string.</summary>
        /// <param name="cmdLine">The string containing the full commandline.</param>
        /// <exception cref="ArgumentException">Thrown if single and double quotes are used at the commandline simultaneously.</exception>
        /// <returns>Returns a new <see cref="Arguments" /> instance.</returns>
        public static Arguments FromString(string cmdLine) => FromString(ParseOptions.AllowMissingPrefix | ParseOptions.ContainsCommand, cmdLine);

        #endregion

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="Arguments" /> class.</summary>
        Arguments() { }

        #endregion

        /// <summary>Provides options for the parser.</summary>
        [Flags]
        public enum ParseOptions
        {
            /// <summary>No options</summary>
            None = 0,

            /// <summary>Given arguments contain the command (and parameters and options)</summary>
            ContainsCommand = 0x01,

            /// <summary>Options are options even if the prefix is missing (--, /)</summary>
            AllowMissingPrefix = 0x02
        }

        #region protected functionality

        /// <summary>Reads the parameters and options from a commandline string.</summary>
        /// <param name="text">The commandline string containing the command, parameters and options.</param>
        /// <param name="opt">Options for the parser.</param>
        /// <exception cref="ArgumentException">
        ///     Thrown if single and double quotes are used at the commandline simultaneously or
        ///     the specified string cannot be parsed correctly.
        /// </exception>
        void ReadFromString(string text, ParseOptions opt)
        {
            var stringBuilder = new StringBuilder();
            var strings = new List<string>();
            var inString = new Stack<char>();
            for (var i = 0; i < text.Length; i++)
            {
                switch (text[i])
                {
                    case '\'':
                    case '"':
                        if (inString.Count > 0)
                        {
                            if (inString.Peek() == text[i])
                            {
                                var c = inString.Pop();
                                if (inString.Count > 0)
                                {
                                    stringBuilder.Append(c);
                                }

                                continue;
                            }

                            stringBuilder.Append(text[i]);
                        }

                        inString.Push(text[i]);
                        continue;
                    case ' ':
                        if (inString.Count > 0)
                        {
                            stringBuilder.Append(text[i]);
                        }
                        else
                        {
                            strings.Add(stringBuilder.ToString());
                            stringBuilder = new StringBuilder();
                        }

                        continue;
                    default:
                        stringBuilder.Append(text[i]);
                        continue;
                }
            }

            if (inString.Count > 0)
            {
                throw new ArgumentException("Invalid string end mark (quotes mismatch)!");
            }

            if (stringBuilder.Length > 0)
            {
                strings.Add(stringBuilder.ToString());
            }

            ReadFromArray(strings.ToArray(), opt);
        }

        /// <summary>Reads the parameters and options from an array of parameters / options.</summary>
        /// <param name="args">The parameters and options.</param>
        /// <param name="opt">Options for the parser.</param>
        void ReadFromArray(string[] args, ParseOptions opt)
        {
            var pars = new List<string>();
            var opts = new List<Option>();
            Command = string.Empty;
            var index = 0;
            while (index < args.Length)
            {
                var current = args[index++].UnboxText(false);
                if ((index == 1) && ((opt & ParseOptions.ContainsCommand) != 0))
                {
                    // got command ?
                    Command = current;
                    continue;
                }

                if (Option.IsOption(current, (opt & ParseOptions.AllowMissingPrefix) != 0))
                {
                    // check options
                    if (current.Length > 0)
                    {
                        opts.Add(Option.Parse(current));
                    }
                }
                else
                {
                    if (current.Length > 0)
                    {
                        pars.Add(current.UnboxText(false));
                    }
                }
            }

            Options = new OptionCollection(opts);
            Parameters = new ParameterCollection(pars.ToArray());
        }

        #endregion

        #region public properties

        /// <summary>Gets all <see cref="Option" />s found.</summary>
        public OptionCollection Options { get; private set; }

        /// <summary>Gets all parameters found.</summary>
        public ParameterCollection Parameters { get; private set; }

        /// <summary>Gets the command used to start the program. This may be null or empty if not set!.</summary>
        public string Command { get; private set; }

        /// <summary>Gets the number of parameters and options used. The command counts too, if it is set (not null or empty)!.</summary>
        public int Count
        {
            get
            {
                var count = Parameters.Count + Options.Count;
                if (!string.IsNullOrEmpty(Command))
                {
                    count++;
                }

                return count;
            }
        }

#if NET20 || NET35 || NET40 || NET45 || NET46 || NET47 || NETSTANDARD20
        /// <summary>Gets a <see cref="ProcessStartInfo" />.</summary>
        public ProcessStartInfo ProcessStartInfo => new ProcessStartInfo(Command, ToString(false));
#else
#error No code defined for the current framework or NETXX version define missing!
#endif

        #endregion

        #region public functionality

        /// <summary>Gets a list with invalid parameters by checking all paramters against a list with valid ones.</summary>
        /// <param name="textComparison">Provides the used <see cref="StringComparison" />.</param>
        /// <param name="validParams">The list of valid paramters.</param>
        /// <returns>Returns an string[] containing all invalid <see cref="Parameters" />.</returns>
        public ParameterCollection GetInvalidParameters(StringComparison textComparison, params string[] validParams)
        {
            if (validParams == null)
            {
                throw new ArgumentNullException(nameof(validParams));
            }

            var pars = new List<string>();
            foreach (var parameter in Parameters)
            {
                var equals = false;
                foreach (var validParameter in validParams)
                {
                    if (string.Equals(validParameter, parameter, textComparison))
                    {
                        equals = true;
                        break;
                    }
                }

                if (!equals)
                {
                    pars.Add(parameter);
                }
            }

            var result = new ParameterCollection(pars.ToArray());
            return result;
        }

        /// <summary>Gets a list with invalid parameters by checking all paramters against a list with valid ones.</summary>
        /// <param name="validParams">The list of valid paramters.</param>
        /// <returns>Returns an string[] containing all invalid <see cref="Parameters" />.</returns>
        public ParameterCollection GetInvalidParameters(params string[] validParams) =>
            GetInvalidParameters(StringComparison.CurrentCultureIgnoreCase, validParams);

        /// <summary>Gets a list with invalid options by checking all options against a list with valid ones.</summary>
        /// <param name="textComparison">Provides the used <see cref="StringComparison" />.</param>
        /// <param name="validOpts">The list of valid options.</param>
        /// <returns>Returns an <see cref="OptionCollection" /> containing all invalid <see cref="Options" />.</returns>
        public OptionCollection GetInvalidOptions(StringComparison textComparison, params string[] validOpts)
        {
            if (validOpts == null)
            {
                throw new ArgumentNullException(nameof(validOpts));
            }

            var opts = new List<Option>();
            foreach (var option in Options)
            {
                var isEqual = false;
                foreach (var validOption in validOpts)
                {
                    if (string.Equals(validOption, option.Name, textComparison))
                    {
                        isEqual = true;
                        break;
                    }
                }

                if (!isEqual)
                {
                    opts.Add(option);
                }
            }

            var result = new OptionCollection(opts);
            return result;
        }

        /// <summary>
        ///     Gets a list with invalid options by checking all options against a list with valid ones. This function uses
        ///     <see cref="StringComparison.CurrentCultureIgnoreCase" />!.
        /// </summary>
        /// <param name="validOpts">The list of valid options.</param>
        /// <returns>Returns an <see cref="OptionCollection" /> containing all invalid <see cref="Options" />.</returns>
        public OptionCollection GetInvalidOptions(params string[] validOpts) => GetInvalidOptions(StringComparison.CurrentCultureIgnoreCase, validOpts);

        /// <summary>
        ///     Checks whether a specified parameter is present or not. This function uses
        ///     <see cref="StringComparison.InvariantCultureIgnoreCase" />.
        /// </summary>
        /// <param name="paramName">The parameter to search.</param>
        /// <returns>Returns true if the specified parameter is present.</returns>
        public bool IsParameterPresent(string paramName) => IsParameterPresent(paramName, StringComparison.OrdinalIgnoreCase);

        /// <summary>Check whether a specified parameter is present or not.</summary>
        /// <param name="paramName">The parameter to search.</param>
        /// <param name="textComparison">Provides the used <see cref="StringComparison" />.</param>
        /// <returns>Returns true if the specified parameter is present.</returns>
        public bool IsParameterPresent(string paramName, StringComparison textComparison)
        {
            foreach (var parameter in Parameters)
            {
                if (string.Equals(paramName, parameter, textComparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Checks whether all specified options are present. This function uses
        ///     <see cref="StringComparison.InvariantCultureIgnoreCase" />.
        /// </summary>
        /// <param name="optionNames">The options to search.</param>
        /// <returns>Returns true if options are present, false otherwise.</returns>
        public bool AreOptionsPresent(params string[] optionNames)
        {
            if (optionNames == null)
            {
                throw new ArgumentNullException(nameof(optionNames));
            }

            foreach (var option in optionNames)
            {
                if (!IsOptionPresent(option, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Checks for presence of an option. This function uses
        ///     <see cref="StringComparison.InvariantCultureIgnoreCase" />.
        /// </summary>
        /// <param name="optionName">The option to search.</param>
        /// <returns>Returns true if the specified option is present.</returns>
        public bool IsOptionPresent(string optionName) => IsOptionPresent(optionName, StringComparison.OrdinalIgnoreCase);

        /// <summary>
        ///     Checks for presence of an option. This function uses
        ///     <see cref="StringComparison.InvariantCultureIgnoreCase" />.
        /// </summary>
        /// <param name="optionName">The option to search.</param>
        /// <param name="textComparison">StringComparison to use.</param>
        /// <returns>Returns true if the specified option is present.</returns>
        public bool IsOptionPresent(string optionName, StringComparison textComparison)
        {
            foreach (var option in Options)
            {
                if (string.Equals(option.Name, optionName, textComparison))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Gets the first matching option or null.</summary>
        /// <param name="optionNames">Option name to retrieve.</param>
        /// <returns>Returns a matching option instance or null.</returns>
        public Option GetFirstOption(params string[] optionNames)
        {
            foreach (var name in optionNames)
            {
                if (Options.Contains(name))
                {
                    return Options[name];
                }
            }

            return null;
        }

        /// <summary>Gets the value of the first matching option or null.</summary>
        /// <param name="optionNames">Option name to retrieve.</param>
        /// <returns>Returns the option value or null.</returns>
        public string GetFirstOptionValue(params string[] optionNames) => GetFirstOption(optionNames)?.Value;

        /// <summary>Checks for presence of any (/help -help --help, /h, -h, /?, -?, ...) known help options.</summary>
        /// <returns>Returns true if any help option is found.</returns>
        public bool IsHelpOptionFound() =>
            IsOptionPresent("help", StringComparison.OrdinalIgnoreCase) ||
            IsOptionPresent("?", StringComparison.OrdinalIgnoreCase) ||
            IsOptionPresent("h", StringComparison.OrdinalIgnoreCase);

        #endregion

        /// <summary>Gets a string containing the <see cref="Command" />, <see cref="Parameters" /> and <see cref="Options" />.</summary>
        /// <returns>
        ///     A string containing the <see cref="Arguments" /> in the form "Command [Parameter1 [Parameter2 ..]] [-Option1
        ///     [-Option2 ..]]".
        /// </returns>
        public override string ToString() => ToString(true);

        /// <summary>
        ///     Gets a string containing the <see cref="Command" /> (optional), <see cref="Parameters" /> and
        ///     <see cref="Options" />.
        /// </summary>
        /// <param name="includeCmd">Set to true to include the optional <see cref="Command" /> part.</param>
        /// <returns>
        ///     Returns a string containing the <see cref="Arguments" /> in the form "Command [Parameter1 [Parameter2 ..]]
        ///     [-Option1 [-Option2 ..]]".
        /// </returns>
        public string ToString(bool includeCmd)
        {
            var result = new StringBuilder();
            if (includeCmd)
            {
                var containsSpace = Command.IndexOf(' ') >= 0;
                if (containsSpace)
                {
                    result.Append('"');
                }

                result.Append(Command);
                if (containsSpace)
                {
                    result.Append('"');
                }
            }

            var str = Parameters.ToString();
            if (str.Length > 0)
            {
                if (result.Length > 0)
                {
                    result.Append(' ');
                }

                result.Append(str);
            }

            str = Options.ToString();
            if (str.Length > 0)
            {
                if (result.Length > 0)
                {
                    result.Append(' ');
                }

                result.Append(str);
            }

            return result.ToString();
        }

        /// <summary>Gets a value indicating whether these arguments are empty or not.</summary>
        public bool IsEmpty => string.IsNullOrEmpty(Command) && ((Parameters.Count + Options.Count) == 0);

        /// <summary>
        ///     Gets a string array containing the <see cref="Command" /> (first element), <see cref="Parameters" /> and at
        ///     last the <see cref="Options" />.
        /// </summary>
        /// <returns>
        ///     A string array containing the <see cref="Arguments" /> in the form "Command (first element), [Parameter1,
        ///     [Parameter2, ..]], and at last [-Option1, [-Option2, ..]]".
        /// </returns>
        public string[] ToArray() => ToArray(true);

        /// <summary>
        ///     Gets a string array containing the <see cref="Command" /> (first element, optional), <see cref="Parameters" />
        ///     and at last the <see cref="Options" />.
        /// </summary>
        /// <param name="includeCmd">Set to true to include the optional <see cref="Command" /> part.</param>
        /// <returns>
        ///     A string array containing the <see cref="Arguments" /> in the form "Command (first element, optional),
        ///     [Parameter1, [Parameter2, ..]], and at last [-Option1, [-Option2, ..]]".
        /// </returns>
        public string[] ToArray(bool includeCmd)
        {
            var result = new List<string>();
            if (includeCmd)
            {
                if (Command.IndexOf(' ') >= 0)
                {
                    result.Add("\"" + Command + "\"");
                }
                else
                {
                    result.Add(Command);
                }
            }

            foreach (var parameter in Parameters)
            {
                if (parameter.IndexOf(' ') >= 0)
                {
                    result.Add("\"" + parameter + "\"");
                }
                else
                {
                    result.Add(parameter);
                }
            }

            foreach (var option in Options)
            {
                result.Add(option.ToString());
            }

            return result.ToArray();
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">Object to check for equality.</param>
        /// <returns>Returns true if the specified instance equals this.</returns>
        public override bool Equals(object obj) => Equals(obj as Arguments);

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="other">Arguments instance to check against.</param>
        /// <returns>Returns true if the specified instance equals this.</returns>
        public bool Equals(Arguments other) =>
            !(other is null)
         && (Count == other.Count)
         && (Command == other.Command)
         && Options.Equals(other.Options)
         && Parameters.Equals(other.Parameters);

        /// <summary>Gets the hash code for this instance.</summary>
        /// <returns>Returns a hash code.</returns>
        public override int GetHashCode() => Command.GetHashCode() ^ Options.GetHashCode() ^ Parameters.GetHashCode();
    }
}
