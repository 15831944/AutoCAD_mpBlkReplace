using System.Windows.Controls;
using System.Windows.Input;
using ModPlusAPI;
using ModPlusAPI.Windows.Helpers;

namespace mpBlkReplace
{
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
            this.OnWindowStartUp();
            ChkLayer.IsChecked = bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "layer"), out bool b) && b;
            ChkTransform.IsChecked = bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "transform"), out b) && b;
            ChkScales.IsChecked = bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "scales"), out b) && b;
            ChkRotation.IsChecked = bool.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "rotation"), out b) && b;
            CbCleanBD.SelectedIndex = int.TryParse(UserConfigFile.GetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "cleanBD"), out int i) ? i : 0;
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
                        UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "layer", checkedValue.ToString(), true);
                        break;
                    case "ChkScales":
                        UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "scales", checkedValue.ToString(), true);
                        break;
                    case "ChkTransform":
                        UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "transform", checkedValue.ToString(), true);
                        if (checkedValue)
                        {
                            ChkRotation.IsEnabled = false;
                            ChkRotation.IsChecked = false;
                        }
                        else ChkRotation.IsEnabled = true;
                        break;
                    case "ChkRotation":
                        UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "rotation",
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
                UserConfigFile.SetValue(UserConfigFile.ConfigFileZone.Settings, "mpBlkReplace", "cleanBD", cb.SelectedIndex.ToString(), true);
        }

        private void Settings_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
