using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hatzap.Textures;
using OpenTK.Graphics.OpenGL;

namespace Hatzap_Editor
{
    /// <summary>
    /// Interaction logic for TextureSettingsWindow.xaml
    /// </summary>
    public partial class TextureSettingsWindow : Window
    {
        public TextureSettingsWindow()
        {
            InitializeComponent();
        }
        
        public bool OKClicked { get; protected set; }

        public int TextureWidth { get; set; }

        public int TextureHeight { get; set; }

        public PixelInternalFormat? PixelInternalFormat { get; set; }

        public bool Mipmapped { get; set; }

        public bool MipmapsSaved { get; set; }

        public bool Compressed { get; set; }

        public bool CompressionSaved { get; set; }

        public TextureFiltering TextureFiltering { get; set; }

        public int AnisoLevel { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OKClicked = false;
            Width.Text = TextureWidth.ToString();
            Height.Text = TextureHeight.ToString();
            if(PixelInternalFormat != null) 
                FormatLayout.SelectedItem = PixelInternalFormat;
            chkCompressed.IsChecked = Compressed;
            chkSaveCompressed.IsChecked = CompressionSaved;
            chkMipmapped.IsChecked = Mipmapped;
            chkSaveMipmaps.IsChecked = MipmapsSaved;
            txtAniso.Text = AnisoLevel.ToString();
            cmbFiltering.SelectedItem = TextureFiltering;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            TextureWidth = int.Parse(Width.Text);
            TextureHeight = int.Parse(Height.Text);
            PixelInternalFormat = (PixelInternalFormat)FormatLayout.SelectedItem;
            Mipmapped = chkMipmapped.IsChecked == true;
            MipmapsSaved = chkSaveMipmaps.IsChecked == true;
            Compressed = chkCompressed.IsChecked == true;
            CompressionSaved = chkSaveCompressed.IsChecked == true;
            AnisoLevel = int.Parse(txtAniso.Text);
            TextureFiltering = (TextureFiltering)cmbFiltering.SelectedItem;
            OKClicked = true;
            Close();
        }

        private void Filtering_Loaded(object sender, RoutedEventArgs e)
        {
            List<TextureFiltering> data = new List<TextureFiltering>();
            data.Add(TextureFiltering.Nearest);
            data.Add(TextureFiltering.Bilinear);
            data.Add(TextureFiltering.Trilinear);

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = data;
            comboBox.SelectedIndex = 0;
        }

        private void FormatLayout_Loaded(object sender, RoutedEventArgs e)
        {
            List<PixelInternalFormat> data = new List<PixelInternalFormat>();
            data.Add(OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgba);
            data.Add(OpenTK.Graphics.OpenGL.PixelInternalFormat.SrgbAlpha);
            data.Add(OpenTK.Graphics.OpenGL.PixelInternalFormat.Rgb);
            data.Add(OpenTK.Graphics.OpenGL.PixelInternalFormat.Srgb);
            data.Add(OpenTK.Graphics.OpenGL.PixelInternalFormat.Rg8);
            data.Add(OpenTK.Graphics.OpenGL.PixelInternalFormat.R8);

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = data;
            comboBox.SelectedIndex = 0;
        }
    }
}
