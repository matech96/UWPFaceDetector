using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace FaceDetection
{
    public sealed partial class NameBox : UserControl
    {
        public NameBox()
        {
            this.InitializeComponent();
        }

        public string NameText
        {
            get
            {
                return NameTextBlock.Text;
            }
            set
            {
                NameTextBlock.Text = value;
            }
        }
        public string Description
        {
            get
            {
                return ToolTip.Content as string;
            }
            set
            {
                ToolTip.Content = value;
            }
        }
        public new double Width
        {
            get
            {
                return Box.Width;
            }
            set
            {
                Box.Width = value;
            }
        }
        public new double Height
        {
            get
            {
                return Box.Height;
            }
            set
            {
                Box.Height = value;
                NameTextBlock.FontSize = value / 4;
            }
        }
    }
}
