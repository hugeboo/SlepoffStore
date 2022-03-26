using SlepoffStore.Tools;
using System.ComponentModel;
using System.Media;
using System.Windows.Forms;

namespace SlepoffStore;

public partial class ExceptionForm : Form
{
    public const string SERVER_CONNECTING_ERROR = "An error occurred while connecting to the server";
    public const string UNKNOWN_ERROR = "An unexpected error has occurred. Application will be closed";

    private bool _expanded = true;
    private readonly int _exceptionLabelHeight;
    private Action _onClosedAction;

    public bool Expanded
    {
        get => _expanded;
        set
        {
            if (_expanded != value)
            {
                if (value) this.Height += _exceptionLabelHeight;
                else this.Height -= _exceptionLabelHeight;
            }
            _expanded = value;
            richTextBox.Visible = _expanded;
            detailsButton.Text = "Details " + (_expanded ? "\u25B2" : "\u25BC");
        }
    }

    public static ExceptionForm ShowConnectingError(Exception ex, Action onClosed = null)
    {
        var message = SERVER_CONNECTING_ERROR;
        if (ex.InnerException is TimeoutException te)
        {
            message += ".\nThe server didn't respond in time";
            ex = ex.InnerException;
        }
        var form = new ExceptionForm();
        form.Init(message, ex, onClosed).ShowDialog();
        return form;
    }

    public ExceptionForm()
    {
        InitializeComponent();
        _exceptionLabelHeight = richTextBox.Height;
        Expanded = false;
    }

    public ExceptionForm Init(string errorMessage, Exception ex, Action onClosed = null)
    {
        errorLabel.Text = errorMessage;
        richTextBox.Text = ex.ToDetailedString();
        _onClosedAction = onClosed;
        return this;
    }

    private void detailsButton_Click(object sender, EventArgs e)
    {
        Expanded = !Expanded;
    }

    private void ExceptionForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        _onClosedAction?.Invoke();
    }

    private void ExceptionForm_Shown(object sender, EventArgs e)
    {
        SystemSounds.Exclamation.Play();
    }
}
