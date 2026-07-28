using CmlLib.Core.Auth;
using CmlLib.Core.Auth.Microsoft.Sessions;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Nelian
{
    public partial class AccountControl : UserControl
    {
        public JEGameAccount Account { get; }

        public event EventHandler<JEGameAccount>? LoginClicked;
        public event EventHandler<JEGameAccount>? RemoveClicked;
        public event Action<string>? OfflineLoginClicked;
        public event Action? OfflineRemoveClicked;
        private bool _isOffline = false;
        private string _offlineUsername = "";

        public AccountControl(string username)
        {
            InitializeComponent();

            _isOffline = true;
            _offlineUsername = username;

            lbUsername.Text = username;
            lbIdentifier.Text = LanguageManager.Get("AccountManager.OfflineMode");

            if (btnLogin != null)
                btnLogin.Text = LanguageManager.Get("AccountControl.Login");

            if (btnRemove != null)
                btnRemove.Text = LanguageManager.Get("AccountControl.Remove");

            if (btnLogin != null)
                btnLogin.Click += btnLogin_Click_1;

            if (btnRemove != null)
                btnRemove.Click += btnRemove_Click;

            _ = LoadAvatarAsync("8667ba71b85a4004af54457a9734eed7");

            this.Load += AccountControl_Load;
        }

        public AccountControl(JEGameAccount account)
        {
            InitializeComponent();

            Account = account ?? throw new ArgumentNullException(nameof(account));

            // Metinleri LanguageManager'dan al
            lbUsername.Text = account.Profile?.Username ?? "Unknown";
            lbIdentifier.Text = account.Identifier ?? "Unknown";

            // Buton metinlerini güncelle
            if (btnLogin != null)
                btnLogin.Text = LanguageManager.Get("AccountControl.Login");

            if (btnRemove != null)
                btnRemove.Text = LanguageManager.Get("AccountControl.Remove");

            // Button event baglantilari
            if (btnLogin != null)
            {
                btnLogin.Click += btnLogin_Click_1;
            }

            if (btnRemove != null)
            {
                btnRemove.Click += btnRemove_Click;
            }

            _ = LoadAvatarAsync(account.Profile?.UUID);
            this.Load += AccountControl_Load;
        }

        private void AccountControl_Load(object sender, EventArgs e)
        {
            CenterLabels();
            CenterPictureBox();
        }

        private void CenterPictureBox()
        {
            if (pbAvatar.Parent == null) return;

            var parentWidth = pbAvatar.Parent.ClientSize.Width;
            pbAvatar.Left = (parentWidth - pbAvatar.Width) / 2;
            pbAvatar.Top = 5;
        }

        private async Task LoadAvatarAsync(string? uuid)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(uuid))
                    return;

                using var http = new HttpClient();
                var bytes = await http.GetByteArrayAsync(
                    $"https://mc-heads.net/head/{uuid}"
                );

                using var ms = new MemoryStream(bytes);
                var img = Image.FromStream(ms);

                if (pbAvatar.InvokeRequired)
                {
                    pbAvatar.Invoke(new Action(() =>
                    {
                        pbAvatar.Image = img;
                        pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                    }));
                }
                else
                {
                    pbAvatar.Image = img;
                    pbAvatar.SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
            catch
            {
                pbAvatar.Image = null;
            }
        }

        private void CenterLabels()
        {
            CenterLabel(lbUsername);
            CenterLabel(lbIdentifier);
        }

        private void CenterLabel(Label lbl)
        {
            if (lbl.Parent == null) return;

            var parentWidth = lbl.Parent.ClientSize.Width;
            var textWidth = TextRenderer.MeasureText(lbl.Text, lbl.Font).Width;
            lbl.Left = (parentWidth - textWidth) / 2;
        }

        private void btnLogin_Click_1(object sender, EventArgs e)
        {
            if (_isOffline)
            {
                OfflineLoginClicked?.Invoke(_offlineUsername);
                return;
            }

            if (Account == null)
                return;

            LoginClicked?.Invoke(this, Account);
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (_isOffline)
            {
                OfflineRemoveClicked?.Invoke();
                return;
            }

            if (Account == null)
                return;

            RemoveClicked?.Invoke(this, Account);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
