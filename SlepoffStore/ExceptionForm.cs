using SlepoffStore.Tools;
using System.ComponentModel;
using System.Windows.Forms;

namespace SlepoffStore;

public partial class ExceptionForm : Form
{
    public const string SERVER_CONNECTING_ERROR = "An error occurred while connecting to the server";
    public const string UNKNOWN_ERROR = "An unexpected error has occurred. Application will be closed";

    private bool _expanded = true;
    private readonly int _exceptionLabelHeight;

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

    public ExceptionForm()
    {
        InitializeComponent();
        _exceptionLabelHeight = richTextBox.Height;
        Expanded = false;
    }

    public ExceptionForm Init(string errorMessage, Exception ex)
    {
        errorLabel.Text = errorMessage;
        richTextBox.Text = ex.ToDetailedString();
        return this;
    }

    private void detailsButton_Click(object sender, EventArgs e)
    {
        Expanded = !Expanded;
    }
}