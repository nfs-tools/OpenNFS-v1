using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenNFSUI
{
    public partial class ChoiceButton : UserControl
    {
        private static Color HoverColor = Color.FromArgb(31, 31, 31);
        private static Color DefaultColor = Color.FromArgb(60, 63, 65);

        public Action OnClickAction { get; set; }

        public string Title { get; private set; }
        public string Description { get; private set; }
        public Image Icon { get; private set; }

        public ChoiceButton(string title, string description, Image icon, Action onClickAction)
        {
            Title = title;
            Description = description;
            Icon = icon;

            OnClickAction = onClickAction;

            InitializeComponent();
        }

        private void InitializeButton()
        {
            // Initialize design
            btnTitle.Text = Title.ToUpper();
            btnDesc.Text = Description;
            btnIcon.BackgroundImage = Icon;

            // Initialize Events
            Click += ClickEvent;
            btnTitle.Click += ClickEvent;
            btnDesc.Click += ClickEvent;
            btnIcon.Click += ClickEvent;
            divider.Click += ClickEvent;

            MouseEnter += MouseEnterEvent;
            btnTitle.MouseEnter += MouseEnterEvent;
            btnDesc.MouseEnter += MouseEnterEvent;
            btnIcon.MouseEnter += MouseEnterEvent;
            divider.MouseEnter += MouseEnterEvent;

            MouseLeave += MouseExitEvent;
            btnTitle.MouseLeave += MouseExitEvent;
            btnDesc.MouseLeave += MouseExitEvent;
            btnIcon.MouseLeave += MouseExitEvent;
            divider.MouseLeave += MouseExitEvent;
        }

        #region Events
        private void MouseEnterEvent(object sender, EventArgs e)
        {
            BackColor = HoverColor;
        }

        private void MouseExitEvent(object sender, EventArgs e)
        {
            BackColor = DefaultColor;
        }

        private void ClickEvent(object sender, EventArgs e)
        {
            OnClickAction?.Invoke();
        }

        private void ChoiceButton_Load(object sender, EventArgs e)
        {
            InitializeButton();
        }
        #endregion

        private void ChoiceButton_MouseEnter(object sender, EventArgs e)
        {

        }
    }
}
