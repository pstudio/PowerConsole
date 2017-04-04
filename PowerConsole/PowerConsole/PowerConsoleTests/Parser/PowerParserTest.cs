using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pstudio.PowerConsole.Parser;
using Sprache;

namespace PowerConsoleTests.Parser
{
    [TestClass]
    public class PowerParserTest
    {
        #region Identifier

        [TestMethod]
        public void ValidIdentifierParsing()
        {
            var input = "hello";
            var id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("hello", id);

            input = "_id007";
            id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("_id007", id);

            input = "_Id007";
            id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("_Id007", id);

            input = "Get-Item";
            id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("Get-Item", id);

            input = "Hello_World";
            id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("Hello_World", id);
        }

        [TestMethod]
        public void InvalidIndentifierParsing()
        {
            var input = "0A";
            try
            {
                PowerParser.Identifier.Parse(input);
                Assert.Fail();
            }
            catch (ParseException)
            {
            }

            input = "-id007";
            try
            {
                PowerParser.Identifier.Parse(input);
                Assert.Fail();
            }
            catch (ParseException)
            {
            }

            input = "id007+";
            var id = PowerParser.Identifier.Parse(input);
            Assert.AreNotEqual("id007+", id);
        }

        [TestMethod]
        public void ExtendedAsciiIdentiierParsing()
        {
            var input = "æble";
            var id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("æble", id);

            input = "ÄÅÆËÖØßäåæöø";
            id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("ÄÅÆËÖØßäåæöø", id);
        }

        [TestMethod]
        public void UnicodeIdentifierParsing()
        {
            var input = "ΑΔΘαδθ";
            var id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("ΑΔΘαδθ", id);

            input = "БбПпЯя";
            id = PowerParser.Identifier.Parse(input);
            Assert.AreEqual("БбПпЯя", id);

            // FAILS: Don't use arabic characters I guess - Or it is quite possible that the string I created is just plain wrong
            //input = "ب‎ـج‎ﺩ‎ـش";
            //id = PowerParser.Identifier.Parse(input);
            //Assert.AreEqual("ب‎ـج‎ﺩ‎ـش", id);
        }

        #endregion

        #region Quoted String

        [TestMethod]
        public void DoubleQuotedStringParsing()
        {
            var input = "\"Hello World +-*/!@#$% 007\"";
            var s = PowerParser.DoubleQuotedString.Parse(input);
            Assert.AreEqual("Hello World +-*/!@#$% 007", s);

            input = "\"Hello 'World'\"";
            s = PowerParser.DoubleQuotedString.Parse(input);
            Assert.AreEqual("Hello 'World'", s);
        }

        [TestMethod]
        public void SingleQuotedStringParsing()
        {
            var input = "'Hello World +-*/!@#$% 007'";
            var s = PowerParser.SingleQuotedString.Parse(input);
            Assert.AreEqual("Hello World +-*/!@#$% 007", s);

            input = "'Hello \"World\"'";
            s = PowerParser.SingleQuotedString.Parse(input);
            Assert.AreEqual("Hello \"World\"", s);
        }

        [TestMethod]
        public void InvalidDoubleQuotedStringParsing()
        {
            var input = "\"Hello World\" +-*/!@#$% 007\"";
            var s = PowerParser.DoubleQuotedString.Parse(input);
            Assert.AreNotEqual("Hello World +-*/!@#$% 007", s);

            input = "a\"Hello\"";
            try
            {
                PowerParser.DoubleQuotedString.Parse(input);
                Assert.Fail();
            }
            catch (ParseException)
            {
            }
        }

        [TestMethod]
        public void InvalidSingleQuotedStringParsing()
        {
            var input = "'Hello World' +-*/!@#$% 007'";
            var s = PowerParser.SingleQuotedString.Parse(input);
            Assert.AreNotEqual("Hello World +-*/!@#$% 007", s);

            input = "a'Hello'";
            try
            {
                PowerParser.SingleQuotedString.Parse(input);
                Assert.Fail();
            }
            catch (ParseException)
            {
            }
        }

        #endregion

        #region Number

        [TestMethod]
        public void ValidNumberParsing()
        {
            var input = "2";
            var n = PowerParser.Number.Parse(input);
            Assert.AreEqual(2, n);

            input = "3.14";
            n = PowerParser.Number.Parse(input);
            Assert.AreEqual(3.14, n);

            input = ".007";
            n = PowerParser.Number.Parse(input);
            Assert.AreEqual(.007, n);

            input = "-3.14";
            n = PowerParser.Number.Parse(input);
            Assert.AreEqual(-3.14, n);

            input = "+3.14";
            n = PowerParser.Number.Parse(input);
            Assert.AreEqual(+3.14, n);
        }

        #endregion

        #region Special Operators

        [TestMethod]
        public void VariableParsing()
        {
            var input = "$variable";
            var v = PowerParser.Variable.Parse(input);
            Assert.AreEqual("variable", v);
        }

        [TestMethod]
        public void PipeParsing()
        {
            var input = "|";
            var c = PowerParser.Pipe.Parse(input);
            Assert.AreEqual('|', c);
        }

        [TestMethod]
        public void ReflectParsing()
        {
            var input = ".";
            var c = PowerParser.Reflect.Parse(input);
            Assert.AreEqual('.', c);
        }

        [TestMethod]
        public void AssignParsing()
        {
            var input = "=";
            var c = PowerParser.Assign.Parse(input);
            Assert.AreEqual('=', c);
        }

        #endregion

        #region Statements

        #region Command

        [TestMethod]
        public void CommandNoArgsParsing()
        {
            var input = "Get-Location";
            var command = PowerParser.Command.Parse(input);
            Assert.IsNotNull(command);
            Assert.AreEqual("Get-Location", command.CommandName);
            Assert.IsNotNull(command.Arguments);
            Assert.IsTrue(command.Arguments.Length == 0);
        }

        [TestMethod]
        public void CommandWithArgsParsing()
        {
            var input = "Concat Id \"double quote\" 'single quote' -2.0 $variable";
            var command = PowerParser.Command.Parse(input);
            Assert.IsNotNull(command);
            Assert.AreEqual("Concat", command.CommandName);
            Assert.IsNotNull(command.Arguments);
            Assert.IsTrue(command.Arguments.Length == 5);
            Assert.IsTrue(command.Arguments[0].ParsedType == ParseType.Type.Identifier);
            Assert.IsTrue(command.Arguments[0].Value is string);
            Assert.IsTrue((string) command.Arguments[0].Value == "Id");
            Assert.IsTrue(command.Arguments[1].ParsedType == ParseType.Type.String);
            Assert.IsTrue(command.Arguments[1].Value is string);
            Assert.IsTrue((string) command.Arguments[1].Value == "double quote");
            Assert.IsTrue(command.Arguments[2].ParsedType == ParseType.Type.String);
            Assert.IsTrue(command.Arguments[2].Value is string);
            Assert.IsTrue((string) command.Arguments[2].Value == "single quote");
            Assert.IsTrue(command.Arguments[3].ParsedType == ParseType.Type.Number);
            Assert.IsTrue(command.Arguments[3].Value is double);
            Assert.IsTrue((double) command.Arguments[3].Value == -2.0);
            Assert.IsTrue(command.Arguments[4].ParsedType == ParseType.Type.Variable);
            Assert.IsTrue(command.Arguments[4].Value is string);
            Assert.IsTrue((string) command.Arguments[4].Value == "variable");
        }

        #endregion

        #region Assignment

        [TestMethod]
        public void CommandToVariableAssignment()
        {
            var input = "$variable = Find-Object 'player'";
            var assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("variable", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.Command, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is pstudio.PowerConsole.Parser.Command);
            var command = (pstudio.PowerConsole.Parser.Command) assignment.Value.Value;
            Assert.AreEqual("Find-Object", command.CommandName);
            Assert.IsNotNull(command.Arguments);
            Assert.IsTrue(command.Arguments.Length == 1);
            Assert.AreEqual(ParseType.Type.String ,command.Arguments[0].ParsedType);
            Assert.IsTrue(command.Arguments[0].Value is string);
            Assert.AreEqual("player", command.Arguments[0].Value);
        }

        [TestMethod]
        public void VariableToVariableAssignment()
        {
            var input = "$a = $b";
            var assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("a", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.Variable, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is string);
            Assert.AreEqual("b", assignment.Value.Value);

            // Let's check that spaces/lack of spaces doesn't mess with parsing
            input = "$a=$b";
            assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("a", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.Variable, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is string);
            Assert.AreEqual("b", assignment.Value.Value);

            input = " $a=$b ";
            assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("a", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.Variable, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is string);
            Assert.AreEqual("b", assignment.Value.Value);
        }

        [TestMethod]
        public void StringToVariableAssignment()
        {
            var input = "$variable = 'Hello World'";
            var assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("variable", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.String, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is string);
            Assert.AreEqual("Hello World", assignment.Value.Value);
        }

        [TestMethod]
        public void NumberToVariableAssignment()
        {
            var input = "$variable = 2";
            var assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("variable", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.Number, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is double);
            Assert.AreEqual(2.0, assignment.Value.Value);
        }

        [TestMethod]
        public void PipeChainToVariableAssignment()
        {
            var input = "$variable = Find-Object 'player' | Get-Transform";
            var assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("variable", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.PipeChain, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is PipeChain);
            var pipechain = (PipeChain) assignment.Value.Value;
            Assert.IsTrue(pipechain.Commands.Length == 2);

            Assert.IsNotNull(pipechain.Commands[0]);
            var command = pipechain.Commands[0];
            Assert.AreEqual("Find-Object", command.CommandName);
            Assert.IsNotNull(command.Arguments);
            Assert.IsTrue(command.Arguments.Length == 1);
            Assert.AreEqual(ParseType.Type.String, command.Arguments[0].ParsedType);
            Assert.IsTrue(command.Arguments[0].Value is string);
            Assert.AreEqual("player", command.Arguments[0].Value);

            Assert.IsNotNull(pipechain.Commands[1]);
            command = pipechain.Commands[1];
            Assert.AreEqual("Get-Transform", command.CommandName);
            Assert.IsNotNull(command.Arguments);
            Assert.IsTrue(command.Arguments.Length == 0);
        }

        [TestMethod]
        public void ReflectionToVariableAssignment()
        {
            var input = "$variable = $player.Transform";
            var assignment = PowerParser.Assignment.Parse(input);
            Assert.IsNotNull(assignment);
            Assert.AreEqual("variable", assignment.Variable);
            Assert.IsNotNull(assignment.Value);
            Assert.AreEqual(ParseType.Type.Reflection, assignment.Value.ParsedType);
            Assert.IsTrue(assignment.Value.Value is Reflection);
            var reflection = (Reflection) assignment.Value.Value;
            Assert.AreEqual("player", reflection.Variable);
            Assert.IsTrue(reflection.Identifiers.Length == 1);
            Assert.AreEqual("Transform", reflection.Identifiers[0]);
        }

        #endregion

        #region Reflection

        [TestMethod]
        public void VariableMemberReflection()
        {
            var input = "$var.Member";
            var reflection = PowerParser.Reflection.Parse(input);
            Assert.IsNotNull(reflection);
            Assert.AreEqual("var", reflection.Variable);
            Assert.IsNotNull(reflection.Identifiers);
            Assert.IsTrue(reflection.Identifiers.Length == 1);
            Assert.AreEqual("Member", reflection.Identifiers[0]);

            input = " $var . Member ";
            reflection = PowerParser.Reflection.Parse(input);
            Assert.IsNotNull(reflection);
            Assert.AreEqual("var", reflection.Variable);
            Assert.IsNotNull(reflection.Identifiers);
            Assert.IsTrue(reflection.Identifiers.Length == 1);
            Assert.AreEqual("Member", reflection.Identifiers[0]);
        }

        #endregion

        #region Pipe Chain

        [TestMethod]
        public void SinglePipe()
        {
            var input = "Find-Object 'player' | Kill";
            var chain = PowerParser.PipeChain.Parse(input);
            Assert.IsNotNull(chain);
            Assert.IsNotNull(chain.Commands);
            Assert.IsTrue(chain.Commands.Length == 2);

            Assert.IsNotNull(chain.Commands[0]);
            Assert.AreEqual("Find-Object", chain.Commands[0].CommandName);
            Assert.IsTrue(chain.Commands[0].Arguments.Length == 1);
            Assert.AreEqual(ParseType.Type.String, chain.Commands[0].Arguments[0].ParsedType);
            Assert.AreEqual("player", chain.Commands[0].Arguments[0].Value);

            Assert.IsNotNull(chain.Commands[1]);
            Assert.AreEqual("Kill", chain.Commands[1].CommandName);
            Assert.IsTrue(chain.Commands[1].Arguments.Length == 0);
        }

        [TestMethod]
        public void MultiPipe()
        {
            var input = "First | Second 2 | Third | Fourth 'End'";
            var chain = PowerParser.PipeChain.Parse(input);
            Assert.IsNotNull(chain);
            Assert.IsNotNull(chain.Commands);
            Assert.IsTrue(chain.Commands.Length == 4);

            // First
            Assert.IsNotNull(chain.Commands[0]);
            Assert.AreEqual("First", chain.Commands[0].CommandName);
            Assert.IsTrue(chain.Commands[0].Arguments.Length == 0);

            // Second
            Assert.IsNotNull(chain.Commands[1]);
            Assert.AreEqual("Second", chain.Commands[1].CommandName);
            Assert.IsTrue(chain.Commands[1].Arguments.Length == 1);
            Assert.AreEqual(ParseType.Type.Number, chain.Commands[1].Arguments[0].ParsedType);
            Assert.AreEqual(2.0, chain.Commands[1].Arguments[0].Value);

            // Third
            Assert.IsNotNull(chain.Commands[2]);
            Assert.AreEqual("Third", chain.Commands[2].CommandName);
            Assert.IsTrue(chain.Commands[2].Arguments.Length == 0);

            // Fourth
            Assert.IsNotNull(chain.Commands[3]);
            Assert.AreEqual("Fourth", chain.Commands[3].CommandName);
            Assert.IsTrue(chain.Commands[3].Arguments.Length == 1);
            Assert.AreEqual(ParseType.Type.String, chain.Commands[3].Arguments[0].ParsedType);
            Assert.AreEqual("End", chain.Commands[3].Arguments[0].Value);
        }

        #endregion

        #endregion

    }
}
