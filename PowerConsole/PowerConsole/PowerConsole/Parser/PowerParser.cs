﻿using System;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using Sprache;

namespace pstudio.PowerConsole.Parser
{

    internal static class PowerParser
    {
        /// <summary>
        /// Parses an identifier.
        /// Grammar: [_Letter][_-LetterOrDigit]*
        /// </summary>
        public static readonly Parser<string> Identifier =
            from first in Parse.Letter.XOr(Parse.Char('_')).Once()
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('_')).XOr(Parse.Char('-')).Many()
            select new string(first.Concat(rest).ToArray());

        /// <summary>
        /// Parses a double quoted text string.
        /// Grammar: "[Char]*"
        /// </summary>
        public static readonly Parser<string> DoubleQuotedString =
            from open in Parse.Char('"')
            from content in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"')
            select content;

        /// <summary>
        /// Parses a single quoted text string.
        /// Grammar: '[Char]*'
        /// </summary>
        public static readonly Parser<string> SingleQuotedString =
            from open in Parse.Char('\'')
            from content in Parse.CharExcept('\'').Many().Text()
            from close in Parse.Char('\'')
            select content;

        /// <summary>
        /// Parses a single or double quoted text string.
        /// Grammar: ("[Char]*"|'[Char]*')
        /// </summary>
        public static readonly Parser<string> QuotedString = DoubleQuotedString.XOr(SingleQuotedString);

        public static readonly Parser<double> NegativeNumber =
            Parse.Char('-')
                .Then(_ => Parse.DecimalInvariant)
                .Select(n => Double.Parse("-" + n, CultureInfo.InvariantCulture));

        public static readonly Parser<double> PositiveNumber =
            Parse.Char('+').Then(_ => Parse.DecimalInvariant).Select(n => Double.Parse(n, CultureInfo.InvariantCulture));

        public static readonly Parser<double> UnsignedNumber =
            Parse.DecimalInvariant.Select(n => Double.Parse(n, CultureInfo.InvariantCulture));

        /// <summary>
        /// Parses a decimal number.
        /// Grammar: (-+)?[0-9]*(.[0-9]*)?
        /// </summary>
        public static readonly Parser<double> Number =
            NegativeNumber.XOr(PositiveNumber).XOr(UnsignedNumber);
            //from sign in Parse.Char('-').Or(Parse.Char('+')).Optional()
            //from number in Parse.DecimalInvariant
            //select double.Parse(sign.IsDefined ? sign.Get() + number : number, CultureInfo.InvariantCulture);


            //Parse.DecimalInvariant.Select(n => double.Parse(n, CultureInfo.InvariantCulture));
            //Parse.Char('-').XOr(Parse.Char('+')).Optional().Then(op => Parse.DecimalInvariant.Select(n => double.Parse(op.IsDefined ? op.Get() + n : n, CultureInfo.InvariantCulture)));

            /// <summary>
            /// Parses the pipe operator.
            /// Grammar: |
            /// </summary>
        public static readonly Parser<char> Pipe = Parse.Char('|');

        /// <summary>
        /// Parses the reflect operator.
        /// Grammar: .
        /// </summary>
        public static readonly Parser<char> Reflect = Parse.Char('.'); 

        /// <summary>
        /// Parses the assignment operator.
        /// Grammar: =
        /// </summary>
        public static readonly Parser<char> Assign = Parse.Char('=');

        /// <summary>
        /// Parses a variable.
        /// Grammar: $Identifier
        /// </summary>
        public static readonly Parser<string> Variable = Parse.Char('$').Then(_ => Identifier);

        /// <summary>
        /// Parses a parameter identifier.
        /// Grammar: -Identifier
        /// </summary>
        public static readonly Parser<string> Parameter = Parse.Char('-').Then(_ => Identifier);

        /// <summary>
        /// Parses a reflection call.
        /// Grammar: Variable(.Identifier)+
        /// </summary>
        public static readonly Parser<Reflection> Reflection =
            from obj in
                //Variable.Token().Select(v => new ParseType(v, ParseType.Type.Variable))
                Variable.Token()
            //.XOr(Identifier) TODO: Maybe it will make sense to reflect on an identifier at some point
            from id in Reflect.Token().Then(_ => Identifier.Token()).AtLeastOnce()
            select new Reflection(obj, id.ToArray());

        /// <summary>
        /// Parses a command.
        /// Grammar: Identifier (Identifier | Parameter | Variable | QuotedString | Number)*
        /// </summary>
        public static readonly Parser<Command> Command =
            from com in Identifier.Token()
            from args in 
                Identifier.Token().Select(id => new ParseType(id, ParseType.Type.Identifier))
                .Or(Parameter.Token().Select(p => new ParseType(p, ParseType.Type.Parameter)))
                .Or(Variable.Token().Select(v => new ParseType(v, ParseType.Type.Variable)))
                .Or(QuotedString.Token().Select(s => new ParseType(s, ParseType.Type.String)))
                .Or(Number.Token().Select(n => new ParseType(n, ParseType.Type.Number)))
                .Many()
            select new Command(com, args.ToArray());

        /// <summary>
        /// Parses a pipe chain.
        /// Grammar: Command (| Command)+
        /// </summary>
        public static readonly Parser<PipeChain> PipeChain = 
            Command.Once()
            .Then(first => Pipe.Token()
            .Then(_ => Command)
            .AtLeastOnce()
            .Select(tail => new PipeChain(first.Concat(tail)
                .ToArray())));


        /// <summary>
        /// Parses an assignment.
        /// Grammar: Variable = (PipeChain | Reflection | Command | Variable | QuotedString | Number)
        /// </summary>
        public static readonly Parser<Assignment> Assignment =
            from var in Variable.Token()
            from eq in Assign.Token()
            from val in
                PipeChain.Token().Select(pipe => new ParseType(pipe, ParseType.Type.PipeChain))
                .Or(Reflection.Token().Select(refl => new ParseType(refl, ParseType.Type.Reflection)))
                .Or(Command.Token().Select(com => new ParseType(com, ParseType.Type.Command)))
                .Or(Variable.Token().Select(v => new ParseType(v, ParseType.Type.Variable)))
                .Or(QuotedString.Token().Select(s => new ParseType(s, ParseType.Type.String)))
                .Or(Number.Token().Select(n => new ParseType(n, ParseType.Type.Number)))
            select new Assignment(var, val);


        /// <summary>
        /// Parse a statement.
        /// Grammar: Assignment | PipeChain | Reflection | Command | Variable | QuotedString | Number
        /// </summary>
        public static readonly Parser<ParseType> Statement =
            Assignment.Token().Select(assignment => new ParseType(assignment, ParseType.Type.Assignment))
                .Or(PipeChain.Token().Select(pipe => new ParseType(pipe, ParseType.Type.PipeChain)))
                .Or(Reflection.Token().Select(refl => new ParseType(refl, ParseType.Type.Reflection)))
                .Or(Command.Token().Select(com => new ParseType(com, ParseType.Type.Command)))
                .Or(Variable.Token().Select(v => new ParseType(v, ParseType.Type.Variable)))
                .Or(QuotedString.Token().Select(s => new ParseType(s, ParseType.Type.String)))
                .Or(Number.Token().Select(n => new ParseType(n, ParseType.Type.Number)));

        /// <summary>
        /// Parse input.
        /// </summary>
        /// <param name="input">PowerConsole input</param>
        /// <returns>Parsed result</returns>
        /// <exception cref="ArgumentException">Thrown if input is null or empty.</exception>
        /// <exception cref="IncompleteParseException">Thrown if not the entire input is parsed.</exception>
        public static ParseType ParseInput(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input string is null or empty");

            var result = Statement.TryParse(input);
            if (result.Remainder.AtEnd)
                return result.Value;

            throw new IncompleteParseException("Could not parse the entire input", result.Remainder.Source, result.Remainder.Position);
        }

        public class IncompleteParseException : Exception
        {
            public string Input { get; }
            public int Position { get; }

            public IncompleteParseException(string input, int position)
            {
                Input = input;
                Position = position;
            }

            public IncompleteParseException(string message, string input, int position) : base(message)
            {
                Input = input;
                Position = position;
            }

            public IncompleteParseException(string message, Exception innerException, string input, int position) : base(message, innerException)
            {
                Input = input;
                Position = position;
            }

            protected IncompleteParseException(SerializationInfo info, StreamingContext context, string input, int position) : base(info, context)
            {
                Input = input;
                Position = position;
            }
        }
    }

}
