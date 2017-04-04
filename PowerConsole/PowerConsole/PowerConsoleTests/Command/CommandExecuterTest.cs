using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using pstudio.PowerConsole.Command;
using pstudio.PowerConsole.Command.Commands.Math;
using pstudio.PowerConsole.Host;
using pstudio.PowerConsole.Parser;

namespace PowerConsoleTests.Command
{
    [TestClass]
    public class CommandExecuteTest
    {
        private CommandContext _context;
        private IHost _host;
        private Dictionary<string, object> _variables;

        [TestInitialize]
        public void Initialize()
        {
            _context = new CommandContext();
            _host = Substitute.For<IHost>();
            _variables = new Dictionary<string, object>();
        }

        [TestMethod]
        public void PositionalCommandExecuted()
        {
            _context.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 5.0");

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual(7.0, result);
        }

        [TestMethod]
        public void PositionalCommandVariableArgumentExecuted()
        {
            _context.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 $var");
            _variables["var"] = 5.0;

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual(7.0, result);
        }

        [TestMethod]
        public void PositionalCommandMissingArgumentExecuted()
        {
            _context.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 ");

            try
            {
                var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);
                Assert.Fail();
            }
            catch (MissingPositionalArgumentException)
            {
                
            }
        }

        [TestMethod]
        public void PositionalCommandIncorrectPositionalArgumentTypeExecuted()
        {
            _context.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 '2'");

            try
            {
                var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);
                Assert.Fail();
            }
            catch (InvalidArgumentTypeException)
            {

            }
        }

        [TestMethod]
        public void NamedCommandExecuted()
        {
            _context.RegisterCommand<TestCommand>();
            var parseResult = PowerParser.ParseInput("Test-Command \"Hello World\" -SubstringIndex 6");

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual("World", result);
        }

        [TestMethod]
        public void CommandStateResetWhenExecuted()
        {
            _context.RegisterCommand<TestCommand>();
            var parseResult = PowerParser.ParseInput("Test-Command \"Hello World\" -SubstringIndex 6");

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual("World", result);

            parseResult = PowerParser.ParseInput("Test-Command \"Hello World\"");
            result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void MandatoryCommandExecuted()
        {
            _context.RegisterCommand<MandatoryNamedCommand>();
            var parseResult = PowerParser.ParseInput("Mandatory-Named -Message 'Goodbye'");
            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual("Hello World", result);

            parseResult = PowerParser.ParseInput("Mandatory-Named -Message 'Goodbye' -Flag");
            result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host, _variables);

            Assert.AreEqual("Goodbye", result);
        }

        [Command("Test", "Command")]
        public class TestCommand : pstudio.PowerConsole.Command.Command
        {
            [Parameter(Position = 0)]
            public string Message { get; set; }

            [Parameter]
            public int SubstringIndex { get; set; }

            public override object Process()
            {
                return Message.Substring(SubstringIndex);
            }
        }

        [Command("Mandatory", "Named")]
        public class MandatoryNamedCommand : pstudio.PowerConsole.Command.Command
        {
            [Parameter]
            public bool Flag { get; set; }

            [Parameter(Mandatory = true)]
            public string Message { get; set; }

            public override object Process()
            {
                return Flag ? Message : "Hello World";
            }
        }
    }

}
