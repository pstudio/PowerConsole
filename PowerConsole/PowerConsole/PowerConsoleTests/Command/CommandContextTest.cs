using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using pstudio.PowerConsole.Command;
using pstudio.PowerConsole.Command.Commands.Math;

namespace PowerConsoleTests.Command
{
    [TestClass]
    public class CommandContextTest
    {
        private CommandContext _context;

        [TestInitialize]
        public void Initialize()
        {
            _context = new CommandContext();
        }

        [TestMethod]
        public void CommandRegistered()
        {
            _context.RegisterCommand<AddNumberCommand>();

            Assert.IsTrue(_context.Commands.Count == 1);
            Assert.IsNotNull(_context.Commands["ADD-NUMBER"]);
        }

        [TestMethod]
        public void InvalidPositionCommandRegisterFailed()
        {
            try
            {
                _context.RegisterCommand<InvalidParameterPositionCommand>();
                Assert.Fail();
            }
            catch (InvalidParameterPositionException)
            {

            }
        }

        [TestMethod]
        public void InvalidNamedCommandRegisterFailed()
        {
            try
            {
                _context.RegisterCommand<InvalidNamedParameterCommand>();
                Assert.Fail();
            }
            catch (InvalidNamedParameterException)
            {

            }
        }

        [Command("Invalid", "Position")]
        public class InvalidParameterPositionCommand : pstudio.PowerConsole.Command.Command
        {
            [Parameter(Position = 1)]
            public int InvalidParameter { get; set; }

            public override object Process()
            {
                return null;
            }
        }

        [Command("Invalid", "Named")]
        public class InvalidNamedParameterCommand : pstudio.PowerConsole.Command.Command
        {
            [Parameter(Mandatory = true)]
            public bool InvalidParameter { get; set; }

            public override object Process()
            {
                return null;
            }
        }
    }

}
