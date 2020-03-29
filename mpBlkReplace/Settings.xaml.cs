namespace mpBlkReplace
{
    using System.Windows.Controls;
    using ModPlusAPI;

    public partial class Settings
    {
        private readonly string _plName = new ModPlusConnector().Name;

        public Settings()
        {
            InitializeComponent();
            ChkLayer.IsChecked = bool.TryParse(UserConfigFile.GetValue(_plName, "layer"), out bool b) && b;
            ChkTransform.IsChecked = bool.TryParse(UserConfigFile.GetValue(_plName, "transform"), out b) && b;
            ChkScales.IsChecked = bool.TryParse(UserConfigFile.GetValue(_plName, "scales"), out b) && b;
            ChkRotation.IsChecked = bool.TryParse(UserConfigFile.GetValue(_plName, "rotation"), out b) && b;
            CbCleanBD.SelectedIndex = int.TryParse(UserConfigFile.GetValue(_plName, "cleanBD"), out int i) ? i : 0;
        }

        private void Chk_OnChecked_OnUnchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            var checkBox = (CheckBox)sender;
            if (checkBox != null)
            {
                var checkedValue = checkBox.IsChecked != null && checkBox.IsChecked.Value;
                switch (checkBox.Name)
                {
                    case "ChkLayer":
                        UserConfigFile.SetValue(_plName, "layer", checkedValue.ToString(), true);
                        break;
                    case "ChkScales":
                        UserConfigFile.SetValue(_plName, "scales", checkedValue.ToString(), true);
                        break;
                    case "ChkTransform":
                        UserConfigFile.SetValue(_plName, "transform", checkedValue.ToString(), true);
                        if (checkedValue)
                        {
                            ChkRotation.IsEnabled = false;
                            ChkRotation.IsChecked = false;
                        }
                        else
                        {
                            ChkRotation.IsEnabled = true;
                        }

                        break;
                    case "ChkRotation":
                        UserConfigFile.SetValue(_plName, "rotation",
                            checkBox.IsEnabled ? checkedValue.ToString() : "False", true);
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
            var cb = (ComboBox)sender;
            if (cb != null && cb.SelectedIndex != -1)
                UserConfigFile.SetValue(_plName, "cleanBD", cb.SelectedIndex.ToString(), true);
        }
    }
}
