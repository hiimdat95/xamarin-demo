using KamooniHost.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.Settings
{
    public class SettingsViewModel : TinyViewModel
    {
        private List<HomeMenuItem> menuItems;

        public ObservableCollection<HomeMenuItem> MenuItems { get; set; } = new ObservableCollection<HomeMenuItem>();

        public ICommand NavigateToDetailCommand { get; private set; }

        public SettingsViewModel()
        {
            NavigateToDetailCommand = new AwaitCommand<HomeMenuItem>(NavigateToDetail);
        }

        public override void Init(object data)
        {
            base.Init(data);

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem
                {
                    Icon = "ic_setting_manage_servers_black",
                    Title = "Manage Servers",
                    TargetType = typeof(ManageServersViewModel),
                    Hide = true
                },
                new HomeMenuItem
                {
                    Icon = "ic_setting_menu_black",
                    Title = "Modify Master Menu",
                    TargetType = typeof(ModifyMasterMenuViewModel)
                },
                new HomeMenuItem
                {
                    Icon = "ic_camera_black",
                    Title = "Camera",
                    TargetType = typeof(CameraSettingsViewModel)
                },
                new HomeMenuItem
                {
                    Icon = "ic_setting_terms_black",
                    Title = "Terms and Conditions",
                    TargetType = typeof(TermsConditionsViewModel)
                },
                new HomeMenuItem
                {
                    Icon = "ic_setting_language_black",
                    Title = "Language",
                    TargetType = typeof(Nullable),
                    Hide = true
                },
                new HomeMenuItem
                {
                    Icon = "ic_account_box_black",
                    Title = "My Profile",
                    TargetType = typeof(MyProfileViewModel)
                },
                 new HomeMenuItem
                {
                    Icon = "icon_lock",
                    Title = "Set up PIN",
                    TargetType = typeof(AddNewOrUpdatePINViewModel)
                }
            };

            foreach (var item in menuItems)
            {
                if (!item.Hide)
                {
                    MenuItems.Add(item);
                }
            }
        }

        private async void NavigateToDetail(HomeMenuItem sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                if (sender.TargetType != null)
                {
                    await CoreMethods.PushViewModel(sender.TargetType, false);
                }
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.Message, TranslateExtension.GetValue("ok"));
            }
            finally
            {
                tcs.SetResult(true);
            }
        }
    }
}