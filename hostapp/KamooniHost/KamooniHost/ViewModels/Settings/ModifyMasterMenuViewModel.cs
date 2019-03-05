using KamooniHost.Constants;
using KamooniHost.Models;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extensions;
using Xamarin.Forms.TinyMVVM;

namespace KamooniHost.ViewModels.Settings
{
    public class ModifyMasterMenuViewModel : TinyViewModel
    {
        private readonly SQLiteConnection LocalDb;

        public ObservableCollection<HomeMenuItem> MasterMenus { get; set; } = new ObservableCollection<HomeMenuItem>();

        public ICommand SaveMasterMenuCommand { get; private set; }

        public ModifyMasterMenuViewModel(SQLiteConnection LocalDb)
        {
            this.LocalDb = LocalDb;

            SaveMasterMenuCommand = new AwaitCommand(SaveMasterMenu);

            InitMasterMenus();
        }

        private void InitMasterMenus()
        {
            foreach (var menu in LocalDb.Table<HomeMenuItem>().ToList())
            {
                if (menu.Id == HomeMenuItemType.Bookings)
                {
                    MasterMenus.Add(new HomeMenuItem()
                    {
                        Id = menu.Id,
                        Title = menu.Title,
                        Hide = menu.Hide
                    });
                }
            }
        }

        private async void SaveMasterMenu(object sender, TaskCompletionSource<bool> tcs)
        {
            try
            {
                var current = LocalDb.Table<HomeMenuItem>().ToList();

                LocalDb.DeleteAll<HomeMenuItem>();

                foreach (var menu in MasterMenus)
                {
                    if (current.ToList().Find(m => m.Id == menu.Id) is HomeMenuItem menuItem)
                    {
                        menuItem.Title = menu.Title;
                        menuItem.Hide = menu.Hide;
                    }
                }

                foreach (var menu in current)
                {
                    LocalDb.Insert(new HomeMenuItem { Id = menu.Id, Title = menu.Title, Icon = menu.Icon, Hide = menu.Hide });
                }

                MessagingCenter.Send(this, MessageKey.HOME_MENUS_CHANGED);

                await CoreMethods.PopViewModel();

                tcs.SetResult(true);
            }
            catch (Exception ex)
            {
                await CoreMethods.DisplayAlert(TranslateExtension.GetValue("error"), ex.GetBaseException().Message, TranslateExtension.GetValue("ok"));

                tcs.SetResult(true);
            }
        }
    }
}