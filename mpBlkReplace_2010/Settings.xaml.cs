using System.Windows.Controls;
using System.Windows.Input;
using mpSettings;
using ModPlus;

namespace mpBlkReplace
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
            MpWindowHelpers.OnWindowStartUp(
                this,
                MpSettings.GetValue("Settings", "MainSet", "Theme"),
                MpSettings.GetValue("Settings", "MainSet", "AccentColor"),
                MpSettings.GetValue("Settings", "MainSet", "BordersType")
                );
            bool b;
            ChkLayer.IsChecked = bool.TryParse(MpSettings.GetValue("Settings", "mpBlkReplace", "layer"), out b) && b;
            ChkTransform.IsChecked = bool.TryParse(MpSettings.GetValue("Settings", "mpBlkReplace", "transform"), out b) && b;
            ChkScales.IsChecked = bool.TryParse(MpSettings.GetValue("Settings", "mpBlkReplace", "scales"), out b) && b;
            ChkRotation.IsChecked = bool.TryParse(MpSettings.GetValue("Settings", "mpBlkReplace", "rotation"), out b) && b;
            int i;
            CbCleanBD.SelectedIndex = int.TryParse(MpSettings.GetValue("Settings", "mpBlkReplace", "cleanBD"), out i) ? i : 0;
        }

        private void Chk_OnChecked_OnUnchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var chkb = (CheckBox)sender;
            if (chkb != null)
            {
                var checkedValue = chkb.IsChecked != null && chkb.IsChecked.Value;
                switch (chkb.Name)
                {
                    case "ChkLayer":
                        MpSettings.SetValue("Settings", "mpBlkReplace", "layer", checkedValue.ToString(), true);
                        break;
                    case "ChkScales":
                        MpSettings.SetValue("Settings", "mpBlkReplace", "scales", checkedValue.ToString(), true);
                        break;
                    case "ChkTransform":
                        MpSettings.SetValue("Settings", "mpBlkReplace", "transform", checkedValue.ToString(), true);
                        if (checkedValue)
                        {
                            ChkRotation.IsEnabled = false;
                            ChkRotation.IsChecked = false;
                        }
                        else ChkRotation.IsEnabled = true;
                        break;
                    case "ChkRotation":
                        MpSettings.SetValue("Settings", "mpBlkReplace", "rotation",
                            chkb.IsEnabled ? checkedValue.ToString() : "False", true);
                        break;
                }
            }
        }

        private void BtOk_OnClick(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void CbCleanBD_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cb = (ComboBox) sender;
            if(cb != null && cb.SelectedIndex != -1)
                MpSettings.SetValue("Settings", "mpBlkReplace", "cleanBD", cb.SelectedIndex.ToString(), true);
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
