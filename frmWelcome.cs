
using System;
using System.Windows.Forms;

namespace Secure_Facial_Recognition_Security_System_using_EmguCV
{
    /// <summary>
    /// Welcomes user after a successful authentication
    /// </summary>
    public partial class frmWelcome : Form
    {

        #region Declaration
        // Holds User's name to welcome
        public string userName;
        #endregion
        #region Form Controls
        private void btnLogOut_Click(object sender, EventArgs e)
        {
            // Instantiating a frmAuthentication form to show
            frmAuthentication frm = new frmAuthentication();
            frm.Show();
            this.Dispose();
        }
        #endregion
        #region Form Events
        public frmWelcome()
        {
            InitializeComponent();
        }
        private void frmWelcome_Load(object sender, EventArgs e)
        {
            // Welcomes the logged in user
            lblWelcome.Text = $"Welcome, {userName}";
        }
        #endregion
    }
}
