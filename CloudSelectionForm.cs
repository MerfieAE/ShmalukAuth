using System.Windows.Forms;

namespace SmalukAuth
{
    public partial class CloudSelectionForm : Form
    {
        public string SelectedCloud { get; private set; }

        public CloudSelectionForm()
        {
            InitializeComponent();
            SetupForm();
        }

        private void SetupForm()
        {
            this.Text = "Выберите облачное хранилище";
            var yandexButton = new Button { Text = "Yandex", Dock = DockStyle.Top };
            var googleDriveButton = new Button { Text = "Google Drive", Dock = DockStyle.Top };
            var oneDriveButton = new Button { Text = "Bybit", Dock = DockStyle.Top };

            yandexButton.Click += (sender, e) => { SelectedCloud = "Yandex"; this.DialogResult = DialogResult.OK; this.Close(); };
            googleDriveButton.Click += (sender, e) => { SelectedCloud = "Google Drive"; this.DialogResult = DialogResult.OK; this.Close(); };
            oneDriveButton.Click += (sender, e) => { SelectedCloud = "Bybit"; this.DialogResult = DialogResult.OK; this.Close(); };

            this.Controls.Add(yandexButton);
            this.Controls.Add(googleDriveButton);
            this.Controls.Add(oneDriveButton);
        }

    }
}