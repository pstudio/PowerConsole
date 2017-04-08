using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using pstudio.PowerConsole.Command;
using pstudio.PowerConsole.Command.Commands.Math;
using pstudio.PowerConsole.Context;
using pstudio.PowerConsole.Host;
using pstudio.PowerConsole.Parser;

namespace PowerConsoleTests.Command
{
    [TestClass]
    public class CommandExecuteTest
    {
        private IContext _context;
        private IHost _host;

        [TestInitialize]
        public void Initialize()
        {
            _context = new DefaultContext();
            _host = Substitute.For<IHost>();
        }

        [TestMethod]
        public void PositionalCommandExecuted()
        {
            _context.CommandContext.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 5.0");

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual(7.0, result);
        }

        [TestMethod]
        public void PositionalCommandVariableArgumentExecuted()
        {
            _context.CommandContext.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 $var");
            _context.VariableContext["var"] = 5.0;

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual(7.0, result);
        }

        [TestMethod]
        public void PositionalCommandMissingArgumentExecuted()
        {
            _context.CommandContext.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 ");

            try
            {
                var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);
                Assert.Fail();
            }
            catch (MissingMandatoryParameterException)
            {
                
            }
        }

        [TestMethod]
        public void PositionalCommandIncorrectPositionalArgumentTypeExecuted()
        {
            _context.CommandContext.RegisterCommand<AddNumberCommand>();
            var parseResult = PowerParser.ParseInput("Add-Number 2 '2'");

            try
            {
                var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);
                Assert.Fail();
            }
            catch (InvalidArgumentTypeException)
            {

            }
        }

        [TestMethod]
        public void NamedCommandExecuted()
        {
            _context.CommandContext.RegisterCommand<TestCommand>();
            var parseResult = PowerParser.ParseInput("Test-Command \"Hello World\" -SubstringIndex 6");

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual("World", result);
        }

        [TestMethod]
        public void CommandStateResetWhenExecuted()
        {
            _context.CommandContext.RegisterCommand<TestCommand>();
            var parseResult = PowerParser.ParseInput("Test-Command \"Hello World\" -SubstringIndex 6");

            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual("World", result);

            parseResult = PowerParser.ParseInput("Test-Command \"Hello World\"");
            result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual("Hello World", result);
        }

        [TestMethod]
        public void MandatoryCommandExecuted()
        {
            _context.CommandContext.RegisterCommand<MandatoryNamedCommand>();
            var parseResult = PowerParser.ParseInput("Mandatory-Named -Message 'Goodbye'");
            var result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual("Hello World", result);

            parseResult = PowerParser.ParseInput("Mandatory-Named -Message 'Goodbye' -Flag");
            result = CommandExecuter.Execute(parseResult.Value as pstudio.PowerConsole.Parser.Command, _context, _host);

            Assert.AreEqual("Goodbye", result);
        }

        [TestMethod]
        public void PipeChainExecuted()
        {
            _context.CommandContext.RegisterCommand<AddNumberCommand>();
            _context.CommandContext.RegisterCommand<SubtractNumberCommand>();
            _context.CommandContext.RegisterCommand<MultiplyNumberCommand>();
            _context.CommandContext.RegisterCommand<DivideNumberCommand>();

            var parseResult = PowerParser.ParseInput("Add-Number 3 7 | Subtract-Number 5 | Multiply-Number 4 | Divide-Number 40 -FlipArguments");
            var result = CommandExecuter.ExecuteChain(parseResult.Value as PipeChain, _context, _host);

            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void PipeChainExcessArgumentExecuted()
        {
            _context.CommandContext.RegisterCommand<AddNumberCommand>();
            _context.CommandContext.RegisterCommand<SubtractNumberCommand>();
            _context.CommandContext.RegisterCommand<MultiplyNumberCommand>();
            _context.CommandContext.RegisterCommand<DivideNumberCommand>();

            var parseResult = PowerParser.ParseInput("Add-Number 3 7 | Subtract-Number 5 5");
            try
            {
                CommandExecuter.ExecuteChain(parseResult.Value as PipeChain, _context, _host);
                Assert.Fail();
            }
            catch (UnexpectedPositionalArgument)
            {
            }
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
