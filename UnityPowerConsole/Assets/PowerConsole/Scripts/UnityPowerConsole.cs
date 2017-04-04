
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using pstudio.PowerConsole;
using pstudio.PowerConsole.Command.Commands.Math;
using pstudio.PowerConsole.Context;
using pstudio.PowerConsole.Host;
using TMPro;
using UnityEngine;


public class UnityPowerConsole : MonoBehaviour, IHost
{

    [SerializeField] private TMP_InputField _input;
    [SerializeField] private TMP_InputField _output;

    private PowerConsole _console;


    [UsedImplicitly]
    private void Awake()
    {
        var context = new DefaultContext();
        context.CommandContext.RegisterCommand<AddNumberCommand>();
        context.CommandContext.RegisterCommand<SubtractNumberCommand>();
        context.CommandContext.RegisterCommand<MultiplyNumberCommand>();
        context.CommandContext.RegisterCommand<DivideNumberCommand>();

        _console = new PowerConsole(this, context);
    }

    [UsedImplicitly]
    private void OnEnable()
    {
        _input.onSubmit.AddListener(SubmitCommand);
    }

    [UsedImplicitly]
    private void OnDisable()
    {
        _input.onSubmit.RemoveListener(SubmitCommand);
    }

    private void SubmitCommand(string command)
    {
        _input.text = string.Empty;

        OutputText(command, OutputColorType.Default);


        _console.Execute(command);

        //var history = _console.GetHistory();
        //StringBuilder sb = new StringBuilder();
        //foreach (var s in history)
        //{
        //    sb.AppendLine(s);
        //}
        //sb.AppendLine(reply);
        //_output.text += "\n" + command;

        //_output.text = sb.ToString();
        //_output.verticalScrollbar.value = 1;

        _input.ActivateInputField();
    }

    private static readonly string[] ColorPalette =
    {
        "#F0F0F0",
        "#F0F020",
        "#AAAAAA",
        "#FF2020",
        "#FFAAAA",
        "#AAAAFF",
    };

    public string FormatColor(string message, OutputColorType colorType)
    {
        return "<color=" + ColorPalette[(int) colorType] + ">" + message + "</color>";
    }

    private void OutputText(string message, OutputColorType colorType)
    {
        _output.text += FormatColor(message + "\n", colorType);
        //_output.verticalScrollbar.value = 1;
    }

    public void Write(string message)
    {
        OutputText("~ " + message, OutputColorType.Default);
    }

    public void WriteDebug(string debugMessage)
    {
        OutputText("[DEBUG]~ " + debugMessage, OutputColorType.Debug);
    }

    public void WriteError(string errorMessage)
    {
        OutputText("[ERROR]~ " + errorMessage, OutputColorType.Error);
    }

    public bool SupportsColor { get { return true; } }
}
